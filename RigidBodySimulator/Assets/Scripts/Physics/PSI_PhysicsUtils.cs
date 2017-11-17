using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PSI_PhysicsUtils {

    //---------------------------------Checking For Collisions----------------------------------

    public static bool SphereSphereCollisionOccured(PSI_Collider_Sphere col1, PSI_Collider_Sphere col2, out Vector3 point)
    {
        bool collision = Vector3.Distance(col1.pPosition, col2.pPosition) <= Mathf.Abs(col1.pRadius) + Mathf.Abs(col2.pRadius);
        point = collision ? col1.pPosition + (col2.pPosition - col1.pPosition) / 2f : Vector3.zero;
        return collision;
    }
}
