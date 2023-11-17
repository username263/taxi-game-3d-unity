using PathCreation;
using UnityEngine;

namespace TexiGame3D
{
    public class Car : MonoBehaviour
    {
        [SerializeField]
        PathCreator pathCreator;
        float movement = 0f;
        float speed = 5f;

        void Start()
        {
            transform.position = pathCreator.path.GetPointAtDistance(movement, EndOfPathInstruction.Stop);
        }

        void Update()
        {
            movement += Time.deltaTime * speed;
            transform.position = pathCreator.path.GetPointAtDistance(movement, EndOfPathInstruction.Stop);
            transform.rotation = pathCreator.path.GetRotationAtDistance(movement, EndOfPathInstruction.Stop);
        }
    }
}