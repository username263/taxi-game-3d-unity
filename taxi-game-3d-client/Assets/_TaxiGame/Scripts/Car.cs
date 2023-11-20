using PathCreation;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TaxiGame3D
{
    public class Car : MonoBehaviour, InputControls.IPlayerActions
    {
        [SerializeField]
        PathCreator pathCreator;
        
        /// <summary>
        /// 가속도
        /// </summary>
        [SerializeField]
        float acceleration = 1f;
        /// <summary>
        /// 최고 속도
        /// </summary>
        [SerializeField]
        float maxSpeed = 5f;
        /// <summary>
        /// 제동력
        /// </summary>
        [SerializeField]
        float brakeForce = 1f;

        bool isPressing = false;
        float speed = 1f;
        float movement = 0f;

        InputControls inputControls;


        void Start()
        {
            transform.position = pathCreator.path.GetPointAtDistance(movement, EndOfPathInstruction.Stop);
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
            var delta = Time.deltaTime;
            
            if (isPressing)
                speed = Mathf.Min(speed + delta * acceleration, maxSpeed);
            else
                speed = Mathf.Max(speed - delta * brakeForce, 1f);

            movement += delta * speed;
            transform.position = pathCreator.path.GetPointAtDistance(movement, EndOfPathInstruction.Stop);
            transform.rotation = pathCreator.path.GetRotationAtDistance(movement, EndOfPathInstruction.Stop);
        }

        public void OnAccelerate(InputAction.CallbackContext context)
        {
            isPressing = context.ReadValue<float>() != 0f;
        }
    }
}