using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float ForwardForce;
    public float SidewayForce;
    public float TurnSpeed;
    public float ForwardAccel;
    public float SidewaysAccel;
    public float MaxSpeed;
    public float MaxBackSpeed;
    public float MaxSidewaysSpeed;
    public float Deceleration;
    public Vector3 LookAt;
    public float MouseSpeedHorizontal;
    public float MouseSpeedVertical;
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
        LookAt = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        float forward = Input.GetAxis("Vertical");
        float sideways = Input.GetAxis("Horizontal");
        Vector2 moveAccel = Vector2.zero;
        if (forward > 0)
        {
            moveAccel.y = forward * ForwardAccel;
        }
        else
        {
            moveAccel.y = forward * ForwardAccel;
        }
        moveAccel.x = sideways * SidewaysAccel;

        Vector3 originalVel = RB.velocity;
        Vector3 originalRelativeVel = transform.InverseTransformDirection(originalVel);
        Vector3 newRelativeVelocity = originalRelativeVel;
        newRelativeVelocity.z += moveAccel.y * Time.fixedDeltaTime;
        newRelativeVelocity.x += moveAccel.x * Time.fixedDeltaTime;

        // Limit forward and back
        if (newRelativeVelocity.z > MaxSpeed)
        {
            newRelativeVelocity.z = MaxSpeed;
        }
        else if (newRelativeVelocity.z < -MaxBackSpeed)
        {
            newRelativeVelocity.z = -MaxBackSpeed;
        }
        // Limit Sideways
        if (Mathf.Abs(newRelativeVelocity.x) > MaxSidewaysSpeed)
        {
            newRelativeVelocity.x = MaxSidewaysSpeed * Mathf.Sign(newRelativeVelocity.x);
        }
        // Limit moving on angles
        Vector2 horizontalVelocity = new Vector2(newRelativeVelocity.x, newRelativeVelocity.z);
        if (Vector2.SqrMagnitude(horizontalVelocity) > MaxSpeed * MaxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * MaxSpeed;
            newRelativeVelocity.x = horizontalVelocity.x;
            newRelativeVelocity.z = horizontalVelocity.y;
        }
        // Decelerate forward
        if (forward < 0.7f && forward >= 0f)
        {
            newRelativeVelocity.z -= (1 - forward) * Deceleration * Time.fixedDeltaTime;
            if (newRelativeVelocity.z < 0f)
            {
                newRelativeVelocity.z = 0f;
            }
        }
        else if (forward > -0.7f && forward < 0f)
        {
            newRelativeVelocity.z += (1 - forward) * Deceleration * Time.fixedDeltaTime;
            if (newRelativeVelocity.z > 0f)
            {
                newRelativeVelocity.z = 0f;
            }
        }
        // Decelerate sideways
        if (sideways < 0.7f && sideways >= 0f)
        {
            newRelativeVelocity.x -= (1 - sideways) * Deceleration * Time.fixedDeltaTime;
            if (newRelativeVelocity.x < 0f)
            {
                newRelativeVelocity.x = 0f;
            }
        }
        else if (sideways > -0.7f && sideways < 0f)
        {
            newRelativeVelocity.x += (1 - sideways) * Deceleration * Time.fixedDeltaTime;
            if (newRelativeVelocity.x > 0f)
            {
                newRelativeVelocity.x = 0f;
            }
        }

        Debug.Log(newRelativeVelocity);
        Vector3 newVel = transform.TransformDirection(newRelativeVelocity);
        RB.AddForce(newVel - originalVel, ForceMode.VelocityChange);

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
