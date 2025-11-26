using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace PCG.Project
{
    public class ProceduralBiomeGenerator : MonoBehaviour
    {
        [Header("Tilemap")]
        [SerializeField] private Tilemap tilemap;

        [Header("Biome Tiles")]
        [SerializeField] private TileBase waterTile;
        [SerializeField] private TileBase sandTile;
        [SerializeField] private TileBase grassTile;
        [SerializeField] private TileBase forestTile;
        [SerializeField] private TileBase mountainTile;
        [SerializeField] private TileBase volcanoTile;

        [Header("World Settings")]
        [SerializeField, Range(10, 1000)] private int width = 500;
        [SerializeField, Range(10, 1000)] private int height = 500;

        [Header("Noise")]
        [SerializeField, Range(1f, 500f)] private float terrainScale = 20f;
        [SerializeField, Range(1f, 500f)] private float biomeScale = 15f;

        [Header("Seed")]
        [SerializeField] private bool useRandomSeed = false;
        [SerializeField] private int seed = 5;

        float offsetTerrainX, offsetTerrainY;
        float offsetBiomeX, offsetBiomeY;

        void Start()
        {
            GenerateTerrainOptimized();
        }

        private void GenerateTerrainOptimized()
        {
            if (useRandomSeed)
                seed = Random.Range(int.MinValue, int.MaxValue);

            Random.InitState(seed);

            offsetTerrainX = Random.Range(0f, 100000f);
            offsetTerrainY = Random.Range(0f, 100000f);
            offsetBiomeX = Random.Range(0f, 100000f);
            offsetBiomeY = Random.Range(0f, 100000f);

            float invTerrainScale = 1f / terrainScale;
            float invBiomeScale = 1f / biomeScale;

            TileBase[] buffer = new TileBase[width * height];
            int i = 0;

            for (int y = 0; y < height; y++)
            {
                float ty = (y + offsetTerrainY) * invTerrainScale;
                float by = (y + offsetBiomeY) * invBiomeScale;

                for (int x = 0; x < width; x++)
                {
                    float terrainNoise = Mathf.PerlinNoise(
                        (x + offsetTerrainX) * invTerrainScale,
                        ty
                    );

                    float biomeNoise = Mathf.PerlinNoise(
                        (x + offsetBiomeX) * invBiomeScale,
                        by
                    );

                    buffer[i++] = ChooseBiomeTile(terrainNoise, biomeNoise);
                }
            }

            tilemap.SetTilesBlock(
                new BoundsInt(0, 0, 0, width, height, 1),
                buffer
            );
        }

        TileBase ChooseBiomeTile(float terrain, float biome)
        {
            if (terrain < 0.3f) return waterTile;
            if (terrain < 0.35f) return sandTile;
            if (terrain < 0.6f && biome > 0.65f) return sandTile;
            if (terrain < 0.6f && biome < 0.4f) return forestTile;
            if (terrain < 0.6f) return grassTile;
            if (terrain < 0.8f) return mountainTile;
            return volcanoTile;
        }
    }
}
