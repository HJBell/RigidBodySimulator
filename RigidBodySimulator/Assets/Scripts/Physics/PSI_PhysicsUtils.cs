using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PSI_PhysicsUtils {


    //----------------------------------------Public Functions---------------------------------------

    public static bool SphereSphereCollisionCheck(PSI_Collider_Sphere col1, PSI_Collider_Sphere col2, out Vector3 point)
    {
        // Determine if the sphere are colliding by doing a distance check.
        var collisionVector = col2.pPosition - col1.pPosition;
        point = col1.pPosition + collisionVector / 2f;
        bool isColliding = collisionVector.magnitude <= Mathf.Abs(col1.pRadius) + Mathf.Abs(col2.pRadius);

        if (isColliding)
        {
            // Resolve any overlap between the spheres.
            var collisionAxis = CorrectCollisionAxisDirection(col1.pPosition, col2.pPosition, collisionVector);
            var overlap = collisionAxis.magnitude - (col1.pRadius + col2.pRadius);
            ResolveCollisionOverlaps(col1, col2, collisionVector, overlap);

            // Calculate and apply the collision impulse to the spheres.
            ApplyImpulses(col1, col2, collisionVector, point);

            return true;
        }
        return false;
    }

    public static bool SpherePlaneCollisionOccured(PSI_Collider_Sphere sphereCol, PSI_Collider_Plane planeCol, out Vector3 point)
    {
        // Determine the signed distance to from the sphere to its projected point on the plane.
        float distToProjectedPoint = Vector3.Dot(planeCol.pNormal, (sphereCol.pPosition - planeCol.pPosition));

        // Determine the spheres projected point on the plane.
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

        // Determine if a collision occured by checking if the sphere is overlapping with the plane and the projected point
        // is within the extents of the plane by checking the sum area of the triangles is equal to the size of the plane.
        bool isColliding = Mathf.Abs(totalTriArea - planeCol.pArea) < 0.01f && Mathf.Abs(distToProjectedPoint) <= sphereCol.pRadius;

        if (isColliding)
        {
            // Resolve any overlap between the sphere and the plane.
            var collisionAxis = CorrectCollisionAxisDirection(planeCol.pPosition, sphereCol.pPosition, point - sphereCol.pPosition);
            var overlap = collisionAxis.magnitude - sphereCol.pRadius;
            ResolveCollisionOverlaps(sphereCol, planeCol, collisionAxis, overlap);

            // Calculate and apply the collision impulse to the sphere.
            ApplyImpulses(sphereCol, planeCol, collisionAxis, point);

            // Apply friction to the sphere.
            ApplyFriction(sphereCol, planeCol, point);

            return true;
        }
        return false;
    }

    public static bool SphereBoxCollisionOccured(PSI_Collider_Sphere sphereCol, PSI_Collider_Box boxCol, out Vector3 point)
    {
        // Get the closest point on the surface of the box to the sphere.
        point = boxCol.GetClosestPointOnBox(sphereCol.pPosition);

        // Do a distance check to see if the sphere and box are colliding.
        var collisionVector = point - sphereCol.pPosition;
        bool isColliding = collisionVector.magnitude <= Mathf.Abs(sphereCol.pRadius);

        if (isColliding)
        {
            // Resolve any overlap between the sphere and the box.
            var collisionAxis = CorrectCollisionAxisDirection(boxCol.pPosition, sphereCol.pPosition, collisionVector);
            float overlap = collisionVector.magnitude - sphereCol.pRadius;
            ResolveCollisionOverlaps(sphereCol, boxCol, collisionAxis, overlap);

            // Calculate and apply the collision impulse to the sphere and the box.
            ApplyImpulses(sphereCol, boxCol, collisionAxis, point);

            return true;
        }

        return false;
    }

    public static bool BoxBoxCollisionOccured(PSI_Collider_Box col1, PSI_Collider_Box col2, out Vector3 point)
    {
        point = Vector3.zero;

        // Determine the axis to check during SAT.
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

        // Determine the verts to check during SAT.
        var col1Verts = col1.GetVertices();
        var col2Verts = col2.GetVertices();

        // Determine if the boxes are colliding using SAT.
        Vector3 minOverlapAxis = Vector3.zero;
        float minOverlap = 0f;
        bool isColliding = CheckForOverlapUsingSAT(axesToCheck, col2Verts, col1Verts, out minOverlapAxis, out minOverlap) ||
                           CheckForOverlapUsingSAT(axesToCheck, col1Verts, col2Verts, out minOverlapAxis, out minOverlap);

        if (isColliding)
        {
            // Estimate the collision point by converting the box faces to planes and projecting the vertices of the other
            // box onto them. The projection points with the smallest distance are averaged to give the collision point.
            var facePlanes = new PSI_Plane[][] { col1.GetFacePlanes(), col2.GetFacePlanes() };
            var verts = new Vector3[][] { col1Verts, col2Verts };
            var contactPoints = new List<Vector3>();
            float contactPointDist = float.PositiveInfinity;
            for(int i = 0; i < 2; i++)
            {
                foreach (var plane in facePlanes[i])
                {
                    foreach (var vert in verts[1-i])
                    {
                        float projectionDist = float.PositiveInfinity;
                        if (plane.PointProjectsOntoPlane(vert, out projectionDist))
                        {
                            projectionDist = Mathf.Abs(projectionDist);
                            if (Mathf.Abs(projectionDist - contactPointDist) < float.Epsilon)
                                contactPoints.Add(vert);
                            else if (projectionDist < contactPointDist)
                            {
                                contactPoints.Clear();
                                contactPoints.Add(vert);
                                contactPointDist = projectionDist;
                            }
                        }
                    }
                }
            }
            point = Vector3.zero;
            foreach (var contactPoint in contactPoints)
                point += contactPoint;
            point /= contactPoints.Count;

            // Resolve any overlap between the boxes.
            var collisionAxis = CorrectCollisionAxisDirection(col2.pPosition, col1.pPosition, minOverlapAxis);
            ResolveCollisionOverlaps(col1, col2, collisionAxis, minOverlap);

            // Calculate and apply the collision impulse to the boxes.
            ApplyImpulses(col1, col2, collisionAxis, point);

            return true;
        }
        return false;
    }

    public static bool BoxPlaneCollisionOccured(PSI_Collider_Box boxCol, PSI_Collider_Plane planeCol, out Vector3 point)
    {
        // Projecting the box vertices onto the plane. If a the vertex that is closest
        // to the plane intersects it then the box and plane are colliding.
        var boxVerts = boxCol.GetVertices();
        float overlap = float.PositiveInfinity;
        bool collisionOccured = false;
        point = Vector3.zero;        
        for(int i = 0; i < boxVerts.Length; i++)
        {
            float distanceToPlane;
            if (!planeCol.PosIsWithinPlaneBounds(boxVerts[i], out distanceToPlane))
                continue;

            if (distanceToPlane <= 0f && Mathf.Abs(distanceToPlane) < boxCol.pSize.magnitude)
            {
                collisionOccured = true;
                if (distanceToPlane < overlap) overlap = distanceToPlane;
            }
        }

        if (collisionOccured)
        {
            // Resolve any overlap between the box and the plane.
            ResolveCollisionOverlaps(boxCol, planeCol, -planeCol.pNormal.normalized, overlap);

            // Getting the collision point by averaging the closest box verts to the plane.
            boxVerts = boxCol.GetVertices();
            int touchingVertexCount = 0;
            for (int i = 0; i < boxVerts.Length; i++)
            {
                float distanceToPlane;
                if (!planeCol.PosIsWithinPlaneBounds(boxVerts[i], out distanceToPlane))
                    continue;

                if (distanceToPlane <= 0.01f)
                {
                    point += boxVerts[i];
                    touchingVertexCount++;
                }
            }
            point /= (float)touchingVertexCount;

            // Calculate and apply the collision impulse to the box.
            ApplyImpulses(boxCol, planeCol, -planeCol.pNormal.normalized, point);

            // Apply friction to the box.
            ApplyFriction(boxCol, planeCol, point);

            return true;
        }
        return false;
    }


    //===== Moments Of Inertia =====
    
    public static float MomentOfInertiaOfSphere(float mass, float radius)
    {
        return (2f / 5f) * mass * Mathf.Pow(radius, 2);
    }

    public static Vector3 MomentOfInertiaOfBox(float mass, Vector3 dims)
    {
        // Calculating the moment of intertia of a cubeoid on each axis and returning as a vector3.
        Vector3 momentOfInertia = Vector3.zero;
        momentOfInertia.x = (mass / 12f) * (Mathf.Pow(dims.z, 2) + Mathf.Pow(dims.y, 2));
        momentOfInertia.y = (mass / 12f) * (Mathf.Pow(dims.x, 2) + Mathf.Pow(dims.z, 2));
        momentOfInertia.z = (mass / 12f) * (Mathf.Pow(dims.x, 2) + Mathf.Pow(dims.y, 2));
        return momentOfInertia;
    }


    //----------------------------------------Private Functions--------------------------------------

    private static bool CheckForOverlapUsingSAT(Vector3[] separatingAxes, Vector3[] verts1, Vector3[] verts2, out Vector3 minOverlapAxis, out float minOverlap)
    {
        // Check whether any of the vertices from first set overlap with any from the second 
        // set on any of the separating axes. If so then a collision has been found.
        minOverlapAxis = Vector3.zero;
        minOverlap = float.PositiveInfinity;
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
                if (dot < minDot1) minDot1 = dot;
                if (dot > maxDot1) maxDot1 = dot;
            }

            for (int j = 0; j < verts2.Length; j++)
            {
                float dot = Vector3.Dot((verts2[j]), axis);
                if (dot < minDot2) minDot2 = dot;
                if (dot > maxDot2) maxDot2 = dot;
            }

            float overlap = maxDot2 - minDot1;
            if (minDot1 < minDot2)
            {
                if (maxDot1 < minDot2) overlap = 0f;
                else overlap = maxDot1 - minDot2;
            }
            if (maxDot2 < minDot1) overlap = 0f;

            if (overlap < minOverlap)
            {
                minOverlap = overlap;
                minOverlapAxis = separatingAxes[i];
            }

            if (overlap <= 0) return false;
        }
        return true;
    }

    private static void ResolveCollisionOverlaps(PSI_Collider col1, PSI_Collider col2, Vector3 collisionAxis, float overlap)
    {
        // Calculating the invese masses of the bodies.
        var inverseMass1 = col1.pRigidbody ? 1.0f / col1.pRigidbody.Mass : 0f;
        var inverseMass2 = col2.pRigidbody ? 1.0f / col2.pRigidbody.Mass : 0f;
        if (inverseMass1 + inverseMass2 == 0f) return;

        // Translating the bodies by an amount proportional to their
        // inverse mass so that they are no longer overlapping.
        var minTranslationVector = -collisionAxis.normalized * Mathf.Abs(overlap);
        col1.transform.Translate(minTranslationVector * (inverseMass1 / (inverseMass1 + inverseMass2)), Space.World);
        col2.transform.Translate(-minTranslationVector * (inverseMass2 / (inverseMass1 + inverseMass2)), Space.World);
    }

    private static void ApplyImpulses(PSI_Collider col1, PSI_Collider col2, Vector3 collisionAxis, Vector3 collisionPoint)
    {
        // Gathering the necessary physics properties from the bodies.
        Vector3[] velocities = new Vector3[] { Vector3.zero, Vector3.zero };
        float[] inverseMasses = new float[] { 0f, 0f };
        float[] coeffsOfRes = new float[] { 1f, 1f };
        PSI_Rigidbody[] rbs = new PSI_Rigidbody[] { col1.pRigidbody, col2.pRigidbody };
        for (int j = 0; j < 2; j++)
        {
            if (rbs[j] != null)
            {
                velocities[j] = rbs[j].Velocity;
                inverseMasses[j] = 1.0f / rbs[j].Mass;
                coeffsOfRes[j] = rbs[j].CoeffOfRest;
            }
        }

        // Calculating the impact velocity.
        var impactVelocity = velocities[0] - velocities[1];

        // Determining the difference between the collision axis and the impact velocity.
        var vn = Vector3.Dot(impactVelocity, -collisionAxis.normalized);
        if (vn > 0.0f) return;

        // Calculating the final collision impulse.
        var finalCoeffOfRest = coeffsOfRes[0] * coeffsOfRes[1];
        var i = (-(1.0f + finalCoeffOfRest) * vn) / (inverseMasses[0] + inverseMasses[1]);
        var impulse = -collisionAxis.normalized * i;

        // Applying the collision impulse to the bodies.
        if(rbs[0] != null) rbs[0].ApplyImpulseAtPoint(impulse, collisionPoint);
        if (rbs[1] != null) rbs[1].ApplyImpulseAtPoint(impulse, collisionPoint);
    }

    private static void ApplyFriction(PSI_Collider col, PSI_Collider_Plane planeCol, Vector3 collisionPoint)
    {
        // Calculating and applying friction to a body moving along a plane.
        if (!col.pRigidbody) return;
        var incline = Vector3.Angle(Vector3.up, planeCol.pNormal);
        var normalForce = col.pRigidbody.Mass * 9.81f * Mathf.Cos(incline);
        var coeffFriction = col.pRigidbody.CoeffOfFrict;
        var friction = coeffFriction * normalForce;
        col.pRigidbody.AddFrictionAtPoint(friction * -col.pRigidbody.Velocity.normalized, collisionPoint);
    }

    private static Vector3 CorrectCollisionAxisDirection(Vector3 colPoint1, Vector3 colPoint2, Vector3 colAxis)
    {
        // Ensuring that the collision axis has the correct orientation.
        if (Vector3.Dot(colPoint1 - colPoint2, colAxis) < 0f)
            return -colAxis;
        return colAxis;
    }
}
