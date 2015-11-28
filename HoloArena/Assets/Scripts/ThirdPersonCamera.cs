using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform Player;
    public Transform PlayerHead;
    public Transform PlayerLookAtTarget;
    public Vector3 Offset;
    public float PositionLerpSpeed;
    public float RotationLerpSpeed;

    private PlayerController MyPlayerController;

    void Start()
    {
        MyPlayerController = Player.gameObject.GetComponent<PlayerController>();
    }

    void LateUpdate()
    {
        Vector3 playerLookDir = (PlayerLookAtTarget.position - PlayerHead.position).normalized;
        Vector3 cameraDir = Quaternion.AngleAxis(180f, Player.up) * playerLookDir;
        Quaternion cameraPosRot = Quaternion.LookRotation(MathUtils.ProjectVectorOnPlane(Player.up, cameraDir), Player.up);
        float playerLookAngle = MathUtils.SignedVectorAngle(Player.forward, playerLookDir, Player.right);
        Quaternion cameraPosTilt = Quaternion.AngleAxis(playerLookAngle * 0.5f, Player.right);
        //cameraPosTilt = Quaternion.identity;
        Vector3 desiredCameraPos = cameraPosTilt * cameraPosRot * Offset;
        desiredCameraPos += PlayerHead.position;
        transform.position = Vector3.Lerp(transform.position, desiredCameraPos, Time.deltaTime * PositionLerpSpeed);

        Quaternion desiredCameraRot = Quaternion.LookRotation(MyPlayerController.LookAt, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredCameraRot, Time.deltaTime * RotationLerpSpeed);
    }
}
