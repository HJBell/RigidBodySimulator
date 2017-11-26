using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSI_Rigidbody : MonoBehaviour {

    public Vector3 Velocity = Vector3.zero;
    public Vector3 AngularVelocity = Vector3.zero;
    [Range(1.0f, 20.0f)]
    public float Mass = 1.0f;
    [Range(0.0f, 1.0f)]
    [Tooltip("The coefficient of restitution for the body")]
    public float CoeffOfRest = 0.5f;
    [Range(0.0f, 1.0f)]
    [Tooltip("The coefficient of friction for the body")]
    public float CoeffOfFrict = 0.5f;
    public bool UseGravity = true;

    private float mAngularRestingStateCutoff = 20f;

    private Vector3 mForceThisFrame = Vector3.zero;
    private Vector3 mTorqueThisFrame = Vector3.zero;

    private Vector3 mImpulseThisFrame = Vector3.zero;
    private Vector3 mAngularImpulseThisFrame = Vector3.zero;

    private PSI_Collider pCollider { get { return GetComponent<PSI_Collider>(); } }


    //--------------------------------------Unity Functions--------------------------------------

    private void Update()
    {
        // Applying gravity if necessary.
        if (this.UseGravity) Velocity.y -= 9.81f * Time.deltaTime;

        // Updating the angular velocity.
        var angularAcceleration = CalculateAngularAcceleration(mTorqueThisFrame);
        AngularVelocity += angularAcceleration * Time.deltaTime;
        AngularVelocity += mAngularImpulseThisFrame;
        AngularVelocity -= AngularVelocity * 0.1f;

        // Updating the linear velocity.
        var acceleration = this.mForceThisFrame / this.Mass;
        this.Velocity += acceleration * Time.deltaTime;
        Velocity += mImpulseThisFrame;

        // Applying the linear velocity to the body if 
        // it is large enough to not be in a resting state.
        if (Velocity.magnitude > 9.81f * Time.deltaTime)
            this.transform.Translate(this.Velocity * Time.deltaTime, Space.World);

        // Applying the angular velocity to the body if 
        // it is large enough to not be in a resting state.
        if (AngularVelocity.magnitude > mAngularRestingStateCutoff)
            this.transform.Rotate(this.AngularVelocity * Time.deltaTime, Space.World);

        // Reseting the per frame variables.
        mTorqueThisFrame = Vector3.zero;
        mForceThisFrame = Vector3.zero;
        mImpulseThisFrame = Vector3.zero;
        mAngularImpulseThisFrame = Vector3.zero;
    }


    //-------------------------------------Public Functions-------------------------------------

    public void AddFrictionAtPoint(Vector3 force, Vector3 worldPos)
    {
        var linearForceMagnitude = Vector3.Dot(-force.normalized, Velocity.normalized);
        this.mForceThisFrame += force * linearForceMagnitude;

        var torque = Vector3.Cross(worldPos - this.transform.position, force);
        this.mTorqueThisFrame += torque;
    }

    public void ApplyImpulseAtPoint(Vector3 impulse, Vector3 worldPos)
    {
        var linearImpulseMagnitude = Vector3.Dot(-impulse.normalized, (worldPos - this.transform.position).normalized);
        var velChange = impulse * linearImpulseMagnitude / Mass;
        mImpulseThisFrame += velChange;

        var inertiaTensor = CalculateInertiaTensor();
        var inverse = inertiaTensor.inverse;
        var cross = Vector3.Cross(-impulse, worldPos - this.transform.position);
        var angVelChange = inverse.MultiplyVector(cross);
        mAngularImpulseThisFrame += Mathf.Rad2Deg * angVelChange;
    }


    //------------------------------------Private Functions-------------------------------------

    private Matrix4x4 CalculateInertiaTensor()
    {
        Vector3 momentOfInertia = Vector3.one;

        switch (pCollider.pType)
        {
            case ColliderType.Sphere:
                momentOfInertia *= PSI_PhysicsUtils.MomentOfInertiaOfSphere(Mass, ((PSI_Collider_Sphere)pCollider).pRadius);
                break;

            case ColliderType.Box:
                momentOfInertia = PSI_PhysicsUtils.MomentOfInertiaOfBox(Mass, ((PSI_Collider_Box)pCollider).pSize);
                break;

            default:
                return Matrix4x4.identity;
        }

        Matrix4x4 inertiaTensor = new Matrix4x4();
        inertiaTensor.m00 = momentOfInertia.x;
        inertiaTensor.m11 = momentOfInertia.y;
        inertiaTensor.m22 = momentOfInertia.z;
        inertiaTensor.m33 = 1f;
        return inertiaTensor;
    }

    private Vector3 CalculateAngularAcceleration(Vector3 torque)
    {
        switch(pCollider.pType)
        {
            case ColliderType.Sphere:
                return (Mathf.Rad2Deg * torque) * PSI_PhysicsUtils.MomentOfInertiaOfSphere(Mass, ((PSI_Collider_Sphere)pCollider).pRadius);

            case ColliderType.Box:
                Vector3 angAcc = (Mathf.Rad2Deg * torque);
                Vector3 momentOfInertia = PSI_PhysicsUtils.MomentOfInertiaOfBox(Mass, ((PSI_Collider_Box)pCollider).pSize);
                angAcc.x *= momentOfInertia.x;
                angAcc.y *= momentOfInertia.y;
                angAcc.z *= momentOfInertia.z;
                return angAcc;

            default:
                return Vector3.zero;
        }        
    }
}
