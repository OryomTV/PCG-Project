using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace PCG.Project
{
    public class ProceduralBiomeGeneratorPolaire : MonoBehaviour
    {
        [Header("Tilemap")]
        [SerializeField] private Tilemap tilemap;

        [Header("Biome Tiles")]
        [SerializeField] private TileBase waterTile;
        [SerializeField] private TileBase sandTile;
        [SerializeField] private TileBase grassTile;
        [SerializeField] private TileBase forestTile;
        [SerializeField] private TileBase snowTile;
        [SerializeField] private TileBase jungleTile;
        [SerializeField] private TileBase desertTile;
        [SerializeField] private TileBase mountainTile;
        [SerializeField] private TileBase volcanoTile;

        [Header("World Settings")]
        [SerializeField, Range(10, 1000)] private int width = 500;
        [SerializeField, Range(10, 1000)] private int height = 500;

        [Header("Noise Settings")]
        [SerializeField, Range(1f, 100f)] private float terrainScale = 20f;
        [SerializeField, Range(1f, 100f)] private float biomeScale = 15f;

        [Header("Seed")]
        [SerializeField] private bool useRandomSeed = false;
        [SerializeField] private int seed = 5;

        private float offsetTerrainX, offsetTerrainY;
        private float offsetBiomeX, offsetBiomeY;

        // Entry Point
        void Start()
        {
            InitializeOffsets();
            GenerateTerrain();
        }

        // Seed & Offsets
        void InitializeOffsets()
        {
            if (useRandomSeed)
                seed = Random.Range(int.MinValue, int.MaxValue);

            Random.InitState(seed);

            offsetTerrainX = Random.Range(0f, 100000f);
            offsetTerrainY = Random.Range(0f, 100000f);
            offsetBiomeX = Random.Range(0f, 100000f);
            offsetBiomeY = Random.Range(0f, 100000f);
        }

        // Full Generation
        void GenerateTerrain()
        { 
            int mapResolutionX = width / 2;
            int mapResolutionY = height / 2;

            TileBase[] buffer = new TileBase[mapResolutionX * mapResolutionY];

            float invTerrainScale = 1f / terrainScale;
            float invBiomeScale = 1f / biomeScale;

            float stepSize = 2f;

            for (int y = 0; y < mapResolutionY; y+=2)
            {
                float worldY = y * stepSize;

                float latitude = worldY / height;

                float ty = (worldY + offsetTerrainY) * invTerrainScale;
                float by = (worldY + offsetBiomeY) * invBiomeScale;

                for (int x = 0; x < mapResolutionX; x+=2)
                {
                    float worldX = x * stepSize;

                    float nx = (worldX + offsetTerrainX) * invTerrainScale;
                    float bx = (worldX + offsetBiomeX) * invBiomeScale;

                    float terrainNoise = Mathf.PerlinNoise(nx, ty);
                    float biomeNoise = Mathf.PerlinNoise(bx, by);

                    int index = x + y * mapResolutionX;
                    buffer[index] = ChooseBiomeTile(terrainNoise, biomeNoise, latitude);
                }
            }

            tilemap.SetTilesBlock(new BoundsInt(0, 0, 0, mapResolutionX, mapResolutionY, 1), buffer);
        }

        // Biome Decision
        TileBase ChooseBiomeTile(float terrain, float biome, float latitude)
        {
            float climate = Mathf.Lerp(latitude, biome, 0.5f);

            if (terrain < 0.28f) return waterTile;
            if (terrain < 0.32f) return sandTile;

            if (terrain >= 0.8f) return mountainTile;
            if (terrain >= 0.92f) return volcanoTile;

            if (climate < 0.2f) return snowTile;
            if (climate < 0.4f) return forestTile;
            if (climate < 0.6f) return grassTile;
            if (climate < 0.8f) return jungleTile;
            return desertTile;
        }
    }
}
