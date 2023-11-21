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
        PathCreator path;
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
        public Car PlayerCar
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
            PlayerCar.SetPath(path.path);
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

        /// <summary> ¼Õ´Ô Å¾½Â </summary>
        IEnumerator TakeIn()
        {
            PlayerCar.StopMoving();
            yield return new WaitForSeconds(3f);
            wasCustomerTaken = true;
            PlayerCar.PlayMoving();
        }

        /// <summary> ¼Õ´Ô ÇÏÂ÷ </summary>
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
