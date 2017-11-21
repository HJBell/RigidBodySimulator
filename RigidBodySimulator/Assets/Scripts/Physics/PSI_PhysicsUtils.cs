using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ImpulseResult
{
    public Vector3 Impulse1;
    public Vector3 Impulse2;
}

public static class PSI_PhysicsUtils {


    //----------------------------------------Public Functions---------------------------------------

    public static bool SphereSphereCollisionCheck(PSI_Collider_Sphere col1, PSI_Collider_Sphere col2, out Vector3 point)
    {
        // Determine if colliding.
        var collisionVector = col2.pPosition - col1.pPosition;
        point = col1.pPosition + collisionVector / 2f;
        bool isColliding = collisionVector.magnitude <= Mathf.Abs(col1.pRadius) + Mathf.Abs(col2.pRadius);

        if (isColliding)
        {
            // Resolve collision intersection.
            float overlap = collisionVector.magnitude - (col1.pRadius + col2.pRadius);
            ResolveCollisionOverlaps(col1, col2, collisionVector, overlap);

            // Apply impulses to the bodies.
            ApplyImpulses(col1, col2, collisionVector);

            return true;
        }
        return false;
    }

    public static bool SpherePlaneCollisionOccured(PSI_Collider_Sphere sphereCol, PSI_Collider_Plane planeCol, out Vector3 point)
    {
        float distToProjectedPoint = Vector3.Dot(planeCol.pNormal, (sphereCol.pPosition - planeCol.pPosition));
        point = sphereCol.pPosition - distToProjectedPoint * planeCol.pNormal;

        // Generate 4 triangles between the corners of the plane and the projected point.
        var planeVerts = planeCol.GetVertices();
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

        // Determine if a collision occured.
        bool isColliding = Mathf.Abs(totalTriArea - planeCol.pArea) < 0.01f && Mathf.Abs(distToProjectedPoint) <= sphereCol.pRadius;

        if (isColliding)
        {
            // Resolve collision intersection.
            var colVector = point - sphereCol.pPosition;
            var overlap = colVector.magnitude - sphereCol.pRadius;
            ResolveCollisionOverlaps(sphereCol, planeCol, colVector, overlap);
            
            // Apply impulses to bodies.
            ApplyImpulses(sphereCol, planeCol, colVector);

            return true;
        }
        return false;
    }

    public static bool SphereBoxCollisionOccured(PSI_Collider_Sphere sphereCol, PSI_Collider_Box boxCol, out Vector3 point)
    {
        point = Vector3.up * 9999;

        var boxAxes = boxCol.GetAxes();
        var boxVerts = boxCol.GetVertices();

        float nearestVertDist = float.PositiveInfinity;
        Vector3 nearestVert = Vector3.zero;
        foreach (var vert in boxVerts)
        {
            var dist = Vector3.Distance(vert, sphereCol.pPosition);
            if (dist < nearestVertDist)
            {
                nearestVertDist = dist;
                nearestVert = vert;
            }
        }

        var pointCloseToBoxVert = sphereCol.pPosition + (nearestVert - sphereCol.pPosition).normalized * sphereCol.pRadius;
        var pointCloseToBoxCentre = sphereCol.pPosition + (boxCol.pPosition - sphereCol.pPosition).normalized * sphereCol.pRadius;
        var sphereVerts = new Vector3[] { pointCloseToBoxVert, pointCloseToBoxCentre };

        if (CheckForOverlapUsingSAT(boxAxes, boxVerts, sphereVerts))
            return true;
        return false;
    }

    public static bool BoxBoxCollisionOccured(PSI_Collider_Box col1, PSI_Collider_Box col2, out Vector3 point)
    {
        point = Vector3.up * 9999;

        var col1Axes = col1.GetAxes();
        var col2Axes = col2.GetAxes();
        var axesToCheck = new Vector3[]
        {
            col1Axes[0],
            col1Axes[1],
            col1Axes[2],
            col2Axes[0],
            col2Axes[1],
            col2Axes[2],
            Vector3.Cross(col1Axes[0], col2Axes[0]),
            Vector3.Cross(col1Axes[0], col2Axes[1]),
            Vector3.Cross(col1Axes[0], col2Axes[2]),
            Vector3.Cross(col1Axes[1], col2Axes[0]),
            Vector3.Cross(col1Axes[1], col2Axes[1]),
            Vector3.Cross(col1Axes[1], col2Axes[2]),
            Vector3.Cross(col1Axes[2], col2Axes[0]),
            Vector3.Cross(col1Axes[2], col2Axes[1]),
            Vector3.Cross(col1Axes[2], col2Axes[2])
        };
        var col1Verts = col1.GetVertices();
        var col2Verts = col2.GetVertices();

        if (CheckForOverlapUsingSAT(axesToCheck, col2Verts, col1Verts)) return true;
        if (CheckForOverlapUsingSAT(axesToCheck, col1Verts, col2Verts)) return true;
        return false;
    }

