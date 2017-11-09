using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSI_CameraController : MonoBehaviour {

    [SerializeField]
    private float MoveSpeed = 1.0f;
    [SerializeField]
    private float ShiftMultiplier = 2.0f;
    [SerializeField]
    private Vector2 MouseSensitivity = Vector2.one;


    //----------------------------------------Unity Functions----------------------------------------

    private void Update()
    {
        // Camera translation.
        Vector3 translation = Vector3.zero;
        translation.x = Input.GetAxis("Horizontal");
        translation.z = Input.GetAxis("Vertical");
        float finalMoveSpeed = (Input.GetKey(KeyCode.LeftShift)) ? MoveSpeed * ShiftMultiplier : MoveSpeed;
        this.transform.Translate(translation.normalized * finalMoveSpeed * Time.deltaTime);

        if(Input.GetMouseButton(1))
        {
            // Camera rotation.
            Vector3 rotation = Vector3.zero;
            rotation.x = Input.GetAxis("Mouse Y") * MouseSensitivity.y;
            rotation.y = Input.GetAxis("Mouse X") * MouseSensitivity.x;
            this.transform.Rotate(rotation);

            // Zeroing camera z rot.
            Vector3 currentRot = this.transform.eulerAngles;
            currentRot.z = 0.0f;
            this.transform.eulerAngles = currentRot;
        }        
    }
}
