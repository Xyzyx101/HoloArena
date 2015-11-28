using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float ForwardForce;
    public float SidewayForce;
    public float TurnSpeed;
    public float MaxSpeed;
    public float SqrMaxSpeed;
    public Vector3 LookAt;
    public float MouseSpeedHorizontal;
    public float MouseSpeedVertical;
    public float ResponseModifier;
    public Transform LookAtTarget;
    public Transform HeadPosition;
    public float MaxTilt;
    public float MinTilt;

    public Quaternion turnRotation;

    private Rigidbody RB;

    // Use this for initialization
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        SqrMaxSpeed = MaxSpeed * MaxSpeed;
        LookAt = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        // Calculate movement force
        Vector3 MoveForce = Vector3.zero;
        MoveForce += Input.GetAxis("Vertical") * ForwardForce * transform.forward * Time.fixedDeltaTime;
        MoveForce += Input.GetAxis("Horizontal") * SidewayForce * transform.right * Time.fixedDeltaTime;

        // Limit top speed;
        Vector3 vel = RB.velocity;
        Vector2 xzVel = new Vector2(vel.x, vel.z);
        if (xzVel.sqrMagnitude > SqrMaxSpeed)
        {
            xzVel = xzVel.normalized * MaxSpeed;
            RB.velocity = new Vector3(xzVel.x, vel.y, xzVel.y);
        }

        // Apply Movement force
        if (xzVel.sqrMagnitude < ResponseModifier * ResponseModifier)
        {
            MoveForce = MoveForce + MoveForce * (ResponseModifier - xzVel.magnitude);
        }
        RB.AddForce(MoveForce, ForceMode.Force);

        // Look Rotation
        float mouseYAngle = Input.GetAxis("Mouse X") * MouseSpeedHorizontal;
        float mouseXAngle = Input.GetAxis("Mouse Y") * -MouseSpeedVertical;
        LookAt = Quaternion.AngleAxis(mouseYAngle, Vector3.up) * LookAt;
        float tiltAngle = MathUtils.SignedVectorAngle(transform.forward, LookAt, transform.right);
        if (tiltAngle > MaxTilt)
        {
            mouseXAngle = -1f;
        }
        else if (tiltAngle < MinTilt)
        {
            mouseXAngle = 1f;
        }
        LookAt = Quaternion.AngleAxis(mouseXAngle, transform.right) * LookAt;
        RaycastHit hit;
        if (Physics.Raycast(HeadPosition.position, LookAt, out hit))
        {
            LookAtTarget.position = hit.point;
        }

        //  Turn Rotation
        Vector3 turn = LookAt - (Vector3.Dot(LookAt, Vector3.up) * Vector3.up);
        Quaternion turnRotation = Quaternion.LookRotation(turn, Vector3.up);
        RB.MoveRotation(Quaternion.Slerp(transform.rotation, turnRotation, Time.fixedDeltaTime * TurnSpeed));
    }
}
