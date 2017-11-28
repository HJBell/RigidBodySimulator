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

        // Adding the object to the debug manager.
        if (mDebugManager) mDebugManager.AddObject(this);
    }

    protected virtual void OnDisable()
    {
        // Removing the object from the debug manager.
        if (mDebugManager) mDebugManager.RemoveObject(this);
    }

    private void OnMouseDown()
    {
        // Selecting the object if the user clicks on it.
        if(!FindObjectOfType<EventSystem>().IsPointerOverGameObject()) Select();
    }


    //----------------------------------------Public Functions---------------------------------------

    public virtual void Select()
    {
        // Outlining the object.
        this.GetComponent<cakeslice.Outline>().enabled = true;

        // Informing the debug manager that the object was selected.
        if (mDebugManager) mDebugManager.ObjectSelected(this);

        // Recording locally that the object is now selected.
        mIsSelected = true;
    }

    public virtual void Deselect()
    {
        // Disbaling the outline for the object.
        this.GetComponent<cakeslice.Outline>().enabled = false;

        // Recording locally that the object is no longer selected.
        mIsSelected = false;
    }
}
