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

    private Vector3 mForceThisFrame = Vector3.zero;
    private Vector3 mTorqueThisFrame = Vector3.zero;

    private PSI_Collider pCollider { get { return GetComponent<PSI_Collider>(); } }


    //--------------------------------------Unity Functions--------------------------------------

    private void Update()
    {
        // Applying gravity if necessary.
        if (this.UseGravity) AddForce(Vector3.down * Mass * 9.81f);

        // Applying the torque that the body received this frame.
        float momentOfIntertia = CalculateMomentOfIntetia();
        var angularAcceleration = mTorqueThisFrame / momentOfIntertia;
        AngularVelocity += angularAcceleration * Time.deltaTime;
        mTorqueThisFrame = Vector3.zero;

        // Applying the force applied to the body this frame.
        var acceleration = this.mForceThisFrame / this.Mass;
        this.Velocity += acceleration * Time.deltaTime;
        this.mForceThisFrame = Vector3.zero;

        // Moving the rigidbody based on its velocity and angular velocity.
        this.transform.Rotate(this.AngularVelocity * Time.deltaTime, Space.World);
        this.transform.Translate(this.Velocity * Time.deltaTime, Space.World);       
    }


    //-------------------------------------Public Functions-------------------------------------

    public void AddForce(Vector3 force)
    {
        this.mForceThisFrame += force;
    }

    public void AddForceAtPoint(Vector3 force, Vector3 point)
    {
        if (!pCollider) return;
        var torque = Vector3.Cross(point - this.transform.position, force);
        this.mTorqueThisFrame += torque;
    }

    public void ApplyImpulse(Vector3 impulse)
    {
        var velChange = impulse / Mass;
        Velocity += velChange;
    }

    public void ApplyAngularImpulse(Vector3 impulse, Vector3 worldPos)
    {
        //var inertiaTensor = CalculateInertiaTensor();
        //var inverse = inertiaTensor.inverse;
        //var cross = Vector3.Cross(impulse, worldPos - this.transform.position);
        //var velChange = inverse.MultiplyVector(cross);
        //AngularVelocity += velChange;
    }


    //------------------------------------Private Functions-------------------------------------

    private float CalculateMomentOfIntetia()
    {
        if (!pCollider) return 1f;

        // Sphere
        if (pCollider.pType == ColliderType.Sphere)
            return (2f / 5f) * Mass * Mathf.Pow(((PSI_Collider_Sphere)pCollider).pRadius, 2);

        return 1f;
    }

    private Matrix4x4 CalculateInertiaTensor()
    {
        Matrix4x4 inertiaTensor = new Matrix4x4();
        inertiaTensor.m00 = CalculateMomentOfIntetia();
        inertiaTensor.m11 = CalculateMomentOfIntetia();
        inertiaTensor.m22 = CalculateMomentOfIntetia();
        inertiaTensor.m33 = 1f;
        return inertiaTensor;
    }
}
