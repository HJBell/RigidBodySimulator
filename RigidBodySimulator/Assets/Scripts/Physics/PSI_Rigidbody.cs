using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSI_Rigidbody : MonoBehaviour {

    public float pMass { get { return Mass; } }
    public float pCoeffOfRest { get { return CoeffOfRest; } }

    public Vector3 Velocity = Vector3.zero;
    public Vector3 AngularVelocity = Vector3.zero;

    [SerializeField]
    [Range(1.0f, 20.0f)]
    private float Mass = 1.0f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    [Tooltip("The coefficient of restitution for the body")]
    private float CoeffOfRest = 0.5f;

    [SerializeField]
    private bool UseGravity = true;

    private Vector3 mSumForce = Vector3.zero;


    //--------------------------------------Unity Functions--------------------------------------

    private void Update()
    {
        // Applying angular movement.
        this.transform.Rotate(this.AngularVelocity * Time.deltaTime);

        // Applying linear movement.
        var acceleration = this.mSumForce / this.Mass;
        this.Velocity += acceleration * Time.deltaTime;
        if (this.UseGravity) this.Velocity.y += Physics.gravity.y * Time.deltaTime;
        this.transform.Translate(this.Velocity * Time.deltaTime, Space.World);

        // Reseting variables.
        this.mSumForce = Vector3.zero;
    }


    //-------------------------------------Public Functions-------------------------------------

    public void AddForce(Vector3 force)
    {
        this.mSumForce += force;
    }
}
