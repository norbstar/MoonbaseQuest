using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] int width = 256;
    [SerializeField] int height = 256;
    [SerializeField] int depth = 20;
    [SerializeField] float scale = 20f;
    [SerializeField] float offsetX = 100f;
    [SerializeField] float offsetY = 100f;
    [SerializeField] float speed = 10f;

    private Terrain terrain;

    void Awake() => ResolveDependencies();

    private void ResolveDependencies() => terrain = GetComponent<Terrain>();

    private void Start()
    {
        offsetX = Random.Range(0f, 9999f);
        offsetY = Random.Range(0f, 9999f);
    }

    private void Update()
    {
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
        offsetX += Time.deltaTime * speed;
    }

    private TerrainData GenerateTerrain (TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);
        terrainData.SetHeights(0, 0, GenerateHeights());

        return terrainData;
    }

    private float[,] GenerateHeights()
    {
        float[,] heights = new float[width, height];
        
        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = CalculateHeight(x, y);
            }
        }

        return heights;
    }

    private float CalculateHeight (int x, int y)
    {
        float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }
}