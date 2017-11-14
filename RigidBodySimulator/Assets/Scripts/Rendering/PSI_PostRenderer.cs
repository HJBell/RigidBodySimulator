using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PSI_PostRenderer : MonoBehaviour {

    public Transform TestObject;

    [SerializeField]
    private Material DebugMaterial;

    private Stack<Vector3> mLinePoints = new Stack<Vector3>();
    private Stack<Vector3> mCircleCentres = new Stack<Vector3>();
    private Stack<float> mCircleRadii = new Stack<float>();


    //----------------------------------------Unity Functions----------------------------------------

    private void Update()
    {
        DrawCircle(TestObject.position, TestObject.lossyScale.x / 2f);
        DrawLine(TestObject.position, TestObject.position + Vector3.up * TestObject.lossyScale.y/2f);
    }

    private void OnPostRender()
    {
        if (!DebugMaterial)
        {
            Debug.LogError("Post renderer missing material");
            return;
        }
        DebugMaterial.SetPass(0);

        int lineCount = mLinePoints.Count / 2;
        for (int i = 0; i < lineCount; i++)
        {
            GL.Begin(GL.LINES);
            GL.Vertex(mLinePoints.Pop());
            GL.Vertex(mLinePoints.Pop());
            GL.End();
        }

        int circleCount = mCircleCentres.Count;
        for (int i = 0; i < circleCount; i++)
        {
            var centre = mCircleCentres.Pop();
            var radius = mCircleRadii.Pop();
            GL.Begin(GL.LINES);
            for (float theta = 0.0f; theta < (2 * Mathf.PI); theta += 0.01f)
            {
                Vector3 ci = (new Vector3(Mathf.Cos(theta) * radius + centre.x, Mathf.Sin(theta) * radius + centre.y, centre.z));
                GL.Vertex3(ci.x, ci.y, ci.z);
            }
            GL.End();
            GL.Begin(GL.LINES);
            for (float theta = 0.0f; theta < (2 * Mathf.PI); theta += 0.01f)
            {
                Vector3 ci = (new Vector3(centre.x, Mathf.Sin(theta) * radius + centre.y, Mathf.Cos(theta) * radius + centre.z));
                GL.Vertex3(ci.x, ci.y, ci.z);
            }
            GL.End();
        }
    }


    //----------------------------------------Public Functions---------------------------------------

    public void DrawLine(Vector3 start, Vector3 end)
    {
        mLinePoints.Push(start);
        mLinePoints.Push(end);
    }

    public void DrawCircle(Vector3 centre, float radius)
    {
        mCircleCentres.Push(centre);
        mCircleRadii.Push(radius);
    }
}
