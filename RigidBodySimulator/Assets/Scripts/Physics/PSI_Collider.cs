using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColliderType
{
    Sphere, Plane
};

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshRenderer))]
public abstract class PSI_Collider : PSI_SelectableObject {


    public ColliderType pType { get { return mType; } }
    public Vector3 pPosition { get { return this.transform.position + this.transform.TransformDirection(LocalPosition); } }
    public bool pIsColliding { set { mIsColliding = value; } }

    [SerializeField]
    protected float ColliderScale = 1.0f;

    [SerializeField]
    private Vector3 LocalPosition;
    [Header("Debug Settings")]
    [SerializeField]
    private float DebugColourFadeDuration = 1f;
    [SerializeField]
    private Color IdleColour = Color.white;
    [SerializeField]
    private Color DebugColour = Color.magenta;
    [Space]

    protected ColliderType mType;
    protected bool mIsColliding = false;

    private MeshRenderer mMeshRenderer;
    private float mColourFadeTimer = 0.0f;

    


    //----------------------------------------Unity Functions----------------------------------------

    protected override void OnEnable()
    {
        base.OnEnable();
        FindObjectOfType<PSI_PhysicsManager>().AddCollider(this);
        mMeshRenderer = this.GetComponent<MeshRenderer>();
    }

    protected override void OnDisable()
    {
        base.OnEnable();
        if (FindObjectOfType<PSI_PhysicsManager>())
            FindObjectOfType<PSI_PhysicsManager>().RemoveCollider(this);
    }

    protected virtual void Update()
    {
        if (mIsSelected) DrawDebug();

        mMeshRenderer.material.color = Color.Lerp(IdleColour, DebugColour, mColourFadeTimer);
        mColourFadeTimer = Mathf.Clamp01(mColourFadeTimer - (Time.deltaTime / DebugColourFadeDuration));
    }

    protected abstract void OnDrawGizmos();


    //----------------------------------------Public Functions---------------------------------------

    public void UseDebugColour()
    {
        mColourFadeTimer = 1f;
    }

    public abstract void DrawDebug();
}
