using UnityEngine;
using System.Collections;

public class MathUtils
{
    // Returns the angle in degrees between reference vector and other vector around the axis normal
    public static float SignedVectorAngle(Vector3 referenceVector, Vector3 otherVector, Vector3 normal)
    {
        Vector3 perpVector;
        float angle;

        perpVector = Vector3.Cross(normal, referenceVector);
        angle = Vector3.Angle(referenceVector, otherVector);
        angle *= Mathf.Sign(Vector3.Dot(perpVector, otherVector));

        return angle;
    }

    public static Vector3 ProjectVectorOnPlane(Vector3 planeNormal, Vector3 vector)
    {
        return vector - (Vector3.Dot(vector, planeNormal) * planeNormal);
    }
}
