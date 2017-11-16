using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PSI_DebugManager : MonoBehaviour {

    private List<PSI_SelectableObject> mSelectableObjects = new List<PSI_SelectableObject>();
    private PSI_SelectableObject mSelectedObject;
    private PSI_UITaskbar mTaskbar;


    //----------------------------------------Unity Functions----------------------------------------

    private void Start()
    {
        mTaskbar = FindObjectOfType<PSI_UITaskbar>();
    }

    private void Update()
    {
        if (mTaskbar != null)
            UpdateWindowContent();

        // Deselecting the current object if the player clicks in empty space.
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, 100) && !FindObjectOfType<EventSystem>().IsPointerOverGameObject())
                ObjectSelected(null);
        }
    }


    //----------------------------------------Public Functions---------------------------------------

    public void AddObject(PSI_SelectableObject obj)
    {
        if (!mSelectableObjects.Contains(obj))
            mSelectableObjects.Add(obj);
    }

    public void RemoveObject(PSI_SelectableObject obj)
    {
        if (mSelectableObjects.Contains(obj))
            mSelectableObjects.Remove(obj);
    }

    public void ObjectSelected(PSI_SelectableObject obj)
    {
        foreach (var listObj in mSelectableObjects)
            if (listObj != obj)
                listObj.Deselect();
        mSelectedObject = obj;
    }


    //----------------------------------------Private Functions--------------------------------------

    private void UpdateWindowContent()
    {
        var transformWindow = mTaskbar.GetWindow("Transform");
        var rigidbodyWindow = mTaskbar.GetWindow("Rigidbody");

        if(transformWindow != null)
        {
            transformWindow.ClearContent();
            if (mSelectedObject != null)
            {
                var trans = mSelectedObject.transform;
                transformWindow.SetContent("Position:  " + trans.position);
                transformWindow.AddContentLine("Rotation:  " + trans.eulerAngles);
                transformWindow.AddContentLine("Scale:  " + trans.localScale);
            }
        }

        if (rigidbodyWindow != null)
        {
            rigidbodyWindow.ClearContent();
            if (mSelectedObject != null && mSelectedObject.GetComponent<PSI_Rigidbody>() != null)
            {
                var rb = mSelectedObject.GetComponent<PSI_Rigidbody>();
                rigidbodyWindow.SetContent("Velocity:  " + rb.Velocity);
                rigidbodyWindow.AddContentLine("Angular Velocity:  " + rb.AngularVelocity);
                rigidbodyWindow.AddContentLine("Coeff Of Rest:  " + rb.pCoeffOfRest);
            }
        }
    }
}
