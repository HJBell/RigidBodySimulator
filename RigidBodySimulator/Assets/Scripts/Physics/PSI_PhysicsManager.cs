using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class PSI_PhysicsManager : MonoBehaviour {

    private List<PSI_Collider> mColliders = new List<PSI_Collider>();


    //--------------------------------------Unity Functions--------------------------------------

    private void FixedUpdate()
    {
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


    //------------------------------------Private Functions-------------------------------------

    private void CheckForCollision(PSI_Collider col1, PSI_Collider col2)
    {
        // Sphere on sphere.
        if (col1.pColliderType == ColliderType.Sphere && col2.pColliderType == ColliderType.Sphere)
            if (PSI_PhysicsUtils.SphereSphereCollisionOccured((PSI_Collider_Sphere)col1, (PSI_Collider_Sphere)col2))
                ;
    }
}
