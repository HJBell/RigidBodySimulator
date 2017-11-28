using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColliderType
{
    Sphere, Plane, Box
};

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshRenderer))]
public abstract class PSI_Collider : PSI_SelectableObject {

    protected enum DrawMode
    {
        Gizmo, Debug
    }


    public ColliderType pType { get { return mType; } }
    public Vector3 pPosition { get { return this.transform.position + this.transform.TransformDirection(LocalPosition); } }
    public PSI_Rigidbody pRigidbody { get { return GetComponent<PSI_Rigidbody>(); } }

    public float ColliderScale = 1.0f;

    [SerializeField]
    private Vector3 LocalPosition;
    [Header("Debug Settings")]
    [SerializeField]
    private float DebugColourFadeDuration = 0.1f;
    [SerializeField]
    private Color IdleColour = Color.white;
    [SerializeField]
    private Color DebugColour = Color.magenta;
    [Space]

    protected ColliderType mType;
    protected bool mIsColliding = false;

    private MeshRenderer mMeshRenderer;
    private float mColourFadeTimer = 0.0f;
    private PSI_DebugRenderer mDebugRenderer;


    //----------------------------------------Unity Functions----------------------------------------

    protected virtual void Awake()
    {
        mDebugRenderer = FindObjectOfType<PSI_DebugRenderer>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        mMeshRenderer = this.GetComponent<MeshRenderer>();

        // Adding the collider to the physics manager.
        FindObjectOfType<PSI_PhysicsManager>().AddCollider(this);
    }

    protected override void OnDisable()
    {
        base.OnEnable();

        // Removing the collider from the physics manager.
        if (FindObjectOfType<PSI_PhysicsManager>())
            FindObjectOfType<PSI_PhysicsManager>().RemoveCollider(this);
    }

    protected virtual void LateUpdate()
    {
        // Drawing the collider debug if it is selected.
        if (mIsSelected) DrawDebug();

        // Fading the collider material colour back from debug to idle.
        mMeshRenderer.material.color = Color.Lerp(IdleColour, DebugColour, mColourFadeTimer);
        mColourFadeTimer = Mathf.Clamp01(mColourFadeTimer - (Time.deltaTime / DebugColourFadeDuration));
    }

    protected void OnDrawGizmos()
    {
        // Draw the collider using the gizmos.
        DrawCollider(DrawMode.Gizmo);
    }


    //----------------------------------------Public Functions---------------------------------------

    public void UseDebugColour()
    {
        // Setting the colour fader to its debug state of 1.
        mColourFadeTimer = 1f;
    }

    public void DrawDebug()
    {
        // Draw the collider using the debug renderer.
        DrawCollider(DrawMode.Debug);
    }


    //---------------------------------------Protected Functions-------------------------------------

    protected void DrawLine(Vector3 start, Vector3 end, DrawMode mode)
    {
        // Drawing a line either using the gizmos or debug renderer depending on the draw mode.
        if (mode == DrawMode.Gizmo) Gizmos.DrawLine(start, end);
        else mDebugRenderer.DrawLine(start, end);
    }

    protected void DrawWireSphere(Vector3 centre, float radius, DrawMode mode)
    {
        // Drawing a wire sphere either using the gizmos or debug renderer depending on the draw mode.
        if (mode == DrawMode.Gizmo) Gizmos.DrawWireSphere(centre, radius);
        else mDebugRenderer.DrawWireSphere(centre, radius);
    }

    protected abstract void DrawCollider(DrawMode mode);
}
