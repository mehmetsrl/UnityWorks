using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mehmetsrl.ComputationalGeometry.Structures;

public class Custom2DShape : MonoBehaviour
{
    public RegularPolygon polygon;
    public Color color = Color.white;
    public IShapeRenderer renderer3D;
    public IShapeRenderer renderer2D;

    // Start is called before the first frame update
    void Start()
    {
        /*
        gameObject.AddComponent<MeshRenderer>();
        gameObject.AddComponent<MeshFilter>();

        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        MeshFilter filter = gameObject.GetComponent<MeshFilter>();

        renderer3D = new ShapeRenderer3D(ref renderer, ref filter);
        triangle = new Triangle(ref renderer3D);
        */
        
        
        gameObject.AddComponent<CanvasRenderer>();
        CanvasRenderer renderer = gameObject.GetComponent<CanvasRenderer>();
        renderer2D = new ShapeRenderer2D(ref renderer);
        polygon = new RegularPolygon(ref renderer2D);
        polygon.Color = color;

        /*
        gameObject.AddComponent<MeshRenderer>();
        gameObject.AddComponent<MeshFilter>();

        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        MeshFilter filter = gameObject.GetComponent<MeshFilter>();

        renderer3D = new ShapeRenderer3D(ref renderer, ref filter);
        shape.InstantiateMesh();
        shape.SetRenderer(ref renderer3D);
        */
    }

    // Update is called once per frame
    void Update()
    {
        polygon.Draw();
    }
}
