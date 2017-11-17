using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PSI_PhysicsUtils {

    //---------------------------------Checking For Collisions----------------------------------

    public static bool SphereSphereCollisionOccured(PSI_Collider_Sphere col1, PSI_Collider_Sphere col2)
    {
        return Vector3.Distance(col1.pPosition, col2.pPosition) <= Mathf.Abs(col1.pRadius) + Mathf.Abs(col2.pRadius);
    }
}
