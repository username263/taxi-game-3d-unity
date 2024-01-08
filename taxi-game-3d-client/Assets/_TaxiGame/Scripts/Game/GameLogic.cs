using Cysharp.Threading.Tasks;
using PathCreation;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace TaxiGame3D
{
    public class GameLogic : MonoBehaviour, InputControls.IPlayerActions
    {
        [SerializeField]
        PathCreator playerPath;
        [SerializeField]
        NpcCarManager npcCarManager;
        [SerializeField]
        CustomerManager customerManager;
        [SerializeField]
        CustomerTrigger[] customerTriggers;

        InputControls inputControls;
        bool isAccelPressing = false;

        int numOfCustomers = 0;
        float customerTakePos;

        public static GameLogic Instance
        {
            get;
            private set;
        }

        public static int StageIndex
        {
            get;
            private set;
        }

        public bool IsPlaying
        {
            get;
            private set;
        }

        public PlayerCar PlayerCar
        {
            get;
            private set;
        }

        public int CustomerCount => customerTriggers.Length / 2;

        public int RewardedCoin
        {
            get;
            private set;
        }

        public event EventHandler GamePlayedEvent;
        public event EventHandler<int> CustomerTakeInEvent;
        public event EventHandler<int> CustomerTakeOutEvent;
        public event EventHandler<bool> GameEndedEvent;

        void Awake()
        {
            Instance = this;
            IsPlaying = false;
        }

        void Start()
        {
            GameUI.CreateInstance();

            npcCarManager.Play();

            foreach (var trigger in customerTriggers)
            {
                trigger.OnPlayerEntered += (sender, args) =>
                {
                    OnCarEnterTrigger(sender as CustomerTrigger);
                };
            }

            RespawnPlayerCar();

            GameUI.Instance.OnGameLoaded();
        }

        void OnEnable()
        {
            if (inputControls == null)
                inputControls = new();
            inputControls.Player.SetCallbacks(this);
            inputControls.Player.Enable();

            ClientManager.Instance.AuthService.TokenExpired += OnTokenExpired;
            ClientManager.Instance.UserService.UserUpdateFailed += OnUserUpdateFailed;
        }

        void OnDisable()
        {
            inputControls?.Player.Disable();

            ClientManager.Instance.AuthService.TokenExpired -= OnTokenExpired;
            ClientManager.Instance.UserService.UserUpdateFailed -= OnUserUpdateFailed;

            GamePlayedEvent = null;
            CustomerTakeInEvent = null;
            CustomerTakeOutEvent = null;
            GameEndedEvent = null;
        }

        void Update()
        {
            if (!IsPlaying || PlayerCar == null)
                return;
            if (isAccelPressing)
                PlayerCar.PressAccel();
            else
                PlayerCar.PressBrake();
        }

        public void PlayGame()
        {
            IsPlaying = true;
            GamePlayedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void RespawnPlayerCar()
        {
            if (PlayerCar != null)
            {
                Destroy(PlayerCar.gameObject);
                PlayerCar = null;
            }

            var carPrefab = ClientManager.Instance.UserService.User.CurrentCar.Prefab;
            if (carPrefab == null)
            {
                Debug.LogError("Player car prefab is null.");
                return;
            }

            PlayerCar = Instantiate(carPrefab)?.GetComponent<PlayerCar>();
            PlayerCar.SetPath(playerPath.path);
            PlayerCar.OnCrashed += (sender, args) =>
            {
                EndGame(false);
            };
            PlayerCar.OnArrive += (sender, args) =>
            {
                EndGame(true);
            };
        }

        public void OnAccelerate(InputAction.CallbackContext context)
        {
            if (!IsPlaying || PlayerCar == null)
                return;
            isAccelPressing = context.ReadValue<float>() != 0f;
            if (isAccelPressing && !PlayerCar.IsEnableMoving)
                PlayerCar.PlayMoving();
        }

        void OnCarEnterTrigger(CustomerTrigger trigger)
        {
            if (customerManager.WasCustomerTaken)
                StartCoroutine(TakeOut(trigger));
            else
                StartCoroutine(TakeIn(trigger));
        }

        /// <summary> 손님 탑승 </summary>
        IEnumerator TakeIn(CustomerTrigger trigger)
        {
            isAccelPressing = false;
            PlayerCar.StopMoving();
            
            yield return StartCoroutine(
                customerManager.TakeIn(trigger.CustomerPoint, PlayerCar)
            );
            customerTakePos = PlayerCar.Movement;
            ++numOfCustomers;
            CustomerTakeInEvent?.Invoke(this, numOfCustomers);
            
            PlayerCar.PlayMoving();
        }

        /// <summary> 손님 하차 </summary>
        IEnumerator TakeOut(CustomerTrigger trigger)
        {
            isAccelPressing = false;
            PlayerCar.StopMoving();

            var distance = PlayerCar.Movement - customerTakePos;
            var reward = Mathf.FloorToInt(
                distance * ((70f + StageIndex) / 100f)
            );

            if (reward > 0)
                RewardedCoin += reward;
            CustomerTakeOutEvent?.Invoke(this, numOfCustomers);
            yield return StartCoroutine(
                customerManager.TakeOut(trigger.CustomerPoint, PlayerCar)
            );
            customerTakePos = 0f;

            PlayerCar.PlayMoving();
        }

        async void EndGame(bool isGoal)
        {
            IsPlaying = false;
            PlayerCar.StopMoving();
            npcCarManager.Stop();
            
            await ClientManager.Instance.UserService.EndStage(StageIndex, isGoal, RewardedCoin);

            GameEndedEvent?.Invoke(this, isGoal);
        }

        void OnTokenExpired(object sender, EventArgs args)
        {
            Debug.LogError("Token was expired.");
            SceneManager.LoadScene(0);
        }

        void OnUserUpdateFailed(object sender, EventArgs args)
        {
            Debug.LogError("User updating was failed.");
            SceneManager.LoadScene(0);
        }

        [ContextMenu("Print PlayerPath Distance")]
        void PrintPlayerPathDistance()
        {
            Debug.Log(playerPath.path.length);
        }

        public static void LoadStage(int index)
        {
            StageIndex = index;
            var template = ClientManager.Instance.TemplateService.Stages[index];
            SceneManager.LoadScene(template.SceneName);
        }
    }
}
