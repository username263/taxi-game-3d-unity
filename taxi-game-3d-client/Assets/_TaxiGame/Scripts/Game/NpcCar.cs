using PathCreation;
using UnityEngine;

public class NpcCar : MonoBehaviour
{
    VertexPath path;
    Rigidbody rb;

    public float Movement
    {
        get;
        private set;
    }
    public bool IsArrive => Movement >= path.length;

    public void SetPath(VertexPath path)
    {
        this.path = path;
        Movement = 0;

        if (rb == null)
            rb = GetComponent<Rigidbody>();
        rb.position = path.GetPoint(0);
        rb.rotation = path.GetRotation(0f, EndOfPathInstruction.Stop);
    }

    public void UpdateMoving(float amount)
    {
        Movement += amount;
        if (rb == null)
            return;
        rb.MovePosition(path.GetPointAtDistance(Movement, EndOfPathInstruction.Stop));
        rb.MoveRotation(path.GetRotationAtDistance(Movement, EndOfPathInstruction.Stop));
    }
}
