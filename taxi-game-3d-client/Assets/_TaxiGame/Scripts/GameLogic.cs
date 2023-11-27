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
        CustomerTrigger[] customerTriggers;
        [SerializeField]
        TMP_Text stateText;

        InputControls inputControls;
        bool isAccelPressing = false;

        bool wasCustomerTaken;
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
            if (wasCustomerTaken)
                StartCoroutine(TakeOut());
            else
                StartCoroutine(TakeIn());
        }

        /// <summary> ¼Õ´Ô Å¾½Â </summary>
        IEnumerator TakeIn()
        {
            PlayerCar.StopMoving();
            yield return new WaitForSeconds(3f);
            wasCustomerTaken = true;
            customerTakePos = PlayerCar.Movement;
            PlayerCar.PlayMoving();
        }

        /// <summary> ¼Õ´Ô ÇÏÂ÷ </summary>
        IEnumerator TakeOut()
        {
            PlayerCar.StopMoving();

            var distance = PlayerCar.Movement - customerTakePos;
            var reward = Mathf.FloorToInt(
                distance * ((70f + stageIndex) / 100f)
            );
            Debug.Log($"Distance: {distance}, Reward: {reward}");
            if (reward > 0)
                coin += reward;

            yield return new WaitForSeconds(3f);

            wasCustomerTaken = false;            
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
