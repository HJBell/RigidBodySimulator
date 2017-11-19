﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSI_Collider_Plane : PSI_Collider {

    public Vector2 pSize { get { return new Vector2(this.transform.lossyScale.x * ColliderScale, this.transform.lossyScale.z * ColliderScale) * 10f; } }
    public Vector3 pNormal { get { return this.transform.TransformDirection(Vector3.up).normalized; } }
    public float pArea { get { return pSize.x * pSize.y; } }


    //----------------------------------------Unity Functions----------------------------------------

    private void Awake()
    {
        this.mType = ColliderType.Plane;
    }

    protected override sealed void OnDrawGizmos()
    {
        var verts = GetPlaneVertices();
        Gizmos.DrawLine(verts[0], verts[1]);
        Gizmos.DrawLine(verts[1], verts[2]);
        Gizmos.DrawLine(verts[2], verts[3]);
        Gizmos.DrawLine(verts[3], verts[0]);
        Gizmos.DrawLine(verts[0], verts[2]);
        Gizmos.DrawLine(verts[1], verts[3]);
    }


    //----------------------------------------Public Functions---------------------------------------

    public Vector3[] GetPlaneVertices()
    {
        Vector3[] vertices = new Vector3[4];
        vertices[0] = pPosition + this.transform.TransformDirection(new Vector3(pSize.x / 2, 0.0f, pSize.y / 2));
        vertices[1] = pPosition + this.transform.TransformDirection(new Vector3(pSize.x / 2, 0.0f, -pSize.y / 2));
        vertices[2] = pPosition + this.transform.TransformDirection(new Vector3(-pSize.x / 2, 0.0f, -pSize.y / 2));
        vertices[3] = pPosition + this.transform.TransformDirection(new Vector3(-pSize.x / 2, 0.0f, pSize.y / 2));
        return vertices;
    }

    public override sealed void DrawDebug()
    {
        var verts = GetPlaneVertices();
        var renderer = FindObjectOfType<PSI_DebugRenderer>();
        renderer.DrawLine(verts[0], verts[1]);
        renderer.DrawLine(verts[1], verts[2]);
        renderer.DrawLine(verts[2], verts[3]);
        renderer.DrawLine(verts[3], verts[0]);
        renderer.DrawLine(verts[0], verts[2]);
        renderer.DrawLine(verts[1], verts[3]);
    }
}