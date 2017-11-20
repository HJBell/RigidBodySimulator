using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public struct PSI_Collision
{
    public PSI_Collider col1;
    public PSI_Collider col2;
    public Vector3 point;
}

public class PSI_PhysicsManager : MonoBehaviour {

    private List<PSI_Collider> mColliders = new List<PSI_Collider>();
    private List<PSI_Collision> mCollisionData = new List<PSI_Collision>();


    //--------------------------------------Unity Functions--------------------------------------

    private void FixedUpdate()
    {
        mCollisionData.Clear();
        for (int i = 0; i < mColliders.Count; i++)
            for (int j = i + 1; j < mColliders.Count; j++)
                CheckForCollision(mColliders[i], mColliders[j]);
    }


    //-------------------------------------Public Functions-------------------------------------    

    public void AddCollider(PSI_Collider collider)
    {
        if (!mColliders.Contains(collider))
            mColliders.Add(collider);
    }

    public void RemoveCollider(PSI_Collider collider)
    {
        if (mColliders.Contains(collider))
            mColliders.Remove(collider);
    }

    public List<PSI_Collision> GetCollisionData()
    {
        return mCollisionData;
    }


    //------------------------------------Private Functions-------------------------------------

    private void CheckForCollision(PSI_Collider col1, PSI_Collider col2)
    {
        bool collisionOccurred = false;
        PSI_Collision collision = new PSI_Collision();
        collision.col1 = col1;
        collision.col2 = col2;

        // Sphere on sphere.
        if (col1.pType == ColliderType.Sphere && col2.pType == ColliderType.Sphere)
            if (PSI_PhysicsUtils.SphereSphereCollisionOccured((PSI_Collider_Sphere)col1, (PSI_Collider_Sphere)col2, out collision.point))
                collisionOccurred = true;

        // Sphere on plane.
        if ((col1.pType == ColliderType.Sphere && col2.pType == ColliderType.Plane) ||
            (col1.pType == ColliderType.Plane && col2.pType == ColliderType.Sphere))
        {
            PSI_Collider_Sphere sphere = (PSI_Collider_Sphere)((col1.pType == ColliderType.Sphere) ? col1 : col2);
            PSI_Collider_Plane plane = (PSI_Collider_Plane)((col1.pType == ColliderType.Sphere) ? col2 : col1);
            if (PSI_PhysicsUtils.SpherePlaneCollisionOccured(sphere, plane, out collision.point))
                collisionOccurred = true;
        }

        // Sphere on box.
        if ((col1.pType == ColliderType.Sphere && col2.pType == ColliderType.Box) ||
            (col1.pType == ColliderType.Box && col2.pType == ColliderType.Sphere))
        {
            PSI_Collider_Sphere sphere = (PSI_Collider_Sphere)((col1.pType == ColliderType.Sphere) ? col1 : col2);
            PSI_Collider_Box box = (PSI_Collider_Box)((col1.pType == ColliderType.Sphere) ? col2 : col1);
            if (PSI_PhysicsUtils.SphereBoxCollisionOccured(sphere, box, out collision.point))
                collisionOccurred = true;
        }

        // Box on box.
        if ((col1.pType == ColliderType.Box && col2.pType == ColliderType.Box))
          if (PSI_PhysicsUtils.BoxBoxCollisionOccured((PSI_Collider_Box)col1, (PSI_Collider_Box)col2, out collision.point))
                collisionOccurred = true;

        // Box on plane.
        if ((col1.pType == ColliderType.Box && col2.pType == ColliderType.Plane) ||
            (col1.pType == ColliderType.Plane && col2.pType == ColliderType.Box))
        {
            PSI_Collider_Box box = (PSI_Collider_Box)((col1.pType == ColliderType.Box) ? col1 : col2);
            PSI_Collider_Plane plane = (PSI_Collider_Plane)((col1.pType == ColliderType.Box) ? col2 : col1);
            if (PSI_PhysicsUtils.BoxPlaneCollisionOccured(box, plane, out collision.point))
                collisionOccurred = true;
        }

        if (collisionOccurred) mCollisionData.Add(collision);
    }
}
