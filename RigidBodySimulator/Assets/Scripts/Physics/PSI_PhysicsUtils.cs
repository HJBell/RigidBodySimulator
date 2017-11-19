using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PSI_PhysicsUtils {

    //---------------------------------Checking For Collisions----------------------------------

    public static bool SphereSphereCollisionOccured(PSI_Collider_Sphere col1, PSI_Collider_Sphere col2, out Vector3 point)
    {
        point = col1.pPosition + (col2.pPosition - col1.pPosition) / 2f;
        return Vector3.Distance(col1.pPosition, col2.pPosition) <= Mathf.Abs(col1.pRadius) + Mathf.Abs(col2.pRadius);
    }

    public static bool SpherePlaneCollisionOccured(PSI_Collider_Sphere sphereCol, PSI_Collider_Plane planeCol, out Vector3 point)
    {
        float distToProjectedPoint = Vector3.Dot(planeCol.pNormal, (sphereCol.pPosition - planeCol.pPosition));
        point = sphereCol.pPosition - distToProjectedPoint * planeCol.pNormal;

        // Generate 4 triangles between the corners of the plane and the projected point.
        var planeVerts = planeCol.GetPlaneVertices();
        var triangles = new Vector3[4, 3];
        for (int i = 0; i < 4; i++)
        {
            triangles[i, 0] = point;
            triangles[i, 1] = planeVerts[i];
            triangles[i, 2] = planeVerts[(i == 3) ? 0 : i + 1];
        }

        // Sum the area of the traingles.
        float totalTriArea = 0.0f;
        for (int i = 0; i < 4; i++)
        {
            float a = Vector3.Distance(triangles[i, 0], triangles[i, 1]);
            float b = Vector3.Distance(triangles[i, 1], triangles[i, 2]);
            float c = Vector3.Distance(triangles[i, 2], triangles[i, 0]);
            float s = (a + b + c) / 2;
            totalTriArea += Mathf.Sqrt(s * (s - a) * (s - b) * (s - c));
        }

        // Check if the sum area is equal to the area of the plane. 
        // If so then the projected point is within the bounds of the plane.
        if (Mathf.Abs(totalTriArea - planeCol.pArea) > sphereCol.pRadius) return false;

        // Check if the sphere is intersecting with the plane.
        return (Mathf.Abs(distToProjectedPoint) <= sphereCol.pRadius);
    }
}
