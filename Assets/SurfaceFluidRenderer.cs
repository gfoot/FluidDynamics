using UnityEngine;
using System.Collections;

public class SurfaceFluidRenderer : FluidRenderer
{
    IFluidSimulation _simulation;
    Mesh _mesh;

    public override void Init(IFluidSimulation simulation)
    {
        _simulation = simulation;
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;
        
        int numVerts = _simulation.Width * _simulation.Width;
        int numTris = 2 * (_simulation.Width - 1) * (_simulation.Width - 1);
        
        Vector3[] newVertices = new Vector3[numVerts];
        Vector2[] newUV = new Vector2[numVerts];
        Vector3[] newNormals = new Vector3[numVerts];
        
        int[] indices = new int[numTris * 3];
        
        for (int y = 0; y < _simulation.Width; ++y)
        {
            for (int x = 0; x < _simulation.Width; ++x)
            {
                int vbase = x + y * _simulation.Width;
                newVertices[vbase] = new Vector3(x, 0, y);
                newUV[vbase] = new Vector2(0, 0);
                newNormals[vbase] = new Vector3(0, 1, 0);
            }
        }
        
        for (int y = 0; y < _simulation.Width - 1; ++y)
        {
            for (int x = 0; x < _simulation.Width - 1; ++x)
            {
                int vbase = x + y * _simulation.Width;
                int ibase = 6 * (x + y * (_simulation.Width-1));
                indices[ibase] = vbase;
                indices[ibase + 1] = vbase + _simulation.Width;
                indices[ibase + 2] = vbase + 1;
                indices[ibase + 3] = vbase + _simulation.Width + 1;
                indices[ibase + 4] = vbase + 1;
                indices[ibase + 5] = vbase + _simulation.Width;
            }
        }

        _mesh.vertices = newVertices;
        _mesh.uv = newUV;
        _mesh.normals = newNormals;
        _mesh.triangles = indices;

        _mesh.RecalculateBounds();
    }

    void LateUpdate()
    {
        Vector3[] newVertices = _mesh.vertices;
        Vector3[] newNormals = _mesh.normals;
        
        for (int y = 0; y < _simulation.Width; ++y)
        {
            for (int x = 0; x < _simulation.Width; ++x)
            {
                int vbase = x + y * _simulation.Width;
                
                newVertices[vbase] = new Vector3(x, _simulation.GetHeight(x, y) * 2, y);

                float gradient_x;
                if (x == 0)
                    gradient_x = _simulation.GetHeight(x + 1, y) - _simulation.GetHeight(x, y);
                else if (x == _simulation.Width - 1)
                    gradient_x = _simulation.GetHeight(x, y) - _simulation.GetHeight(x - 1, y);
                else
                    gradient_x = (_simulation.GetHeight(x + 1, y) - _simulation.GetHeight(x, y)) / 2;

                float gradient_y;
                if (y == 0)
                    gradient_y = _simulation.GetHeight(x, y + 1) - _simulation.GetHeight(x, y);
                else if (y == _simulation.Height - 1)
                    gradient_y = _simulation.GetHeight(x, y) - _simulation.GetHeight(x, y - 1);
                else
                    gradient_y = (_simulation.GetHeight(x, y + 1) - _simulation.GetHeight(x, y - 1)) / 2;

                Vector3 normal = new Vector3(-gradient_x * 2, 1.0f, -gradient_y * 2);
                normal.Normalize();

                newNormals[vbase] = normal;
            }
        }
        
        _mesh.vertices = newVertices;
        _mesh.normals = newNormals;
    }
}