    public static bool BoxPlaneCollisionOccured(PSI_Collider_Box boxCol, PSI_Collider_Plane planeCol, out Vector3 point)
    {
        point = Vector3.up * 9999;

        var boxAxes = boxCol.GetAxes();
        var boxVerts = boxCol.GetVertices();

        var projectedPointsList = new List<Vector3>();
    
        for(int i = 0; i < 8; i++)
        {
            float distToProjectedPoint = Vector3.Dot(planeCol.pNormal, (boxVerts[i] - planeCol.pPosition));
            Vector3 projectedPoint = boxVerts[i] - distToProjectedPoint * planeCol.pNormal;

            // Generate 4 triangles between the corners of the plane and the projected point.
            var planeVerts = planeCol.GetVertices();
            var triangles = new Vector3[4, 3];
            for (int j = 0; j < 4; j++)
            {
                triangles[j, 0] = projectedPoint;
                triangles[j, 1] = planeVerts[j];
                triangles[j, 2] = planeVerts[(j == 3) ? 0 : j + 1];
            }

            // Sum the area of the traingles.
            float totalTriArea = 0.0f;
            for (int j = 0; j < 4; j++)
            {
                float a = Vector3.Distance(triangles[j, 0], triangles[j, 1]);
                float b = Vector3.Distance(triangles[j, 1], triangles[j, 2]);
                float c = Vector3.Distance(triangles[j, 2], triangles[j, 0]);
                float s = (a + b + c) / 2;
                totalTriArea += Mathf.Sqrt(s * (s - a) * (s - b) * (s - c));
            }

            if (Mathf.Abs(totalTriArea - planeCol.pArea) <= 0.01f) projectedPointsList.Add(projectedPoint);
        }

        var axesToCheck = new Vector3[]
        {
            boxAxes[0],
            boxAxes[1],
            boxAxes[2],
            planeCol.transform.right,
            planeCol.transform.forward,
            planeCol.transform.up
        };

        if (CheckForOverlapUsingSAT(axesToCheck, boxVerts, projectedPointsList.ToArray())) return true;
        return false;
    }


    //----------------------------------------Private Functions--------------------------------------

    private static bool CheckForOverlapUsingSAT(Vector3[] separatingAxes, Vector3[] verts1, Vector3[] verts2)
    {
        var minOverlap = float.PositiveInfinity;

        for (int i = 0; i < separatingAxes.Length; i++)
        {
            float minDot1 = float.MaxValue;
            float maxDot1 = float.MinValue;

            float minDot2 = float.MaxValue;
            float maxDot2 = float.MinValue;

            Vector3 axis = separatingAxes[i];

            if (separatingAxes[i] == Vector3.zero) return true;

            for (int j = 0; j < verts1.Length; j++)
            {
                float dot = Vector3.Dot((verts1[j]), axis);
                if (dot < minDot2) minDot2 = dot;
                if (dot > maxDot2) maxDot2 = dot;                
            }

            for (int j = 0; j < verts2.Length; j++)
            {
                float dot = Vector3.Dot((verts2[j]), axis);
                if (dot < minDot1) minDot1 = dot;
                if (dot > maxDot1) maxDot1 = dot;
            }

            float overlap = maxDot2 - minDot1;
            if (minDot1 < minDot2)
            {
                if (maxDot1 < minDot2) overlap = 0f;
                else overlap = maxDot1 - minDot2;
            }
            if (maxDot2 < minDot1) overlap = 0f;

            if (overlap < minOverlap) minOverlap = overlap;

            if (overlap <= 0) return false;
        }
        return true;
    }

    private static void ResolveCollisionOverlaps(PSI_Collider col1, PSI_Collider col2, Vector3 collisionAxis, float overlap)
    {
        var inverseMass1 = col1.pRigidbody ? 1.0f / col1.pRigidbody.pMass : 0f;
        var inverseMass2 = col2.pRigidbody ? 1.0f / col2.pRigidbody.pMass : 0f;
        if (inverseMass1 + inverseMass2 == 0f) return;

        var minTranslationVector = -collisionAxis.normalized * Mathf.Abs(overlap);
        col1.transform.Translate(minTranslationVector * (inverseMass1 / (inverseMass1 + inverseMass2)));
        col2.transform.Translate(-minTranslationVector * (inverseMass2 / (inverseMass1 + inverseMass2)));
    }

    private static void ApplyImpulses(PSI_Collider col1, PSI_Collider col2, Vector3 collisionAxis)
    {
        Vector3[] velocities = new Vector3[] { Vector3.zero, Vector3.zero };
        float[] inverseMasses = new float[] { 0f, 0f };
        float[] coeffsOfRes = new float[] { 1f, 1f };
        PSI_Rigidbody[] rbs = new PSI_Rigidbody[] { col1.pRigidbody, col2.pRigidbody };

        for (int j = 0; j < 2; j++)
        {
            if (rbs[j] != null)
            {
                velocities[j] = rbs[j].Velocity;
                inverseMasses[j] = 1.0f / rbs[j].pMass;
                coeffsOfRes[j] = rbs[j].pCoeffOfRest;
            }
        }

        var impactVelocity = velocities[0] - velocities[1];

        var vn = Vector3.Dot(impactVelocity, -collisionAxis.normalized);
        if (vn > 0.0f) return;

        var finalCoeffOfRest = coeffsOfRes[0] * coeffsOfRes[1];
        var i = (-(1.0f + finalCoeffOfRest) * vn) / (inverseMasses[0] + inverseMasses[1]);
        var impulse = -collisionAxis.normalized * i;

        if(rbs[0] != null) rbs[0].Velocity += impulse * inverseMasses[0];
        if (rbs[1] != null) rbs[1].Velocity += -impulse * inverseMasses[1];
    }
}
