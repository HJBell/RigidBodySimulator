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
        var settingsWindow = mTaskbar.GetWindow(UIWindowType.Settings);
        var transformWindow = mTaskbar.GetWindow(UIWindowType.Transform);

        float timeScale = Time.timeScale;
        float.TryParse(settingsWindow.GetSetContentValue("TimeScale"), out timeScale);
        Time.timeScale = timeScale;

        if (mSelectedObject != null)
        {
            Vector3 pos = mSelectedObject.transform.position;
            float.TryParse(transformWindow.GetSetContentValue("PosX", pos.x.ToString()), out pos.x);
            float.TryParse(transformWindow.GetSetContentValue("PosY", pos.y.ToString()), out pos.y);
            float.TryParse(transformWindow.GetSetContentValue("PosZ", pos.z.ToString()), out pos.z);
            mSelectedObject.transform.position = pos;

            Vector3 rot = mSelectedObject.transform.eulerAngles;
            float.TryParse(transformWindow.GetSetContentValue("RotX", rot.x.ToString()), out rot.x);
            float.TryParse(transformWindow.GetSetContentValue("RotY", rot.y.ToString()), out rot.y);
            float.TryParse(transformWindow.GetSetContentValue("RotZ", rot.z.ToString()), out rot.z);
            mSelectedObject.transform.eulerAngles = rot;

            Vector3 scale = mSelectedObject.transform.localScale;
            float.TryParse(transformWindow.GetSetContentValue("ScaleX", scale.x.ToString()), out scale.x);
            float.TryParse(transformWindow.GetSetContentValue("ScaleY", scale.y.ToString()), out scale.y);
            float.TryParse(transformWindow.GetSetContentValue("ScaleZ", scale.z.ToString()), out scale.z);
            mSelectedObject.transform.localScale = scale;
        }
        else
        {
            transformWindow.ResetContent();
        }
    }
}
