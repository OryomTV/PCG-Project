using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace PCG.Project
{
    public class test : MonoBehaviour
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

        void InitializeOffsets()
        {
            if (useRandomSeed)
                seed = Random.Range(int.MinValue, int.MaxValue);

            Random.InitState(seed);

            offsetTerrainX = Random.value * 100000f;
            offsetTerrainY = Random.value * 100000f;
            offsetBiomeX = Random.value * 100000f;
            offsetBiomeY = Random.value * 100000f;
        }

        void GenerateTerrain()
        {
            TileBase[] buffer = new TileBase[width * height];

            // Pre-calc scales
            float invTerrainScale = 1f / terrainScale;
            float invBiomeScale = 1f / biomeScale;
            float invHeight = 1f / height;

            int index = 0;

            for (int y = 0; y < height; y += 2)
            {
                float latitude = y * invHeight;

                float ty = (y + offsetTerrainY) * invTerrainScale;
                float by = (y + offsetBiomeY) * invBiomeScale;

                for (int x = 0; x < width; x += 2)
                {
                    float nx = (x + offsetTerrainX) * invTerrainScale;
                    float bx = (x + offsetBiomeX) * invBiomeScale;

                    float terrainNoise = Mathf.PerlinNoise(nx, ty);
                    float biomeNoise = Mathf.PerlinNoise(bx, by);

                    index = x + y * width;
                    buffer[index] = ChooseBiomeTile(terrainNoise, biomeNoise, latitude);
                }
            }



            tilemap.SetTilesBlock(new BoundsInt(0, 0, 0, width, height, 1), buffer);
        }

        TileBase ChooseBiomeTile(float terrain, float biome, float latitude)
        {
            float climate = (latitude + biome) * 0.5f;

            // Relief first
            if (terrain < 0.28f) return waterTile;
            if (terrain < 0.32f) return sandTile;
            if (terrain >= 0.92f) return volcanoTile;
            if (terrain >= 0.8f) return mountainTile;

            // Climate second
            if (climate < 0.2f) return snowTile;
            if (climate < 0.4f) return forestTile;
            if (climate < 0.6f) return grassTile;
            if (climate < 0.8f) return jungleTile;
            return desertTile;
        }
    }
}
