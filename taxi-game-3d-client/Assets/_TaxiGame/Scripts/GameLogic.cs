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
        TMP_Text stateText;

        InputControls inputControls;
        bool isAccelPressing = false;

        bool wasCustomerTaken;

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

            PlayerCar.SetPath(playerPath.path);
            PlayerCar.OnArrive += (sender, args) =>
            {
                StartCoroutine(EndGame());
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
            var s = $"Moving: {PlayerCar.IsEnableMoving}\n";
            s += $"Customer: {wasCustomerTaken}";
            stateText.text = s;

            if (isAccelPressing)
                PlayerCar.PressAccel();
            else
                PlayerCar.PressBrake();
        }

        public void OnAccelerate(InputAction.CallbackContext context)
        {
            isAccelPressing = context.ReadValue<float>() != 0f;
        }

        public void OnCarEnterTrigger(CustomerTrigger trigger)
        {
            if (wasCustomerTaken)
                StartCoroutine(TakeOut());
            else
                StartCoroutine(TakeIn());
        }

        /// <summary> �մ� ž�� </summary>
        IEnumerator TakeIn()
        {
            PlayerCar.StopMoving();
            yield return new WaitForSeconds(3f);
            wasCustomerTaken = true;
            PlayerCar.PlayMoving();
        }

        /// <summary> �մ� ���� </summary>
        IEnumerator TakeOut()
        {
            PlayerCar.StopMoving();
            yield return new WaitForSeconds(3f);
            wasCustomerTaken = false;
            PlayerCar.PlayMoving();
        }

        IEnumerator EndGame()
        {
            PlayerCar.StopMoving();
            yield return new WaitForSeconds(3);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
