using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSI_Collider_Box : PSI_Collider {

    public Vector3 pSize { get { return this.transform.lossyScale * ColliderScale; } }


    //----------------------------------------Unity Functions----------------------------------------

    protected override void Awake()
    {
        base.Awake();

        // Setting the collider type.
        this.mType = ColliderType.Box;
    }


    //----------------------------------------Public Functions---------------------------------------

    public Vector3[] GetVertices()
    {   
        // Returning the points at the corners of the box.
        Vector3[] vertices = new Vector3[8];
        vertices[0] = pPosition + this.transform.TransformDirection(new Vector3(-pSize.x / 2, -pSize.y / 2, -pSize.z / 2));
        vertices[1] = pPosition + this.transform.TransformDirection(new Vector3(-pSize.x / 2, -pSize.y / 2, pSize.z / 2));
        vertices[2] = pPosition + this.transform.TransformDirection(new Vector3(-pSize.x / 2, pSize.y / 2, pSize.z / 2));
        vertices[3] = pPosition + this.transform.TransformDirection(new Vector3(-pSize.x / 2, pSize.y / 2, -pSize.z / 2));
        vertices[4] = pPosition + this.transform.TransformDirection(new Vector3(pSize.x / 2, -pSize.y / 2, -pSize.z / 2));
        vertices[5] = pPosition + this.transform.TransformDirection(new Vector3(pSize.x / 2, -pSize.y / 2, pSize.z / 2));
        vertices[6] = pPosition + this.transform.TransformDirection(new Vector3(pSize.x / 2, pSize.y / 2, pSize.z / 2));
        vertices[7] = pPosition + this.transform.TransformDirection(new Vector3(pSize.x / 2, pSize.y / 2, -pSize.z / 2));
        return vertices;
    }

    public Vector3[] GetAxes()
    {
        // Returning the up, right and forward vectors of the box.
        Vector3[] axis = new Vector3[3];
        axis[0] = this.transform.right;
        axis[1] = this.transform.up;
        axis[2] = this.transform.forward;
        return axis;
    }

    public float[] GetExtents()
    {
        // Getting the size of the box in an array (used for looping).
        return new float[] { pSize.x, pSize.y, pSize.z };
    }

    public Vector3 GetClosestPointOnBox(Vector3 point)
    {
        // Determining the closest point on the 
        // surface of the box to the given point.
        Vector3 closestPoint;
        Vector3 d = point - pPosition;
        var axes = GetAxes();
        var extents = GetExtents();
        closestPoint = pPosition;
        for (int i = 0; i < 3; i++)
        {
            float dist = Vector3.Dot(d, axes[i]);
            if (dist > extents[i] * 0.5f) dist = extents[i] * 0.5f;
            if (dist < -extents[i] * 0.5f) dist = -extents[i] * 0.5f;
            closestPoint += dist * axes[i];
        }
        return closestPoint;
    }

    public PSI_Plane[] GetFacePlanes()
    {
        // Generating a returning the box faces as planes.
        var facePlanes = new PSI_Plane[6];
        facePlanes[0] = new PSI_Plane(pPosition - transform.right * pSize.x * 0.5f, this.transform.rotation * Quaternion.Euler(0, 0, 90), new Vector2(pSize.y, pSize.z));
        facePlanes[1] = new PSI_Plane(pPosition - transform.up * pSize.y * 0.5f, this.transform.rotation * Quaternion.Euler(0, 0, 180), new Vector2(pSize.x, pSize.z));
        facePlanes[2] = new PSI_Plane(pPosition + transform.right * pSize.x * 0.5f, this.transform.rotation * Quaternion.Euler(0, 0, -90), new Vector2(pSize.y, pSize.z));
        facePlanes[3] = new PSI_Plane(pPosition + transform.up * pSize.y * 0.5f, this.transform.rotation * Quaternion.Euler(0, 0, 0), new Vector2(pSize.x, pSize.z));
        facePlanes[4] = new PSI_Plane(pPosition + transform.forward * pSize.z * 0.5f, this.transform.rotation * Quaternion.Euler(90, 0, 0), new Vector2(pSize.x, pSize.y));
        facePlanes[5] = new PSI_Plane(pPosition - transform.forward * pSize.z * 0.5f, this.transform.rotation * Quaternion.Euler(-90, 0, 0), new Vector2(pSize.x, pSize.y));
        return facePlanes;
    }


    //---------------------------------------Protected Functions-------------------------------------

    protected override void DrawCollider(DrawMode mode)
    {
        // Drawing the collider using the given drawmode.
        var originalScale = ColliderScale;
        ColliderScale *= 1.005f;

        var verts = GetVertices();

        // Drawing the edges.
        for (int i = 0; i < 4; i++)
        {
            int j = i == 3 ? 0 : i + 1;
            DrawLine(verts[i], verts[j], mode);
            DrawLine(verts[i + 4], verts[j + 4], mode);
            DrawLine(verts[i], verts[i + 4], mode);
        }

        // Drawing the crosses
        for (int i = 0; i < 2; i++)
        {
            DrawLine(verts[i], verts[i + 2], mode);
            DrawLine(verts[i + 4], verts[i + 6], mode);
            DrawLine(verts[i], verts[5 - i], mode);
            DrawLine(verts[2 + i], verts[7 - i], mode);
            DrawLine(verts[i * 3], verts[7 - i * 3], mode);
            DrawLine(verts[1 + i * 4], verts[6 - i * 4], mode);
        }

        ColliderScale = originalScale;
    }
}
