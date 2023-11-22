using PathCreation;
using UnityEngine;

public class NpcCar : MonoBehaviour
{
    VertexPath path;
    float movement;
    Rigidbody rb;

    public bool IsArrive => movement >= path.length;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetPath(VertexPath path)
    {
        this.path = path;
        movement = 0;
        rb.position = path.GetPoint(0);
        rb.rotation = path.GetRotation(0f, EndOfPathInstruction.Stop);
    }

    public void UpdateMoving(float amount)
    {
        movement += amount;
        rb.MovePosition(path.GetPointAtDistance(movement, EndOfPathInstruction.Stop));
        rb.MoveRotation(path.GetRotationAtDistance(movement, EndOfPathInstruction.Stop));
    }
}
