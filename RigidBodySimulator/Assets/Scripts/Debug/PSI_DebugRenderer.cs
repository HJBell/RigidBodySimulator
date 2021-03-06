﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PSI_DebugRenderer : MonoBehaviour {

    [SerializeField]
    private Material DebugMaterial;

    private Stack<Vector3> mSphereCentres = new Stack<Vector3>();
    private Stack<float> mSphereRadii = new Stack<float>();
    private Stack<Vector3> mLineVerts = new Stack<Vector3>();


    //----------------------------------------Unity Functions----------------------------------------

    private void OnPostRender()
    {
        if (!DebugMaterial)
        {
            Debug.LogError("Please Assign a material on the inspector");
            return;
        }
        DebugMaterial.SetPass(0);
        
        // Rendering the wire spheres for this frame.
        while(mSphereCentres.Count > 0)
            DrawGLWireSphere(mSphereCentres.Pop(), mSphereRadii.Pop(), 100);

        // Rendering the lines for this frame.
        while (mLineVerts.Count > 1)
            DrawGLLine(mLineVerts.Pop(), mLineVerts.Pop());
    }


    //----------------------------------------Public Functions---------------------------------------

    public void DrawWireSphere(Vector3 centre, float radius)
    {
        // Adding the sphere to a stack of sphere to be drawn this frame.
        mSphereCentres.Push(centre);
        mSphereRadii.Push(radius+0.005f);
    }

    public void DrawLine(Vector3 start, Vector3 end)
    {
        // Adding the line to a stack of lines to be drawn this frame.
        mLineVerts.Push(start);
        mLineVerts.Push(end);
    }


    //----------------------------------------Private Functions--------------------------------------

    private void DrawGLWireSphere(Vector3 centre, float radius, int circleResolution)
    {
        // Drawing a wire sphere using the Unity GL.
        GL.Begin(GL.LINES);
        for (int i = 0; i < circleResolution-1; i++)
        {
            float theta = ((float)i / (float)(circleResolution - 1)) * 360f;
            float x1 = radius * Mathf.Sin(theta * Mathf.PI / 180);
            float y1 = radius * Mathf.Cos(theta * Mathf.PI / 180);
            theta = ((float)(i+1) / (float)(circleResolution - 1)) * 360f;
            float x2 = radius * Mathf.Sin(theta * Mathf.PI / 180);
            float y2 = radius * Mathf.Cos(theta * Mathf.PI / 180);

            GL.Vertex3(centre.x + x1, centre.y + y1, centre.z);
            GL.Vertex3(centre.x + x2, centre.y + y2, centre.z);
            GL.Vertex3(centre.x, centre.y + y1, centre.z + x1);
            GL.Vertex3(centre.x, centre.y + y2, centre.z + x2);
            GL.Vertex3(centre.x + x1, centre.y, centre.z + y1);
            GL.Vertex3(centre.x + x2, centre.y, centre.z + y2);
        }
        GL.End();
    }

    private void DrawGLLine(Vector3 start, Vector3 end)
    {
        // Drawing a line using Unity GL.
        GL.Begin(GL.LINES);
        GL.Vertex3(start.x, start.y, start.z);
        GL.Vertex3(end.x, end.y, end.z);
        GL.End();
    }
}
