﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaxiGame3D.Server.DTOs;
using TaxiGame3D.Server.Repositories;
using TaxiGame3D.Server.Services;

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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> Get()
    {
        var userId = ClaimHelper.FindNameIdentifier(User);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await userRepository.Get(userId);
        if (user == null)
            return Forbid();

        // 자동차 지급
        if (user.Cars == null || user.Cars.Count == 0)
        {
            var cars = await templateService.GetCars();
            var newCar = cars.FirstOrDefault(e => e.Cost == 0);
            if (newCar != null)
            {
                user.CurrentCarId = newCar?.Id;
                user.Cars = [ user.CurrentCarId! ]; 
            }
        }

        return Ok(new
        {
            user.Nickname,
            user.Coin,
            user.Cars,
            CurrentCar = user.CurrentCarId,
            CurrentStage = user.CurrentStageIndex
        });
    }

    [HttpPut("SelctCar/{carId}")]
    public async Task<ActionResult> SelectCar(string carId)
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
    public async Task<ActionResult> BuyCar(string carId)
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
        var carTemp = carTemps.Find(e => e.Id == carId);
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

    [HttpPut]
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
        // 템플릿에 등록되지 않은 스테이지
        if (body.StageIndex < 0 || body.StageIndex >= stageTemps.Count)
            return BadRequest();

        // 현재 스테이지도 아직 클리어 하지 않았을 경우
        if (body.StageIndex > user.CurrentStageIndex)
            return Forbid();

        // 다음 스테이지 개방
        if (
            body.StageIndex == user.CurrentStageIndex && 
            user.CurrentStageIndex < stageTemps.Count - 1
        )
        {
            ++user.CurrentStageIndex;
        }

        // TODO: 나중에 보상금 검증하는 기능 추가
        user.Coin += body.Coin;
        await userRepository.Update(user.Id!, user);

        return NoContent();
    }
}
