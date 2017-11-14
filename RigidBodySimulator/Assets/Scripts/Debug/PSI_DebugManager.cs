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
        if(mTaskbar != null)
        {
            var transformWindow = mTaskbar.GetWindow("Transform");

            if (mSelectedObject != null)
            {
                transformWindow.ClearContent();
                transformWindow.SetContent("Position:  " + mSelectedObject.transform.position);
                transformWindow.AddContentLine("Rotation:  " + mSelectedObject.transform.eulerAngles);
                transformWindow.AddContentLine("Scale:  " + mSelectedObject.transform.localScale);
            }
            else
            {
                transformWindow.ClearContent();
            }            
        }

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
}
