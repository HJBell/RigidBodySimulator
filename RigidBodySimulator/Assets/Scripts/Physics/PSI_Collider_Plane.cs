using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSI_Collider_Plane : PSI_Collider {

    public Vector2 pSize { get { return new Vector2(this.transform.lossyScale.x * ColliderScale, this.transform.lossyScale.z * ColliderScale) * 10f; } }
    public Vector3 pNormal { get { return this.transform.TransformDirection(Vector3.up).normalized; } }
    public float pArea { get { return pSize.x * pSize.y; } }


    //----------------------------------------Unity Functions----------------------------------------

    protected override void Awake()
    {
        base.Awake();
        this.mType = ColliderType.Plane;
    }


    //----------------------------------------Public Functions---------------------------------------

    public Vector3[] GetVertices()
    {
        Vector3[] vertices = new Vector3[4];
        vertices[0] = pPosition + this.transform.TransformDirection(new Vector3(pSize.x / 2, 0.0f, pSize.y / 2));
        vertices[1] = pPosition + this.transform.TransformDirection(new Vector3(pSize.x / 2, 0.0f, -pSize.y / 2));
        vertices[2] = pPosition + this.transform.TransformDirection(new Vector3(-pSize.x / 2, 0.0f, -pSize.y / 2));
        vertices[3] = pPosition + this.transform.TransformDirection(new Vector3(-pSize.x / 2, 0.0f, pSize.y / 2));
        return vertices;
    }

    public bool PosIsWithinPlaneBounds(Vector3 pos, out float distanceToPlane)
    {
        // Projecting the position onto the plane.
        distanceToPlane = Vector3.Dot(pNormal, (pos - pPosition));
        var relativePointOnPlane = pos - distanceToPlane * pNormal;

        // Generate 4 triangles between the corners of the plane and the projected point.
        var verts = GetVertices();
        var triangles = new Vector3[4, 3];
        for (int j = 0; j < 4; j++)
        {
            triangles[j, 0] = relativePointOnPlane;
            triangles[j, 1] = verts[j];
            triangles[j, 2] = verts[(j == 3) ? 0 : j + 1];
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


    //---------------------------------------Protected Functions-------------------------------------

    protected override void DrawCollider(DrawMode mode)
    {
        var verts = GetVertices();
        for(int i = 0; i < verts.Length; i++)
            verts[i] += pNormal.normalized * 0.005f;
        DrawLine(verts[0], verts[1], mode);
        DrawLine(verts[1], verts[2], mode);
        DrawLine(verts[2], verts[3], mode);
        DrawLine(verts[3], verts[0], mode);
        DrawLine(verts[0], verts[2], mode);
        DrawLine(verts[1], verts[3], mode);
    }
}
