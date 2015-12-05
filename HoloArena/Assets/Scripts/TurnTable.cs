using UnityEngine;
using System.Collections;

public class TurnTable : MonoBehaviour
{
    public float TurnSpeed = 2f;
    private float TurnTimer;
    private float TurnTargetAngle;
   
    void Update()
    {
        Quaternion TargetRot = Quaternion.AngleAxis(TurnTargetAngle, Vector3.up);
        if (TargetRot != transform.rotation)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, TargetRot, Time.deltaTime * TurnSpeed);
        }
    }

    public void TurnToSlot(int slot)
    {
        switch (slot)
        {
            case 0:
                TurnTargetAngle = 0f;
                break;
            case 1:
                TurnTargetAngle = 30f;
                break;
            case 2:
                TurnTargetAngle = 60f;
                break;
            default:
                Debug.LogError("Unknown turntable slot");
                break;
        }
    }
}
