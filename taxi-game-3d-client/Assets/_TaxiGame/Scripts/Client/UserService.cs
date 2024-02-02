using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

namespace TaxiGame3D
{
    public class UserService : MonoBehaviour
    {
        HttpContext http;
        TemplateService templateService;

        public event EventHandler UserUpdateFailed;

        public UserModel User
        {
            get;
            private set;
        }

        void Start()
        {
            http = ClientManager.Instance?.Http;
            templateService = ClientManager.Instance?.TemplateService;
        }

        public async UniTask<HttpStatusCode> Load()
        {
            var res = await http.Get<UserResponse>("User");
            if (!res.Item1.IsSuccess())
            {
                Debug.LogWarning($"Load user failed. - {res.Item1}");
                return res.Item1;
            }

            User = new()
            {
                Nickname = res.Item2.Nickname,
                Coin = res.Item2.Coin,
                Cars = res.Item2.Cars
                    .Select(id => templateService.Cars.Find(e => e.Id == id))
                    .ToList(),
                CurrentCar = templateService.Cars
                    .Find(e => e.Id == res.Item2.CurrentCar),
                CurrentStage = templateService.Stages[res.Item2.CurrentStage],
                DailyCarRewards = new(res.Item2.DailyCarRewards.Select(
                    e => new KeyValuePair<int, CarTemplate>(
                        int.Parse(e.Key),
                        templateService.Cars.Find(c => c.Id == e.Value)
                    )
                )),
                DailyRewardedAtUtc = res.Item2.DailyRewardedAt,
                NumberOfAttendance = res.Item2.NumberOfAttendance,
                CoinCollectedAtUtc = res.Item2.CoinCollectedAt,
                RouletteCarRewards = res.Item2.RouletteCarRewards
                    .Select(id => templateService.Cars.Find(e => e.Id == id))
                    .ToList(),
                RouletteSpunAtUtc = res.Item2.RouletteSpunAt
            };

            return res.Item1;
        }

        public async void SelectCar(string carId)
        {
            var car = User.Cars.Find(e => e.Id == carId);
            if (car == null)
            {
                Debug.LogError($"Select car failed. Because user has not {carId}.");
                return;
            }
            User.CurrentCar = car;
            
            var res = await http.Put($"User/SelectCar/{carId}", null);
            if (!res.IsSuccess())
            {
                Debug.LogWarning($"Select car failed. - {res}");
                UserUpdateFailed?.Invoke(this, EventArgs.Empty);
            }
        }

        public async UniTask<HttpStatusCode> BuyCar(string carId)
        {
            if (User.Cars.Any(e => e.Id == carId))
            {
                Debug.LogWarning($"Buy car failed. Because user already owned {carId}.");
                return HttpStatusCode.NoContent;
            }

            var car = templateService.Cars.Find(e => e.Id == carId);
            if (car == null)
            {
                Debug.LogError($"Buy car failed. Because {carId} is not exist in templates.");
                return HttpStatusCode.NotFound;
            }
            
            if (User.Coin < car.Cost)
            {
                Debug.LogWarning($"Buy car failed. Because of not enough coin.");
                return HttpStatusCode.Forbidden;
            }

            User.Coin -= car.Cost;
            User.Cars.Add(car);
            
            var res = await http.Put($"User/BuyCar/{carId}", null);
            if (!res.IsSuccess())
            {
                Debug.LogWarning($"Buy car failed. - {res}");
                UserUpdateFailed?.Invoke(this, EventArgs.Empty);
            }
            return res;
        }

