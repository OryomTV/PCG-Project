using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace PCG.Project
{
    public static class Gen_City
    {
        public static void Generate(MapData data, TileBase[] buffer)
        {
            if (data.cityTiles == null || data.cityTiles.Length == 0)
            {
                Debug.LogError("Attention : Aucun tile de bâtiment assigné dans le Map Profile !");
                return;
            }

            int resX = data.width / 2;
            int resY = data.height / 2;
            int totalTiles = resX * resY;
            int targetHumanTiles = Mathf.FloorToInt(totalTiles * data.humanRatio);
            int currentHumanTiles = 0;

            List<Vector2Int> connectionPoints = new List<Vector2Int>();

            int startX = resX / 2;
            int startY = resY / 2;
            int startSize = Random.Range(3, 6);
            currentHumanTiles += PlaceCityAndRegisterPoints(data, buffer, resX, resY, startX, startY, startSize, connectionPoints);

            int safetyLoop = 0;
            int maxIterations = 200000;

            while (currentHumanTiles < targetHumanTiles && connectionPoints.Count > 0 && safetyLoop < maxIterations)
            {
                safetyLoop++;
                int randIndex = Random.Range(0, connectionPoints.Count);
                Vector2Int startPoint = connectionPoints[randIndex];
                int direction = Random.Range(0, 4);

                int roadLength = Random.Range(3, 15);
                int citySize = Random.Range(2, data.maxCitySize);

                if (AttemptBuildBranch(data, buffer, resX, resY, startPoint, direction, roadLength, citySize, connectionPoints))
                {
                    currentHumanTiles = CountHumanTiles(data, buffer);
                }
            }
        }


        static bool AttemptBuildBranch(MapData d, TileBase[] buffer, int w, int h, Vector2Int start, int dir, int rLen, int cSize, List<Vector2Int> connectors)
        {
            Vector2Int roadEnd = start;
            Vector2Int dirVec = GetDirVector(dir);

            for (int i = 1; i <= rLen; i++)
            {
                Vector2Int pos = start + (dirVec * i);
                if (!IsValidEmpty(d, buffer, w, h, pos.x, pos.y)) return false;
                roadEnd = pos;
            }

            Vector2Int cityCenter = roadEnd + (dirVec * (cSize + 1));
            if (!CanPlaceCity(d, buffer, w, h, cityCenter.x, cityCenter.y, cSize)) return false;

            for (int i = 1; i <= rLen; i++)
            {
                Vector2Int pos = start + (dirVec * i);
                buffer[pos.x + pos.y * w] = d.roadTile;
                if (Random.value > 0.7f) connectors.Add(pos);
            }

            PlaceCityAndRegisterPoints(d, buffer, w, h, cityCenter.x, cityCenter.y, cSize, connectors);
            return true;
        }

        static int PlaceCityAndRegisterPoints(MapData d, TileBase[] buffer, int w, int h, int cx, int cy, int size, List<Vector2Int> connectors)
        {
            int count = 0;
            Vector2 center = new Vector2(cx, cy);
            float maxDist = size * 1.2f;

            for (int x = cx - size; x <= cx + size; x++)
            {
                for (int y = cy - size; y <= cy + size; y++)
                {
                    if (!IsInside(x, y, w, h)) continue;
                    int idx = x + y * w;

                    if (!IsHuman(d, buffer[idx]))
                    {
                        float dist = Vector2.Distance(new Vector2(x, y), center);

                        float t = Mathf.Clamp01(dist / maxDist);

                        float density = 1f - t;

                        float randomNoise = Random.Range(-0.2f, 0.2f);
                        float finalVal = Mathf.Clamp01(density + randomNoise);

                        int buildingIndex = Mathf.FloorToInt(finalVal * (d.cityTiles.Length - 1));

                        buildingIndex = Mathf.Clamp(buildingIndex, 0, d.cityTiles.Length - 1);

                        buffer[idx] = d.cityTiles[buildingIndex];
                        count++;
                    }

                    if (x == cx - size || x == cx + size || y == cy - size || y == cy + size)
                        connectors.Add(new Vector2Int(x, y));
                }
            }
            return count;
        }

        static bool CanPlaceCity(MapData d, TileBase[] buffer, int w, int h, int cx, int cy, int size)
        {
            for (int x = cx - size - 1; x <= cx + size + 1; x++)
            {
                for (int y = cy - size - 1; y <= cy + size + 1; y++)
                {
                    if (!IsInside(x, y, w, h)) return false;
                    int idx = x + y * w;
                    if (buffer[idx] == d.waterTile || IsHuman(d, buffer[idx])) return false;
                }
            }
            return true;
        }

        static bool IsValidEmpty(MapData d, TileBase[] buffer, int w, int h, int x, int y)
        {
            if (!IsInside(x, y, w, h)) return false;
            int idx = x + y * w;
            return buffer[idx] != d.waterTile && !IsHuman(d, buffer[idx]);
        }

        static bool IsHuman(MapData d, TileBase t)
        {
            if (t == null) return false;
            if (t == d.roadTile) return true;

            for (int i = 0; i < d.cityTiles.Length; i++)
            {
                if (t == d.cityTiles[i]) return true;
            }
            return false;
        }

        static bool IsInside(int x, int y, int w, int h) => x >= 0 && x < w && y >= 0 && y < h;

        static int CountHumanTiles(MapData d, TileBase[] b)
        {
            int c = 0;
            foreach (var t in b) if (IsHuman(d, t)) c++;
            return c;
        }

        static Vector2Int GetDirVector(int dir)
        {
            if (dir == 0) return new Vector2Int(0, 1);
            if (dir == 1) return new Vector2Int(0, -1);
            if (dir == 2) return new Vector2Int(-1, 0);
            return new Vector2Int(1, 0);
        }
    }
}