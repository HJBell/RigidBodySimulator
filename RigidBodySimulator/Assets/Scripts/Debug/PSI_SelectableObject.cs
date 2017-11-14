﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PSI_SelectableObject : MonoBehaviour {

    [SerializeField]
    private float OutlineWidth = 0.1f;

    private PSI_DebugManager mDebugManager;


    //----------------------------------------Unity Functions----------------------------------------

    private void OnEnable()
    {
        mDebugManager = FindObjectOfType<PSI_DebugManager>();

        if (mDebugManager) mDebugManager.AddObject(this);
    }

    private void OnDisable()
    {
        if (mDebugManager) mDebugManager.RemoveObject(this);
    }

    private void OnMouseDown()
    {
        if(!FindObjectOfType<EventSystem>().IsPointerOverGameObject()) Select();
    }


    //----------------------------------------Public Functions---------------------------------------

    public virtual void Select()
    {
        GetComponent<Renderer>().material.SetFloat("_Outline", OutlineWidth);
        if (mDebugManager) mDebugManager.ObjectSelected(this);
    }

    public virtual void Deselect()
    {
        GetComponent<Renderer>().material.SetFloat("_Outline", 0f);
    }
}