using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(CurveLineRenderer))]
public class CurveLineRendererEditor : Editor
{
    private CurveLineRenderer curveLineRenderer;
    private Transform handleTransform;
    private Quaternion handleRotation;
    private MeshFilter handleMeshFilter;

    private float vertexButtonSize = 0.04f;
    private float vertexButtonPickSize = 0.06f;

    private int selectedIndex = -1;

    void OnEnable()
    {
        curveLineRenderer = (CurveLineRenderer)target;

        handleTransform = curveLineRenderer.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local 
            ? handleTransform.rotation : Quaternion.identity;
        handleMeshFilter = curveLineRenderer.GetComponent<MeshFilter>();

        SceneView.onSceneGUIDelegate = OnSceneDraw;
    }

    void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= OnSceneDraw;
    }

    void OnSceneDraw(SceneView sceneView)
    {
        List<Vector3> vertices = curveLineRenderer.vertices;
        Handles.color = Color.white;
        
        Vector3 prev = ShowVertex(0);
        for (int i = 1; i < vertices.Count; ++i)
        {
            Vector3 cur = ShowVertex(i);
            Handles.DrawLine(prev, cur);

            prev = cur;
        }

        if (GUI.changed)
            EditorUtility.SetDirty(target);

        if (handleMeshFilter == null)
            return;

        Mesh mesh = handleMeshFilter.sharedMesh;
        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = "Curve Line Mesh";
            handleMeshFilter.sharedMesh = mesh;
        }
        
        curveLineRenderer.Rebuild(mesh);
    }

    Vector3 ShowVertex(int index)
    {
        Vector3 vertex = handleTransform.TransformPoint(curveLineRenderer.vertices[index]);
        float size = HandleUtility.GetHandleSize(vertex);

        if (Handles.Button(vertex, handleRotation, vertexButtonSize * size, vertexButtonPickSize * size, Handles.DotCap))
        {
            selectedIndex = index;
            Repaint();
        }

        if (selectedIndex != index)
            return vertex;

        EditorGUI.BeginChangeCheck();
        vertex = Handles.DoPositionHandle(vertex, handleRotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(curveLineRenderer, "Move vertex");
            curveLineRenderer.vertices[index] = handleTransform.InverseTransformPoint(vertex);
        }

        return vertex;
    }
}
