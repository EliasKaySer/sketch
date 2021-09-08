using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoManager : MonoBehaviour
{
    // Draws a line from "startVertex" var to the curent mouse position.
    Material mat;
    Vector3 startVertex;
    Vector3 mousePos;

    void Start()
    {
        startVertex = Vector3.zero;
    }

    void Update()
    {
        mousePos = Input.mousePosition;
        // Press space to update startVertex
        if (Input.GetKeyDown(KeyCode.Space))
        {
            startVertex = new Vector3(mousePos.x / Screen.width, mousePos.y / Screen.height, 0);
            Debug.Log(startVertex);
        }
    }

    void OnPostRender()
    {
        if (!mat)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things. In this case, we just want to use
            // a blend mode that inverts destination colors.
            var shader = Shader.Find("Hidden/Internal-Colored");
            mat = new Material(shader);
            mat.hideFlags = HideFlags.HideAndDontSave;
            // Set blend mode to invert destination colors.
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusDstColor);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            // Turn off backface culling, depth writes, depth test.
            mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            mat.SetInt("_ZWrite", 0);
            mat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
        }

        GL.PushMatrix();
        GL.LoadOrtho();

        // activate the first shader pass (in this case we know it is the only pass)
        mat.SetPass(0);
        // draw a quad over whole screen
        GL.Begin(GL.QUADS);
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(1, 0, 0);
        GL.Vertex3(1, 1, 0);
        GL.Vertex3(0, 1, 0);
        GL.End();

        GL.PopMatrix();
    }

    //void OnPostRender()
    //{
    //    if (!mat)
    //    {
    //        Debug.LogError("Please Assign a material on the inspector");
    //        return;
    //    }
    //    Debug.Log("OnPostRender");
    //    GL.PushMatrix();
    //    mat.SetPass(0);
    //    GL.LoadOrtho();

    //    GL.Begin(GL.LINES);
    //    GL.Color(Color.red);
    //    GL.Vertex(startVertex);
    //    GL.Vertex(new Vector3(mousePos.x / Screen.width, mousePos.y / Screen.height, 0));
    //    GL.End();

    //    GL.PopMatrix();
    //}
}

//public class GizmoManager : MonoBehaviour
//{
//    public Material material;
//    public static List<GizmoLine> lines = new List<GizmoLine>();

//    void OnPostRender()
//    {
//        material.SetPass(0); //optional, i have mine set to "Sprites-Default"
//        //OnPostRender method
//        //GL.Begin(GL.LINES);
//        //GL.Color(Color.yellow);
//        //GL.Vertex(new Vector2(0, 0));
//        //GL.Vertex(new Vector2(100, 100));
//        //GL.End();

//        GL.Begin(GL.LINES);
//        for (int i = 0; i < lines.Count; i++)
//        {
//            GL.Color(lines[i].color);
//            GL.Vertex(lines[i].a);
//            GL.Vertex(lines[i].b);
//        }
//        GL.End();
//        lines.Clear();
//    }
//}

//public struct GizmoLine
//{
//    public Vector3 a;
//    public Vector3 b;
//    public Color color;
//    public GizmoLine(Vector3 a, Vector3 b, Color color)
//    {
//        this.a = a;
//        this.b = b;
//        this.color = color;
//    }
//}

//public class GizmosInGame : MonoBehaviour
//{
//    public  Gizmos gizmos = new Gizmos();

//    public static void DrawLine(Vector3 a, Vector3 b, Color color)
//    {
//        //DrawLine method
//        GizmoManager.lines.Add(new GizmoLine(a, b, color));
//    }
//    // Start is called before the first frame update
//    void Start()
//    {
//        gizmos = new Gizmos();
//        //DrawLine method
//        gizmos.DrawLine(a, b, color);
//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }
//}
