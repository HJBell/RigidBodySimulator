using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSI_Plane {

    public Vector3 Position;
    public Vector2 Dims;

    public Vector3 pNormal { get { return mRotation * Vector3.up; } }
    public float pArea { get { return Dims.x * Dims.y; } }

    private Quaternion mRotation = Quaternion.identity;


    //----------------------------------------Public Functions---------------------------------------

    public PSI_Plane() { }

    public PSI_Plane(Vector3 pos, Quaternion rot, Vector2 dims)
    {
        UpdatePlane(pos, rot, dims);
    }

    public void UpdatePlane(Vector3 pos, Quaternion rot, Vector2 dims)
    {
        // Updating the properties of the plane.
        Position = pos;
        Dims = dims;
        mRotation = rot;
    }

    public Vector3[] GetVertices()
    {
        // Returning the points representing the corners of the plane.
        Vector3[] vertices = new Vector3[4];
        vertices[0] = Position + (mRotation * new Vector3(Dims.x / 2, 0.0f, Dims.y / 2));
        vertices[1] = Position + (mRotation * new Vector3(Dims.x / 2, 0.0f, -Dims.y / 2));
        vertices[2] = Position + (mRotation * new Vector3(-Dims.x / 2, 0.0f, -Dims.y / 2));
        vertices[3] = Position + (mRotation * new Vector3(-Dims.x / 2, 0.0f, Dims.y / 2));
        return vertices;
    }

    public bool PointProjectsOntoPlane(Vector3 point, out float projectionDistance)
    {
        // Projecting the position onto the plane.
        projectionDistance = Vector3.Dot(pNormal, (point - Position));
        var relativePointOnPlane = point - projectionDistance * pNormal;

        // Generate 4 triangles between the corners of the plane and the projected point.
        var planeVerts = GetVertices();
        var triangles = new Vector3[4, 3];
        for (int j = 0; j < 4; j++)
        {
            triangles[j, 0] = relativePointOnPlane;
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

        // Returning true if the projected point on the plane is within the plane bounds.
        return (Mathf.Abs(totalTriArea - pArea) <= 0.01f);
    }
}
