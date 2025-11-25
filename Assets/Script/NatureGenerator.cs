using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;


namespace PCG.Project
{
    public class ProceduralBiomeGenerator : MonoBehaviour
    {
        #region Variable
        [Header("Tilemap")]
        [SerializeField]
        private Tilemap tilemap;

        [Header("Biomes Tiles")]
        [SerializeField]
        private TileBase waterTile;

        [SerializeField]
        private TileBase sandTile;

        [SerializeField]
        private TileBase grassTile;

        [SerializeField]
        private TileBase forestTile;

        [SerializeField]
        private TileBase mountainTile;

        [SerializeField]
        private TileBase volcanoTile;

        [Header("World Settings")]
        [SerializeField, Range(10, 1000)]
        private int width = 500;

        [SerializeField, Range(10, 1000)]
        private int height = 500;

        [SerializeField, Range(1f, 500f)]
        private float terrainScale = 20f;

        [SerializeField, Range(1f, 500f)]
        private float biomeScale = 15f;

        [Header("Seed")]
        [SerializeField]
        private bool useRandomSeed = false;

        [SerializeField]
        private int seed = 5;

        private float offsetTerrainX, offsetTerrainY;
        private float offsetBiomeX, offsetBiomeY;
        #endregion

        void Start()
        {
            GenerateTerrain();
        }

        #region Time Slicing

        private Coroutine _generationCoroutine;

        private void EditorUpdate()
        {
            if (!Application.isPlaying)
                EditorApplication.QueuePlayerLoopUpdate();
        }

        #endregion

        #region Dirtying handling

        private bool _isDirty;

        private void OnEnable()
        {
            EditorApplication.update += EditorUpdate;
            SetDirty();
        }

        private void OnDisable()
        {
            EditorApplication.update -= EditorUpdate;
            SetDirty();
        }

        private void OnValidate()
        {
            SetDirty();
        }

        private void Reset()
        {
            SetDirty();
        }

        private void SetDirty()
        {
            _isDirty = true;
            if (_generationCoroutine != null)
                StopCoroutine(_generationCoroutine);
        }

        private void Update()
        {
            if (_isDirty)
            {
                //_generationCoroutine = StartCoroutine(GenerateTerrain());
                _isDirty = false;
            }
        }

        #endregion

        private void GenerateTerrain()
        {
            if (useRandomSeed)
            {
                seed = Random.Range(int.MinValue, int.MaxValue);
            }

            Random.InitState(seed);

            offsetTerrainX = Random.Range(0f, 100000f);
            offsetTerrainY = Random.Range(0f, 100000f);
            offsetBiomeX = Random.Range(0f, 100000f);
            offsetBiomeY = Random.Range(0f, 100000f);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float terrainNoise = Mathf.PerlinNoise(
                        x / terrainScale + offsetTerrainX,
                        y / terrainScale + offsetTerrainY
                    );

                    float biomeNoise = Mathf.PerlinNoise(
                        x / biomeScale + offsetBiomeX,
                        y / biomeScale + offsetBiomeY
                    );

                    TileBase tile = ChooseBiomeTile(terrainNoise, biomeNoise);
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }

            _generationCoroutine = null;
        }

        TileBase ChooseBiomeTile(float terrain, float biome)
        {
            if (terrain < 0.3f)
                return waterTile;

            if (terrain < 0.35f)
                return sandTile;

            if (terrain < 0.6f && biome > 0.65f)
                return sandTile;

            if (terrain < 0.6f && biome < 0.4f)
                return forestTile;

            if (terrain < 0.6f)
                return grassTile;

            if (terrain < 0.8f)
                return mountainTile;

            return volcanoTile;
        }


        private void DestroyChildren()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                DestroyImmediate(child.gameObject);
            }
        }

    }


}