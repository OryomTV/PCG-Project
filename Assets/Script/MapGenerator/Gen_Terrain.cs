using UnityEngine;
using UnityEngine.Tilemaps;

namespace PCG.Project
{
    public static class Gen_Terrain
    {
        public static TileBase[] Generate(MapData data, out float offTx, out float offTy, out float offBx, out float offBy)
        {
            int currentSeed = data.useRandomSeed ? Random.Range(-10000, 10000) : data.seed;
            Random.InitState(currentSeed);

            offTx = Random.Range(0f, 9999f);
            offTy = Random.Range(0f, 9999f);
            offBx = Random.Range(0f, 9999f);
            offBy = Random.Range(0f, 9999f);

            int resX = data.width / 2;
            int resY = data.height / 2;
            TileBase[] buffer = new TileBase[resX * resY];

            float invTerrainScale = 1f / data.terrainScale;
            float invBiomeScale = 1f / data.biomeScale;
            float stepSize = 2f;

            for (int y = 0; y < resY; y++)
            {
                float worldY = (y * 2) * stepSize;
                float latitude = worldY / data.height;

                float ty = (worldY + offTy) * invTerrainScale;
                float by = (worldY + offBy) * invBiomeScale;

                for (int x = 0; x < resX; x++)
                {
                    float worldX = (x * 2) * stepSize;
                    float nx = (worldX + offTx) * invTerrainScale;
                    float bx = (worldX + offBx) * invBiomeScale;

                    float terrainNoise = Mathf.PerlinNoise(nx, ty);
                    float biomeNoise = Mathf.PerlinNoise(bx, by);

                    int index = x + y * resX;
                    buffer[index] = ChooseBiomeTile(data, terrainNoise, biomeNoise, latitude);
                }
            }
            return buffer;
        }

        private static TileBase ChooseBiomeTile(MapData d, float terrain, float biome, float latitude)
        {
            float climate = Mathf.Lerp(latitude, biome, 0.5f);

            if (terrain < 0.28f) return d.waterTile;
            if (terrain < 0.32f) return d.sandTile;
            if (terrain >= 0.8f) return d.mountainTile;
            if (terrain >= 0.92f) return d.volcanoTile;

            if (climate < 0.2f) return d.snowTile;
            if (climate < 0.4f) return d.forestTile;
            if (climate < 0.6f) return d.grassTile;
            if (climate < 0.8f) return d.jungleTile;

            return d.desertTile;
        }
    }
}