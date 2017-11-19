using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(cakeslice.Outline))]
public class PSI_SelectableObject : MonoBehaviour {

    protected bool mIsSelected = false;

    private PSI_DebugManager mDebugManager;


    //----------------------------------------Unity Functions----------------------------------------

    protected virtual void Start()
    {
        Deselect();
    }

    protected virtual void OnEnable()
    {
        mDebugManager = FindObjectOfType<PSI_DebugManager>();

        if (mDebugManager) mDebugManager.AddObject(this);
    }

    protected virtual void OnDisable()
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
        this.GetComponent<cakeslice.Outline>().enabled = true;
        if (mDebugManager) mDebugManager.ObjectSelected(this);
        mIsSelected = true;
    }

    public virtual void Deselect()
    {
        this.GetComponent<cakeslice.Outline>().enabled = false;
        mIsSelected = false;
    }
}
