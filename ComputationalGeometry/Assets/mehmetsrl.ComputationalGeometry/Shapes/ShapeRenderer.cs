using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace mehmetsrl.ComputationalGeometry.Structures
{
    public interface IShapeRenderer
    {
        void SetMaterial(Material material, Texture texture = null);
        void SetMesh(Mesh mesh);
    }

    public class ShapeRenderer2D : IShapeRenderer
    {
        CanvasRenderer renderer;
        public CanvasRenderer Renderer { get { return renderer; } }

        public ShapeRenderer2D(ref CanvasRenderer renderer)
        {
            this.renderer = renderer;
        }

        public void SetMesh(Mesh mesh)
        {
            renderer.SetMesh(mesh);
        }

        public void SetMaterial(Material material, Texture texture = null)
        {
            renderer.SetMaterial(material, texture);
        }
    }

    public class ShapeRenderer3D : IShapeRenderer
    {
        MeshRenderer renderer;
        MeshFilter filter;

        Material material;
        public MeshRenderer Renderer { get { return renderer; } }

        public ShapeRenderer3D(ref MeshRenderer renderer, ref MeshFilter filter)
        {
            this.renderer = renderer;
            this.filter = filter;
        }

        public void SetMesh(Mesh mesh)
        {
            filter.mesh = mesh;
        }

        public void SetMaterial(Material material, Texture texture = null)
        {
            renderer.material = material;
            renderer.material.mainTexture = texture;
        }

        public static implicit operator MeshRenderer(ShapeRenderer3D renderer)
        {
            return renderer.renderer;
        }
    }

}