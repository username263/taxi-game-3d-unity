using PathCreation;
using UnityEngine;

public class NpcCar : MonoBehaviour
{
    VertexPath path;
    float movement;
    Rigidbody rb;

    public bool IsArrive => movement >= path.length;

    public void SetPath(VertexPath path)
    {
        this.path = path;
        movement = 0;

        if (rb == null)
            rb = GetComponent<Rigidbody>();
        rb.position = path.GetPoint(0);
        rb.rotation = path.GetRotation(0f, EndOfPathInstruction.Stop);
    }

    public void UpdateMoving(float amount)
    {
        movement += amount;
        if (rb == null)
            return;
        rb.MovePosition(path.GetPointAtDistance(movement, EndOfPathInstruction.Stop));
        rb.MoveRotation(path.GetRotationAtDistance(movement, EndOfPathInstruction.Stop));
    }
}
