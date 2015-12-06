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
    public float MaxStrafeSpeed;
    public float Deceleration;
    public Vector3 LookAt;
    public float MouseSpeedHorizontal;
    public float MouseSpeedVertical;
    public Transform LookAtTarget;
    public Transform HeadPosition;
    public float MaxTilt;
    public float MinTilt;
    public Quaternion turnRotation;
    public bool Grounded;
    public bool CanMove = true;
    public Transform HeadTransform;
    public GameObject TargetPrefab;
    public LineRenderer TargetLine;
    public float Power;
    public float MaxPower;

    private Rigidbody RB;
    private Animator MyAnimator;
    private IKController MyIKController;

    // Use this for initialization
    void Awake()
    {
        RB = GetComponent<Rigidbody>();
        MyAnimator = GetComponentInChildren<Animator>();
        MyIKController = GetComponentInChildren<IKController>();
        LookAt = transform.forward;
        GameObject target = Instantiate(TargetPrefab);
        TargetLine = target.GetComponentInChildren<LineRenderer>();
        target.transform.position = transform.position + transform.forward * 10f;
        target.transform.rotation = transform.rotation;
        LookAtTarget = target.transform;
        IKController ikController = GetComponentInChildren<IKController>();
        ikController.TargetObj = target.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!CanMove)
        {
            return;
        }
        Vector3 touchRayOrigin = transform.position + transform.up * 0.2f;
        Vector3 touchRayDirection = -transform.up;
        float touchRayRange = 0.85f;
        bool touchGround = Physics.Raycast(touchRayOrigin, touchRayDirection, touchRayRange);
        Debug.DrawRay(touchRayOrigin, touchRayDirection * touchRayRange, Color.red);

        // Attack IK
        MyIKController.AimLeft = Input.GetButton("Fire1");
        MyIKController.AimRight = Input.GetButton("Fire2");

        //// Animation Parameters
        if (Grounded)
        {
            if (!touchGround)
            {
                MyAnimator.SetTrigger("Fall");
            }
            RB.drag = 2.5f;
        }
        else
        {
            RB.drag = 0f;
        }
        Grounded = touchGround;
        MyAnimator.SetBool("Grounded", Grounded);


        // Forward
        Vector3 relativeVel = transform.InverseTransformDirection(RB.velocity);
        if (relativeVel.z == 0f)
        {
            MyAnimator.SetFloat("Forward", 0);
        }
        else if (relativeVel.z > 0)
        {
            MyAnimator.SetFloat("Forward", relativeVel.z / MaxSpeed);
        }
        else if (relativeVel.z < 0)
        {
            MyAnimator.SetFloat("Forward", relativeVel.z / MaxBackSpeed);
        }
        // Strafe
        MyAnimator.SetFloat("Strafe", relativeVel.x / MaxStrafeSpeed);

        // Turn
        Vector3 projLookAt = MathUtils.ProjectVectorOnPlane(Vector3.up, LookAt);
        float angle = MathUtils.SignedVectorAngle(transform.forward, projLookAt, transform.up);
        MyAnimator.SetFloat("Turn", Mathf.Clamp(angle / 90, -1f, 1f));

        // AnimationSpeed
        MyAnimator.SetFloat("Speed", 0.8f + RB.velocity.magnitude * 0.2f);
    }

    void FixedUpdate()
    {
        if (!CanMove)
        {
            return;
        }
        if (Grounded)
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
            if (Mathf.Abs(newRelativeVelocity.x) > MaxStrafeSpeed)
            {
                newRelativeVelocity.x = MaxStrafeSpeed * Mathf.Sign(newRelativeVelocity.x);
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

            Vector3 newVel = transform.TransformDirection(newRelativeVelocity);
            RB.AddForce(newVel - originalVel, ForceMode.VelocityChange);
        }

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
            LookAtTarget.position = hit.point + hit.normal * 0.01f;
            LookAtTarget.rotation = Quaternion.LookRotation(hit.normal);
            float distance = (hit.point - HeadPosition.position).magnitude * 0.1f;
            LookAtTarget.localScale = new Vector3(distance, distance, 1f);
            TargetLine.SetPosition(0, HeadPosition.position + Vector3.up * 0.1f);
            TargetLine.SetPosition(1, hit.point);
        }

        //  Turn Rotation
        Vector3 turn = LookAt - (Vector3.Dot(LookAt, Vector3.up) * Vector3.up);
        Quaternion turnRotation = Quaternion.LookRotation(turn, Vector3.up);
        RB.MoveRotation(Quaternion.Slerp(transform.rotation, turnRotation, Time.fixedDeltaTime * TurnSpeed));
    }
}
