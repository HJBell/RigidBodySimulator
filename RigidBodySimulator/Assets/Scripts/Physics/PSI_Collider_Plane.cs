using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSI_Collider_Plane : PSI_Collider {

    public Vector2 pSize { get { return new Vector2(this.transform.lossyScale.x * ColliderScale, this.transform.lossyScale.z * ColliderScale) * 10f; } }
    public Vector3 pNormal { get { return this.transform.TransformDirection(Vector3.up).normalized; } }
    public float pArea { get { return pSize.x * pSize.y; } }

    private PSI_Plane mPlane = new PSI_Plane();


    //----------------------------------------Unity Functions----------------------------------------

    protected override void Awake()
    {
        base.Awake();
        this.mType = ColliderType.Plane;
        UpdatePlane();
    }


    //----------------------------------------Public Functions---------------------------------------

    public Vector3[] GetVertices()
    {
        UpdatePlane();
        return mPlane.GetVertices();
    }

    public bool PosIsWithinPlaneBounds(Vector3 pos, out float distanceToPlane)
    {
        return PSI_PhysicsUtils.PointProjectsOntoPlane(mPlane, pos, out distanceToPlane);
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


    //----------------------------------------Private Functions--------------------------------------

    private void UpdatePlane()
    {
        mPlane.UpdatePlane(pPosition, this.transform.rotation, pSize);
    }
}
