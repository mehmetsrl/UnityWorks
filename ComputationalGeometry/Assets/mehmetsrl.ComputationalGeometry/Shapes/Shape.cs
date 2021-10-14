using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mehmetsrl.ComputationalGeometry.Structures {
    public interface IShape
    {
        Vector GetCenter();
        /*
        Mesh GetMesh();
        Material GetMaterial();
        void SetMaterial(ref Material material);
        Texture GetTexture();
        void SetTexture(ref Texture texture);
        */

        void SetRenderer(ref IShapeRenderer renderer);
        void Draw();
        void InstantiateMesh();
    }

    [System.Serializable]
    public abstract class Shape : IShape
    {
        [SerializeField]
        float scale = 1.0f;
        public Color Color
        {
            get { return color; }
            set { color = value; Material.color = color; }
        }

        [SerializeField]
        private Color color = Color.white;


        [SerializeField]
        protected List<Point> points;
        [SerializeField]
        protected List<Edge> edges;

        [SerializeField]
        protected Vector2[] uv;
        [SerializeField]
        protected int[] triangles;

        protected Vector3[] scaledVerticies;

        protected IShapeRenderer renderer;

        Mesh mesh;
        Material material;
        Texture texture;

        #region Accesors
        public Mesh Mesh
        {
            protected set { mesh = value; }
            get { return mesh; }
        }
        public Material Material
        {
            set { 
                material = value;
                renderer.SetMaterial(material);
            }
            get { 
                if(material == null)
                {
                    material = new Material(Shader.Find("Unlit/Color"));
                    material.color = Color.black;
                    material.mainTexture = null;
                }
                return material; 
            }
        }

        public Texture Texture
        {
            set
            {
                texture = value;
                renderer.SetMaterial(material, texture);
            }
            get { return texture; }
        }

        public float Scale
        {
            set{scale = value;}
            get { return scale; }
        }
        #endregion


        #region interfaceImplementations

        public Vector GetCenter()
        {
            Vector centerSum = Vector.zero2D;
            foreach (var p in points)
            {
                centerSum += p.Position;
            }
            return centerSum / points.Count;
        }
        #endregion


        public Shape(ref IShapeRenderer renderer) {
            InitShape();
            this.renderer = renderer;
            mesh = new Mesh();

            scaledVerticies = new Vector3[points.Count];
        }


        #region Methods ChildrensShouldImplement
        protected virtual void UpdateMesh() {

            for (int i = 0; i < points.Count; i++)
            {
                scaledVerticies[i] = new Vector3(points[i].x * Scale, points[i].y * Scale, points[i].z * Scale);
            }

            Mesh.vertices = scaledVerticies;
            Mesh.uv = uv;
            Mesh.triangles = triangles;
            Mesh.RecalculateNormals();
        }
        public abstract float GetArea();
        #endregion

        public void SetRenderer(ref IShapeRenderer renderer)
        {
            this.renderer = renderer;
        }

        public abstract void Draw();
        public abstract void InitShape();

        public void InstantiateMesh()
        {
            if (mesh != null)
                mesh.Clear();
            mesh = new Mesh();
        }
    }


    public abstract class Shape2D : Shape
    {
        public Shape2D(ref IShapeRenderer renderer):base(ref renderer){
            renderer.SetMaterial(Material);
            Draw();
        }

        LinkedList<Vector> hitPointList;
        public bool CheckHit(Vector rayStart,Vector rayEnd,out List<Vector> hitPoints)
        {
            hitPointList.Clear();
            
            foreach (var e in edges)
            {
                Vector v;
                if (Utils.CheckLineIntersection(e,rayStart,rayEnd,out v))
                    hitPointList.AddLast(v);
            }
            hitPoints = new List<Vector>(hitPointList);
            return hitPoints.Count > 0;
        }

        public override void Draw()
        {
            UpdateMesh();
            renderer.SetMesh(Mesh);
        }

    }



}