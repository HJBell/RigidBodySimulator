﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColliderType
{
    Sphere
};

[RequireComponent(typeof(Collider))]
public abstract class PSI_Collider : PSI_SelectableObject {

    public Vector3 LocalPosition;

    [SerializeField]
    protected float ColliderScale = 1.0f;

    protected ColliderType mType;

    public ColliderType pColliderType { get { return mType; } }
    public Vector3 pPosition { get { return this.transform.position + this.transform.TransformDirection(LocalPosition); } }


    //----------------------------------------Unity Functions----------------------------------------

    protected override void OnEnable()
    {
        base.OnEnable();
        FindObjectOfType<PSI_PhysicsManager>().AddCollider(this);
    }

    protected override void OnDisable()
    {
        base.OnEnable();
        if (FindObjectOfType<PSI_PhysicsManager>())
            FindObjectOfType<PSI_PhysicsManager>().RemoveCollider(this);
    }

    private void Update()
    {
        if (mIsSelected) DrawDebug();
    }

    protected abstract void OnDrawGizmos();


    //----------------------------------------Public Functions---------------------------------------

    public abstract void DrawDebug();
}
