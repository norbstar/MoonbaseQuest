using System;
using System.Collections;
// using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    [SerializeField] int xSize = 10;
    [SerializeField] int zSize = 10;
    [SerializeField] float minHeight = 0f;
    [SerializeField] float maxHeight = 2f;
    [SerializeField] Gradient gradient;

    private Vector3[] verticies;
    private int[] triangles;
    private Vector2[] uvs;
    private Color[] colors;

    // [SerializeField] List<Vector3> verticies;
    // [SerializeField] List<int> triangles;

    private Mesh mesh;

    void Awake() => mesh = new Mesh();

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.name = "Custom Terrain";

        // StartCoroutine(DefineMeshCoroutine());

        DefineMesh();
        UpdateMesh();
    }

    // Update is called once per frame
    // void Update() => UpdateMesh();

    private IEnumerator DefineMeshCoroutine()
    {
        verticies = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int itr = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = GetNoiseSample(x, z);
                verticies[itr] = new Vector3(x, y, z);
                ++itr;
            }
        }

        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris +1] = vert + xSize + 1;
                triangles[tris +2] = vert + 1;

                triangles[tris +3] = vert + 1;
                triangles[tris +4] = vert + xSize + 1;
                triangles[tris +5] = vert + xSize + 2;

                ++vert;
                tris += 6;
            
                yield return new WaitForSeconds(0.05f);
            }

            ++vert;
        }
    }

    private void DefineMesh()
    {
#if false
        verticies = new Vector3[]
        {
            new Vector3 (0, 0, 0),
            new Vector3 (0, 0, 1),
            new Vector3 (1, 0, 0),
            new Vector3 (1, 0, 1)
        };

        triangles = new int[]
        {
            0, 1, 2,
            1, 3, 2
        };
#endif

#if false
        verticies = new List<Vector3>();
        triangles = new List<int>();

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                List<Vector3> quadVerticies = new List<Vector3>();
                quadVerticies.Add(CreateVertex(x, z));
                quadVerticies.Add(CreateVertex(x, z + 1));
                quadVerticies.Add(CreateVertex(x + 1, z));
                quadVerticies.Add(CreateVertex(x + 1, z + 1));

                var lastIdx = verticies.Count;

                List<int> quadTriangles = new List<int>
                {
                    lastIdx, lastIdx + 1, lastIdx + 2,
                    lastIdx + 1, lastIdx + 3, lastIdx + 2
                };

                verticies.AddRange(quadVerticies);
                triangles.AddRange(quadTriangles);
            }
        }    
#endif

        verticies = new Vector3[(xSize + 1) * (zSize + 1)];

        // int[] quads = new int[xSize * zSize];

        for (int itr = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = GetNoiseSample(x, z);
                y = Mathf.Clamp(y, minHeight, maxHeight);
                verticies[itr] = new Vector3(x, y, z);
                ++itr;
            }
        }

        // triangles = new int[6];
        // triangles[0] = 0;
        // triangles[1] = xSize + 1;
        // triangles[2] = 1;

        // triangles[3] = 1;
        // triangles[4] = xSize + 1;
        // triangles[5] = xSize + 2;

        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris +1] = vert + xSize + 1;
                triangles[tris +2] = vert + 1;

                triangles[tris +3] = vert + 1;
                triangles[tris +4] = vert + xSize + 1;
                triangles[tris +5] = vert + xSize + 2;

                ++vert;
                tris += 6;
            }

            ++vert;
        }

        uvs = new Vector2[verticies.Length];

        for (int itr = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                uvs[itr] = new Vector2((float) x / xSize, (float) z / zSize);
                ++itr;
            }
        }

        colors = new Color[verticies.Length];

        for (int itr = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = verticies[itr].y;
                float normalizedValue = (height - 0f) / (2f - 0f);
                // colors[itr] = gradient.Evaluate((height - minHeight) / (maxHeight - minHeight));
                colors[itr] = gradient.Evaluate(Mathf.InverseLerp(minHeight, maxHeight, height));
                ++itr;
            }
        }
    }

    private float GetNoiseSample(float x, float z) => Mathf.PerlinNoise(x * 0.3f, z * 0.3f) * 2f;
    // private float GetNoiseSample(float x, float z) => Mathf.Clamp(Mathf.PerlinNoise(x * 0.3f, z * 0.3f) * 2f, 0f, 2f);

    private Vector3 CreateVertex(int x, int z)
    {
        try
        {
            var vertex = verticies.First(v => v.x == x && v.z == z);
            return new Vector3(x, vertex.y, z);
        }
        catch (InvalidOperationException ex)
        {
            float y = Mathf.PerlinNoise(x * 0.3f, z * 0.3f) * 2f;
            return new Vector3(x, y, z);
        }
    }

    private void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = verticies.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs;
        mesh.colors = colors;

        mesh.RecalculateNormals();
    }

    // private void OnDrawGizmos()
    // {
    //     if (verticies.Length == 0) return;

    //     for (int itr = 0; itr < verticies.Length; itr++)
    //     {
    //         Gizmos.DrawSphere(verticies[itr], 0.1f);
    //     }
    // }
}