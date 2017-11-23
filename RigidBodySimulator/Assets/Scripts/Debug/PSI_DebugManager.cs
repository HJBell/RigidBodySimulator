using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PSI_DebugManager : MonoBehaviour {

    [SerializeField]
    private Image ColPointMarkerPrefab;
    [SerializeField]
    private int ColPointMarkerPoolSize = 20;

    private List<PSI_SelectableObject> mSelectableObjects = new List<PSI_SelectableObject>();
    private PSI_SelectableObject mSelectedObject;
    private PSI_UITaskbar mTaskbar;
    private PSI_PhysicsManager mPhysicsManager;
    private bool mHighlightCollisions = false;
    private List<Image> mColPointMarkerPool = new List<Image>();
    private int mColPointMarkerIndex = 0;


    //----------------------------------------Unity Functions----------------------------------------

    private void Start()
    {
        mTaskbar = FindObjectOfType<PSI_UITaskbar>();
        mPhysicsManager = FindObjectOfType<PSI_PhysicsManager>();
        BuildCollisionPointMarkerPool();
    }

    private void Update()
    {
        // Deselecting the current object if the player clicks in empty space.
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, 100) && !FindObjectOfType<EventSystem>().IsPointerOverGameObject())
                ObjectSelected(null);
        }

        // Updating the debug window content.
        UpdateWindowContent();
    }

    private void LateUpdate()
    {
        ResetCollisionPointMarkers();

        // Debugging colliding objects and collision points.
        if (mHighlightCollisions)
        {
            var collisionData = mPhysicsManager.GetCollisionData();
            foreach (var collision in collisionData)
            {
                collision.col1.UseDebugColour();
                collision.col2.UseDebugColour();
                PlaceCollisionPointMarker(collision.point);
            }
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
        var rigidbodyWindow = mTaskbar.GetWindow(UIWindowType.Rigidbody);

        float timeScale = Time.timeScale;
        float.TryParse(settingsWindow.GetSetContentValue("TimeScale"), out timeScale);
        Time.timeScale = timeScale;

        bool.TryParse(settingsWindow.GetSetContentValue("HighlightCollisions"), out mHighlightCollisions);

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

            if(mSelectedObject.GetComponent<PSI_Rigidbody>())
            {
                var rigidbody = mSelectedObject.GetComponent<PSI_Rigidbody>();

                Vector3 vel = rigidbody.Velocity;
                float.TryParse(rigidbodyWindow.GetSetContentValue("VelX", vel.x.ToString()), out vel.x);
                float.TryParse(rigidbodyWindow.GetSetContentValue("VelY", vel.y.ToString()), out vel.y);
                float.TryParse(rigidbodyWindow.GetSetContentValue("VelZ", vel.z.ToString()), out vel.z);
                rigidbody.Velocity = vel;

                Vector3 angVel = rigidbody.AngularVelocity;
                float.TryParse(rigidbodyWindow.GetSetContentValue("AngVelX", angVel.x.ToString()), out angVel.x);
                float.TryParse(rigidbodyWindow.GetSetContentValue("AngVelY", angVel.y.ToString()), out angVel.y);
                float.TryParse(rigidbodyWindow.GetSetContentValue("AngVelZ", angVel.z.ToString()), out angVel.z);
                rigidbody.AngularVelocity = angVel;

                float mass = rigidbody.Mass;
                float.TryParse(rigidbodyWindow.GetSetContentValue("Mass", mass.ToString()), out mass);
                rigidbody.Mass = mass;

                float coeffRest = rigidbody.CoeffOfRest;
                float.TryParse(rigidbodyWindow.GetSetContentValue("CoeffRest"), out coeffRest);
                rigidbody.CoeffOfRest = coeffRest;

                float coeffFrict = rigidbody.CoeffOfFrict;
                float.TryParse(rigidbodyWindow.GetSetContentValue("CoeffFrict"), out coeffFrict);
                rigidbody.CoeffOfFrict = coeffFrict;

                bool.TryParse(rigidbodyWindow.GetSetContentValue("UseGravity"), out rigidbody.UseGravity);
            }
            else
            {
                rigidbodyWindow.ResetContent();
            }
        }
        else
        {
            transformWindow.ResetContent();
            rigidbodyWindow.ResetContent();
        }
    }

    private void BuildCollisionPointMarkerPool()
    {
        var canvas = GameObject.Find("Canvas");
        for (int i = 0; i < ColPointMarkerPoolSize; i++)
        {
            var obj = Instantiate(ColPointMarkerPrefab);
            obj.transform.SetParent(canvas.transform);
            obj.transform.SetAsFirstSibling();
            mColPointMarkerPool.Add(obj);
        }
    }

    private void ResetCollisionPointMarkers()
    {
        foreach (var marker in mColPointMarkerPool)
            marker.enabled = false;
        mColPointMarkerIndex = 0;
    }

    private void PlaceCollisionPointMarker(Vector3 worldPos)
    {
        var screenPos = Camera.main.WorldToScreenPoint(worldPos);
        var marker = mColPointMarkerPool[mColPointMarkerIndex];

        marker.enabled = true;
        marker.transform.position = new Vector3(screenPos.x, screenPos.y);
        marker.transform.localScale = Vector3.one / Vector3.Distance(Camera.main.transform.position, worldPos);

        mColPointMarkerIndex = Mathf.Clamp(mColPointMarkerIndex+1, 0, mColPointMarkerPool.Count - 1);
    }
}
