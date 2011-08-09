using UnityEngine;
using System.Collections;

public class SurfaceFluidRenderer : FluidRenderer
{
    FluidSimulation _simulation;
    Mesh _mesh;

    public override void Init(FluidSimulation simulation)
    {
        _simulation = simulation;
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;

        int numVerts = 2 * _simulation.Width;
        int numTris = 2 * (_simulation.Width - 1);

        Vector3[] newVertices = new Vector3[numVerts];
        Vector2[] newUV = new Vector2[numVerts];
        Vector3[] newNormals = new Vector3[numVerts];

        int[] indices = new int[numTris*3];

        for (int i = 0; i < numVerts; ++i)
        {
            newVertices[i] = new Vector3(0, 0, 0);
            newUV[i] = new Vector2(0, 0);
            newNormals[i] = new Vector3(0, 1, 0);
        }

        for (int i = 0; i < _simulation.Width-1; ++i)
        {
            int vbase = 2*i;
            int ibase = 6*i;
            indices[ibase] = vbase;
            indices[ibase+1] = vbase+1;
            indices[ibase+2] = vbase+2;
            indices[ibase+3] = vbase+3;
            indices[ibase+4] = vbase+2;
            indices[ibase+5] = vbase+1;
        }

        _mesh.vertices = newVertices;
        _mesh.uv = newUV;
        _mesh.normals = newNormals;
        _mesh.triangles = indices;
    }

    void LateUpdate()
    {
        Vector3[] newVertices = _mesh.vertices;
        Vector3[] newNormals = _mesh.normals;

        for (int i = 0; i < _simulation.Width; ++i)
        {
            int vbase = 2*i;

            newVertices[vbase] = new Vector3(i, _simulation.GetHeight(i) * 2, 0);
            newVertices[vbase+1] = newVertices[vbase] + new Vector3(0, 0, 100);

            float gradient;
            if (i == 0)
                gradient = _simulation.GetHeight(i+1) - _simulation.GetHeight(i);
            else if (i == _simulation.Width - 1)
                gradient = _simulation.GetHeight(i) - _simulation.GetHeight(i-1);
            else
                gradient = (_simulation.GetHeight(i+1) - _simulation.GetHeight(i)) / 2;

            Vector3 normal = new Vector3(-gradient*2, 1.0f, 0);
            normal.Normalize();

            newNormals[vbase] = newNormals[vbase+1] = normal;
        }

        _mesh.vertices = newVertices;
        _mesh.normals = newNormals;
	}
}
