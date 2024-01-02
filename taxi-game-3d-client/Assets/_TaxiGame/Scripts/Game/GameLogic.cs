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

        long coin;

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

        public PlayerCar PlayerCar
        {
            get;
            private set;
        }

        public int CustomerCount => customerTriggers.Length / 2;

        public event EventHandler GamePlayedEvent;
        public event EventHandler<int> CustomerTakeInEvent;
        public event EventHandler<int> CustomerTakeOutEvent;
        public event EventHandler<bool> GameEndedEvent;

        void Awake()
        {
            Instance = this;
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
            if (PlayerCar == null)
                return;
            if (isAccelPressing)
                PlayerCar.PressAccel();
            else
                PlayerCar.PressBrake();
        }

        public void PlayGame()
        {
            PlayerCar.PlayMoving();
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
            if (PlayerCar != null && PlayerCar.IsEnableMoving)
                isAccelPressing = context.ReadValue<float>() != 0f;
        }

        void OnCarEnterTrigger(CustomerTrigger trigger)
        {
            if (customerManager.WasCustomerTaken)
                StartCoroutine(TakeOut(trigger));
            else
                StartCoroutine(TakeIn(trigger));
        }

        /// <summary> �մ� ž�� </summary>
        IEnumerator TakeIn(CustomerTrigger trigger)
        {
            PlayerCar.StopMoving();
            
            yield return StartCoroutine(
                customerManager.TakeIn(trigger.CustomerPoint, PlayerCar)
            );
            customerTakePos = PlayerCar.Movement;
            ++numOfCustomers;
            CustomerTakeInEvent?.Invoke(this, numOfCustomers);
            
            PlayerCar.PlayMoving();
        }

        /// <summary> �մ� ���� </summary>
        IEnumerator TakeOut(CustomerTrigger trigger)
        {
            PlayerCar.StopMoving();

            var distance = PlayerCar.Movement - customerTakePos;
            var reward = Mathf.FloorToInt(
                distance * ((70f + StageIndex) / 100f)
            );

            if (reward > 0)
                coin += reward;
            CustomerTakeOutEvent?.Invoke(this, numOfCustomers);
            yield return StartCoroutine(
                customerManager.TakeOut(trigger.CustomerPoint, PlayerCar)
            );
            customerTakePos = 0f;

            PlayerCar.PlayMoving();
        }

        async void EndGame(bool isGoal)
        {
            PlayerCar.StopMoving();
            npcCarManager.Stop();
            GameEndedEvent?.Invoke(this, isGoal);

            var dt = DateTime.Now;
            
            if (isGoal)
                await ClientManager.Instance.UserService.EndStage(StageIndex, coin);
            
            var ts = DateTime.Now - dt;
            if (ts.TotalSeconds < 3)
                await UniTask.WaitForSeconds(3.0f - (float)ts.TotalSeconds, true);

            LoadStage(ClientManager.Instance.UserService.User.CurrentStage.Index);
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
            var template = ClientManager.Instance.TemplateService.StageTemplates[index];
            SceneManager.LoadScene(template.SceneName);
        }
    }
}