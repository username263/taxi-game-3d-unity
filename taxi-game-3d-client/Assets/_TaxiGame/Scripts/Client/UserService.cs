using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
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
                    .Select(id => templateService.CarTemplates.Find(e => e.Id == id))
                    .ToList(),
                CurrentCar = templateService.CarTemplates
                    .Find(e => e.Id == res.Item2.CurrentCar),
                CurrentStage = templateService.StageTemplates[res.Item2.CurrentStage]
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

            var car = templateService.CarTemplates.Find(e => e.Id == carId);
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

        public async UniTask<HttpStatusCode> EndStage(int stageIndex, long coin)
        {
            var currStageIndex = User.CurrentStage.Index;
            var stageCount = templateService.StageTemplates.Count;

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

            if (coin >= templateService.StageTemplates[stageIndex].MaxCoin)
            {
                Debug.LogError($"End stage failed. Because {coin} is too much.");
                return HttpStatusCode.Forbidden;
            }

            if (stageIndex == currStageIndex && stageIndex < stageCount - 1)
                User.CurrentStage = templateService.StageTemplates[stageIndex + 1];
            User.Coin += coin;

            var res = await http.Put($"User/EndStage", new EndStageRequest
            {
                StageIndex = stageIndex,
                Coin = coin
            });
            if (!res.IsSuccess())
            {
                Debug.LogWarning($"End stage failed. - {res}");
                UserUpdateFailed?.Invoke(this, EventArgs.Empty);
            }
            return res;
        }

        public void Clear()
        {
            User = null;
        }
    }
}
