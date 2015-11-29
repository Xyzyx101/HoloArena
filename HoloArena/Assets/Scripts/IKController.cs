using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]

public class IKController : MonoBehaviour
{

    protected Animator MyAnimator;

    public bool IKActive = false;
    public bool AimRight = false;
    public bool AimLeft = false;
    public Transform TargetObj = null;
    public float AimSpeed = 1f;
    private float AimRightTimer;
    private float AimLeftTimer;

    void Start()
    {
        MyAnimator = GetComponent<Animator>();
    }

    void Update()
    {

    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (MyAnimator)
        {
            //if the IK is active, set the position and rotation directly to the goal. 
            if (IKActive)
            {

                // Set the look target position, if one has been assigned
                if (TargetObj != null)
                {
                    MyAnimator.SetLookAtWeight(1);
                    MyAnimator.SetLookAtPosition(TargetObj.position);
                }

                // Set the right hand target position and rotation, if one has been assigned
                if (AimRight)
                {
                    AimRightTimer = Mathf.Clamp(AimRightTimer + Time.deltaTime, 0f, AimSpeed);
                    MyAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, AimRightTimer / AimSpeed);
                    MyAnimator.SetIKPosition(AvatarIKGoal.RightHand, TargetObj.position);
                }
                else
                {
                    AimRightTimer = Mathf.Clamp(AimRightTimer - Time.deltaTime * 0.35f, 0f, AimSpeed);
                    MyAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, AimRightTimer / AimSpeed);
                    MyAnimator.SetIKPosition(AvatarIKGoal.RightHand, TargetObj.position);
                }

                if (AimLeft)
                {
                    AimLeftTimer = Mathf.Clamp(AimLeftTimer + Time.deltaTime, 0f, AimSpeed);
                    MyAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, AimLeftTimer / AimSpeed);
                    MyAnimator.SetIKPosition(AvatarIKGoal.LeftHand, TargetObj.position);
                }
                else
                {
                    AimLeftTimer = Mathf.Clamp(AimLeftTimer - Time.deltaTime * 0.35f, 0f, AimSpeed);
                    MyAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, AimLeftTimer / AimSpeed);
                    MyAnimator.SetIKPosition(AvatarIKGoal.LeftHand, TargetObj.position);
                }

            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else
            {
                MyAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                MyAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                MyAnimator.SetLookAtWeight(0);
            }
        }
    }
}