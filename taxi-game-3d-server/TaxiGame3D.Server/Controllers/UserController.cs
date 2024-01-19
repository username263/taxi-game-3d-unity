using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaxiGame3D.Server.DTOs;
using TaxiGame3D.Server.Repositories;
using TaxiGame3D.Server.Services;
using TaxiGame3D.Server.Templates;

namespace TaxiGame3D.Server.Controllers;

[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    readonly UserRepository userRepository;
    readonly TemplateService templateService;

    public UserController(UserRepository userRepository, TemplateService templateService)
    {
        this.userRepository = userRepository;
        this.templateService = templateService;
    }

    
    [HttpGet]
    [ProducesResponseType<UserResponse>(200)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<UserResponse>> Get()
    {
        var userId = ClaimHelper.FindNameIdentifier(User);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await userRepository.Get(userId);
        if (user == null)
            return Forbid();

        var utcToday = DateTime.UtcNow.Date;
        var rand = new Random((int)DateTime.Now.Ticks);
        var cars = await templateService.GetCars();
        var carsForReward = cars
            .Where(e => e.EnableReward)
            .OrderBy(e => rand.Next())
            .ToList();
        var carIndex = 0;
        var dailyRewards = await templateService.GetDailyRewards();
        var needUpdate = false;

        // 자동차 지급
        if (user.Cars == null || user.Cars.Count == 0)
        {
            var newCar = cars?.FirstOrDefault(e => e.Cost == 0);
            if (newCar != null)
            {
                user.CurrentCarId = newCar?.Id;
                user.Cars = [ user.CurrentCarId! ];
                needUpdate = true;
            }
        }

        // 출석보상 생성
        // 어제 모든 보상을 다 받았다면 보상생성
        var rewardedAt = user.DailyRewardedAtUtc;
        if (
            rewardedAt <= DateTime.UnixEpoch ||
            (user.NumberOfAttendance >= dailyRewards.Count && utcToday > rewardedAt)
        )
        {
            user.NumberOfAttendance = 0;
            if (user.DailyCarRewards == null)
                user.DailyCarRewards = new();
            else
                user.DailyCarRewards.Clear();
            for (int i = 0; i < dailyRewards.Count; i++)
            {
                if (dailyRewards[i].Type == DailyRewardType.Car)
                {
                    user.DailyCarRewards[i.ToString()] = carsForReward[carIndex].Id;
                    carIndex = (carIndex + 1) % carsForReward.Count;
                }
            }
            // 출석체크하기 전까지 계속 데이터 갱신하지 않도록 처리
            if (rewardedAt <= DateTime.UnixEpoch)
                user.DailyRewardedAtUtc = utcToday.AddYears(-1);
            needUpdate = true;
        }

        if (user.CoinCollectedAtUtc <= DateTime.UnixEpoch)
        {
            user.CoinCollectedAtUtc = DateTime.UtcNow;
            needUpdate = true;
        }

        // 룰렛 보상 생성
        // 어제 룰렛을 돌렸다면 보상생성
        rewardedAt = user.RouletteSpunAtUtc;
        if (rewardedAt <= DateTime.UnixEpoch || utcToday > rewardedAt)
        {
            if (user.RouletteCarRewards == null)
                user.RouletteCarRewards = new();
            else
                user.RouletteCarRewards.Clear();
            const int rouletteCount = 6;
            for (int i = 0; i < rouletteCount; i++)
            {
                user.RouletteCarRewards.Add(carsForReward[carIndex].Id);
                carIndex = (carIndex + 1) % carsForReward.Count;
            }
            // 룰렛 돌리기 전까지 계속 데이터 갱신하지 않도록 처리
            if (rewardedAt <= DateTime.UnixEpoch)
                user.RouletteSpunAtUtc = utcToday.AddYears(-1);
            needUpdate = true;
        }

        if (needUpdate)
            _ = userRepository.Update(userId, user);

        return Ok(new UserResponse(user));
    }

    [HttpPut("SelectCar/{carId}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> SelectCar(string carId)
    {
        var userId = ClaimHelper.FindNameIdentifier(User);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await userRepository.Get(userId);
        if (user == null)
            return Forbid();

        // 보유하지 않는 차는 선택 불가능
        var exist = user.Cars?.Contains(carId) ?? false;
        if (!exist)
            return NotFound();

        if (user.CurrentCarId != carId)
        {
            user.CurrentCarId = carId;
            await userRepository.Update(user.Id!, user);
        }

        return NoContent();
    }

    [HttpPut("BuyCar/{carId}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> BuyCar(string carId)
    {
        var userId = ClaimHelper.FindNameIdentifier(User);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await userRepository.Get(userId);
        if (user == null)
            return Forbid();

        if (user.Cars == null)
            user.Cars = new();

        // 이미 보유한 차는 구매 불가능
        if (user.Cars.Contains(carId))
            return NoContent();

        var carTemps = await templateService.GetCars();
        var carTemp = carTemps?.Find(e => e.Id == carId);
        if (carTemp == null)
            return NotFound();

        // 비용 부족
        if (user.Coin < carTemp.Cost)
            return Forbid();

        user.Coin -= carTemp.Cost;
        user.Cars.Add(carId);
        await userRepository.Update(user.Id!, user);

        return NoContent();
    }

    [HttpPut("EndStage")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> EndStage([FromBody] EndStageRequest body)
    {
        var userId = ClaimHelper.FindNameIdentifier(User);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await userRepository.Get(userId);
        if (user == null)
            return Forbid();

        var stageTemps = await templateService.GetStages();
        var stageCount = stageTemps?.Count ?? 0;
        var stageIndex = body.StageIndex;
        // 템플릿에 등록되지 않은 스테이지
        if (stageIndex < 0 || body.StageIndex >= stageCount)
            return BadRequest();

        // 현재 스테이지도 아직 클리어하지 않았을 경우
        if (stageIndex > user.CurrentStageIndex)
            return Forbid();

        // 클라이언트에서 계산한 요금이 서버에서 계산한 요금보다 더 큰 경우
        var maxCoin = stageTemps?[stageIndex]?.MaxCoin ?? 0;
        if (body.Coin >= maxCoin)
            return Forbid();

        if (body.IsGoal)
        {
            // 다음 스테이지 개방
            if (stageIndex == user.CurrentStageIndex && stageIndex < stageCount + 1)
                user.CurrentStageIndex = stageIndex + 1;
        }
        user.Coin += body.Coin;
        await userRepository.Update(user.Id!, user);

        return NoContent();
    }

    [HttpPut("Attendance")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Attendance([FromBody] DateRequest body)
    {
        var userId = ClaimHelper.FindNameIdentifier(User);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await userRepository.Get(userId);
        if (user == null)
            return Forbid();

        var rewards = await templateService.GetDailyRewards();
        // 출석 완료함
        if (user.NumberOfAttendance >= rewards.Count)
            return StatusCode(StatusCodes.Status410Gone);

        // 오늘 이미 출석함
        if (body.UtcDate <= user.DailyRewardedAtUtc.Date.AddDays(1))
            return StatusCode(StatusCodes.Status410Gone);

        var r = rewards[user.NumberOfAttendance];
        if (r.Type == DailyRewardType.Coin)
        {
            // 돈 지급
            user.Coin += r.Amount;
        }
        else
        {
            var cars = await templateService.GetCars();
            // 미리 지급할 자동차를 지급
            if (!user.DailyCarRewards.TryGetValue(user.NumberOfAttendance.ToString(), out var carId))
                carId = cars.FirstOrDefault(e => e.EnableReward).Id;
            if (user.Cars.Contains(carId))
            {
                // 이미 가지고 있으면, 자동차 가격만큼 돈을 지급
                var car = cars.Find(e => e.Id == carId);
                user.Coin += car.Cost;
            }
            else
            {
                user.Cars.Add(carId);
            }
        }
        ++user.NumberOfAttendance;
        user.DailyRewardedAtUtc = body.UtcDate;
        await userRepository.Update(userId, user);

        return NoContent();
    }

    [HttpPut("SpinRoulette")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<RouletteResponse>> SpinRoulette([FromBody] DateRequest body)
    {
        var userId = ClaimHelper.FindNameIdentifier(User);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await userRepository.Get(userId);
        if (user == null)
            return Forbid();

        // 오늘 이미 룰렛 돌림
        if (body.UtcDate >= user.RouletteSpunAtUtc.Date.AddDays(1))
            return StatusCode(StatusCodes.Status410Gone);

        var cars = await templateService.GetCars();
        var index = new Random((int)DateTime.Now.Ticks).Next(user.RouletteCarRewards.Count);
        var car = cars.Find(e => e.Id == user.RouletteCarRewards[index]);
        if (car == null)
            return NotFound();

        if (user.Cars.Contains(car.Id))
            user.Coin += car.Cost;
        else
            user.Cars.Add(car.Id);
        user.RouletteSpunAtUtc = body.UtcDate;
        await userRepository.Update(userId, user);

        return Ok(new RouletteResponse
        {
            Index = index
        });
    }

    [HttpPut("CollectCoin")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> CollectCoin([FromBody] DateRequest body)
    {
        var userId = ClaimHelper.FindNameIdentifier(User);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await userRepository.Get(userId);
        if (user == null)
            return Forbid();

        if (body.UtcDate <= user.CoinCollectedAtUtc)
            return StatusCode(StatusCodes.Status410Gone);

        var stages = await templateService.GetStages();
        var stage = stages[user.CurrentStageIndex];
        int minutes = (int)(body.UtcDate - user.CoinCollectedAtUtc).TotalMinutes;
        if (minutes <= 0)
            return NoContent();

        user.Coin += Math.Min(minutes, stage.MaxCollect);
        user.CoinCollectedAtUtc = body.UtcDate;
        await userRepository.Update(userId, user);

        return NoContent();
    }
}
