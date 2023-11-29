using Newtonsoft.Json;
using PathCreation;
using System.Collections;
using TMPro;
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
        [SerializeField]
        TMP_Text stateText;

        InputControls inputControls;
        bool isAccelPressing = false;

        float customerTakePos;

        long coin;
        int stageIndex;


        public static GameLogic Instance
        {
            get;
            private set;
        }

        [field: SerializeField]
        public PlayerCar PlayerCar
        {
            get;
            private set;
        }

        void Awake()
        {
            Instance = this;

            if (TemplateManager.Instance == null)
                new GameObject("TemplateManager", typeof(TemplateManager));

            foreach (var carTemp in TemplateManager.Instance.Cars)
                Debug.Log(JsonConvert.SerializeObject(carTemp));
            foreach (var stageTemp in TemplateManager.Instance.Stages)
                Debug.Log(JsonConvert.SerializeObject(stageTemp));
        }

        IEnumerator Start()
        {
            npcCarManager.Play();

            foreach (var trigger in customerTriggers)
            {
                trigger.OnPlayerEntered += (sender, args) =>
                {
                    OnCarEnterTrigger(sender as CustomerTrigger);
                };
            }    

            PlayerCar.SetPath(playerPath.path);
            PlayerCar.OnCrashed += (sender, args) =>
            {
                StartCoroutine(EndGame(false));
            };
            PlayerCar.OnArrive += (sender, args) =>
            {
                StartCoroutine(EndGame(true));
            };
            yield return new WaitForSeconds(1);
            PlayerCar.PlayMoving();
        }

        void OnEnable()
        {
            if (inputControls == null)
                inputControls = new();
            inputControls.Player.SetCallbacks(this);
            inputControls.Player.Enable();
        }

        void OnDisable()
        {
            inputControls?.Player.Disable();
        }

        void Update()
        {
            stateText.text = coin.ToString();

            if (isAccelPressing)
                PlayerCar.PressAccel();
            else
                PlayerCar.PressBrake();
        }

        public void OnAccelerate(InputAction.CallbackContext context)
        {
            isAccelPressing = context.ReadValue<float>() != 0f;
        }

        void OnCarEnterTrigger(CustomerTrigger trigger)
        {
            if (customerManager.WasCustomerTaken)
                StartCoroutine(TakeOut(trigger));
            else
                StartCoroutine(TakeIn(trigger));
        }

        /// <summary> ¼Õ´Ô Å¾½Â </summary>
        IEnumerator TakeIn(CustomerTrigger trigger)
        {
            PlayerCar.StopMoving();
            yield return StartCoroutine(
                customerManager.TakeIn(trigger.CustomerPoint, PlayerCar)
            );
            customerTakePos = PlayerCar.Movement;
            PlayerCar.PlayMoving();
        }

        /// <summary> ¼Õ´Ô ÇÏÂ÷ </summary>
        IEnumerator TakeOut(CustomerTrigger trigger)
        {
            PlayerCar.StopMoving();

            var distance = PlayerCar.Movement - customerTakePos;
            var reward = Mathf.FloorToInt(
                distance * ((70f + stageIndex) / 100f)
            );
            Debug.Log($"Distance: {distance}, Reward: {reward}");
            if (reward > 0)
                coin += reward;

            yield return StartCoroutine(
                customerManager.TakeOut(trigger.CustomerPoint, PlayerCar)
            );
        
            customerTakePos = 0f;
            PlayerCar.PlayMoving();
        }

        IEnumerator EndGame(bool isGoal)
        {
            PlayerCar.StopMoving();
            npcCarManager.Stop();
            yield return new WaitForSeconds(3);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