        public async UniTask<HttpStatusCode> EndStage(int stageIndex, bool isGoal, int coin)
        {
            var currStageIndex = User.CurrentStage.Index;
            var stageCount = templateService.Stages.Count;

            if (stageIndex < 0 || stageIndex >= stageCount)
            {
                Debug.LogError($"End stage failed. Because {stageIndex} is out of range.");
                return HttpStatusCode.BadRequest;
            }
            
            if (stageIndex > currStageIndex)
            {
                Debug.LogWarning($"End stage({stageIndex}) failed. Because {User.CurrentStage.Index} was not clear.");
                return HttpStatusCode.Forbidden;
            }

            if (coin >= templateService.Stages[stageIndex].MaxCoin)
            {
                Debug.LogError($"End stage failed. Because {coin} is too much.");
                return HttpStatusCode.Forbidden;
            }

            if (isGoal)
            {
                if (stageIndex == currStageIndex && stageIndex < stageCount - 1)
                    User.CurrentStage = templateService.Stages[stageIndex + 1];
            }
            User.Coin += coin;

            var res = await http.Put($"User/EndStage", new EndStageRequest
            {
                StageIndex = stageIndex,
                IsGoal = isGoal,
                Coin = coin
            });
            if (!res.IsSuccess())
            {
                Debug.LogWarning($"End stage failed. - {res}");
                UserUpdateFailed?.Invoke(this, EventArgs.Empty);
            }
            return res;
        }

        public bool CheckEnableAttendance()
        {
            var now = DateTime.UtcNow;
            // 출석 완료함
            if (User.NumberOfAttendance >= templateService.DailyRewards.Count)
            {
                Debug.LogWarning("Attendance failed. Because attendance is already completed.");
                return false;
            }

            // 오늘 이미 출석함
            if (now <= User.DailyRewardedAtUtc.Date.AddDays(1))
            {
                Debug.LogWarning($"Attendance failed. Because of already rewarded today({now}/{User.DailyRewardedAtUtc}).");
                return false;
            }
            return true;
        }

        public async void Attendance()
        {
            if (!CheckEnableAttendance())
                return;

            var reward = templateService.DailyRewards[User.NumberOfAttendance];
            if (reward.Type == DailyRewardType.Coin)
            {
                User.Coin += reward.Amount;
            }
            else
            {
                User.DailyCarRewards.TryGetValue(User.NumberOfAttendance, out var carTemp);
                if (User.Cars.Contains(carTemp))
                    User.Coin += carTemp.Cost;
                else
                    User.Cars.Add(carTemp);
            }
            ++User.NumberOfAttendance;
            User.DailyRewardedAtUtc = DateTime.UtcNow;

            var res = await http.Put($"User/Attendance", new DateRequest
            {
                UtcDate = User.DailyRewardedAtUtc
            });
            if (!res.IsSuccess())
            {
                Debug.LogWarning($"Attendance failed. - {res}");
                UserUpdateFailed?.Invoke(this, EventArgs.Empty);
            }
        }

        public async UniTask<(int index, bool newCar)> SpinRoulette()
        {
            var now = DateTime.UtcNow;
            var res = await http.Put<RouletteResponse>("User/SpinRoulette", new DateRequest
            {
                UtcDate = now
            });
            if (!res.Item1.IsSuccess())
            {
                Debug.LogWarning($"Spin roulette failed. - {res}");
                UserUpdateFailed?.Invoke(this, EventArgs.Empty);
                return (-1, false);
            }
            User.RouletteSpunAtUtc = now;
            var car = User.RouletteCarRewards[res.Item2.Index];
            if (User.Cars.Contains(car))
            {
                User.Coin += car.Cost;
                return (res.Item2.Index, false);
            }
            else
            {
                User.Cars.Add(car);
                return (res.Item2.Index, true);
            }
        }

        public async void CollectCoin()
        {
            var now = DateTime.UtcNow;
            if (now <= User.CoinCollectedAtUtc)
                return;

            var minutes = (int)(now - User.CoinCollectedAtUtc).TotalMinutes;
            if (minutes <= 0)
                return;

            User.CoinCollectedAtUtc = now;
            User.Coin += Math.Min(minutes, User.CurrentStage.MaxCollect);

            var res = await http.Put($"User/CollectCoin", new DateRequest
            {
                UtcDate = now
            });
            if (!res.IsSuccess())
            {
                Debug.LogWarning($"Collect coin failed. - {res}");
                UserUpdateFailed?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Clear()
        {
            User = null;
        }
    }
}
