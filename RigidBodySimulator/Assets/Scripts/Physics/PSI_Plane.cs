using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSI_Plane {

    public Vector3 Position;
    public Vector2 Dims;

    public Vector3 pNormal { get { return mRotation * Vector3.up; } }
    public float pArea { get { return Dims.x * Dims.y; } }

    private Quaternion mRotation = Quaternion.identity;


    //----------------------------------------Public Functions---------------------------------------

    public PSI_Plane() { }

    public PSI_Plane(Vector3 pos, Quaternion rot, Vector2 dims)
    {
        UpdatePlane(pos, rot, dims);
    }

    public void UpdatePlane(Vector3 pos, Quaternion rot, Vector2 dims)
    {
        Position = pos;
        Dims = dims;
        mRotation = rot;
    }

    public Vector3[] GetVertices()
    {
        Vector3[] vertices = new Vector3[4];
        vertices[0] = Position + (mRotation * new Vector3(Dims.x / 2, 0.0f, Dims.y / 2));
        vertices[1] = Position + (mRotation * new Vector3(Dims.x / 2, 0.0f, -Dims.y / 2));
        vertices[2] = Position + (mRotation * new Vector3(-Dims.x / 2, 0.0f, -Dims.y / 2));
        vertices[3] = Position + (mRotation * new Vector3(-Dims.x / 2, 0.0f, Dims.y / 2));
        return vertices;
    }
}
