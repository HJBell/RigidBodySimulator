using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSI_Collider_Sphere : PSI_Collider {

    public float pRadius { get { return Mathf.Max(new float[] { transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z }) * ColliderScale * .5f; } }


    //----------------------------------------Unity Functions----------------------------------------

    protected override void Awake()
    {
        base.Awake();
        this.mType = ColliderType.Sphere;
    }


    //---------------------------------------Protected Functions-------------------------------------

    protected override void DrawCollider(DrawMode mode)
    {
        if(mode == DrawMode.Gizmo) Gizmos.DrawWireSphere(pPosition, pRadius);
        else FindObjectOfType<PSI_DebugRenderer>().DrawWireSphere(pPosition, pRadius);
    }
}
