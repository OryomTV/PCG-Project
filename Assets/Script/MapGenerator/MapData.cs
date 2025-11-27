using UnityEngine;
using UnityEngine.Tilemaps;

namespace PCG.Project
{
    [CreateAssetMenu(fileName = "NewMapProfile", menuName = "PCG/Map Profile")]
    public class MapData : ScriptableObject
    {
        [Header("Biome Tiles")]
        public TileBase waterTile;
        public TileBase sandTile;
        public TileBase grassTile;
        public TileBase forestTile;
        public TileBase snowTile;
        public TileBase jungleTile;
        public TileBase desertTile;
        public TileBase mountainTile;
        public TileBase volcanoTile;

        [Header("Human Tiles")]
        public TileBase roadTile;

        [Tooltip("Drag and Drop tiles")]
        public TileBase[] cityTiles;

        [Header("World Settings")]
        [Range(10, 2000)] public int width = 200;
        [Range(10, 2000)] public int height = 200;

        [Space]
        [Range(1f, 500f)] public float terrainScale = 20f;
        [Range(1f, 500f)] public float biomeScale = 15f;

        [Header("Human Generation Rules")]
        [Range(0.1f, 0.9f)] public float humanRatio = 0.5f;
        public int maxCitySize = 5;

        [Header("Seed")]
        public bool useRandomSeed = false;
        public int seed = 5;

        public System.Action OnValuesChanged;

        private void OnValidate()
        {
            OnValuesChanged?.Invoke();
        }
    }
}