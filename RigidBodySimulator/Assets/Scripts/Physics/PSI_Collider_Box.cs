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
        this.mType = ColliderType.Box;
    }


    //----------------------------------------Public Functions---------------------------------------

    public Vector3[] GetVertices()
    {
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
        Vector3[] axis = new Vector3[3];
        axis[0] = this.transform.right;
        axis[1] = this.transform.up;
        axis[2] = this.transform.forward;
        return axis;
    }


    //---------------------------------------Protected Functions-------------------------------------

    protected override void DrawCollider(DrawMode mode)
    {
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
            DrawLine(verts[1 + i * 3], verts[6 - i * 3], mode);
        }

        //var axis = GetAxis();
        //for (int i = 0; i < 3; i++)
        //    DrawLine(pPosition, pPosition + axis[i] * 3, mode);
    }
}
