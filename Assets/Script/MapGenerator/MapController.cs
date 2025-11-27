using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PCG.Project
{
    [ExecuteAlways]
    public class MapController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private MapData mapProfile;

        private bool isDirty = false;
        private double lastChangeTime;

        private void OnEnable()
        {
            if (mapProfile != null) mapProfile.OnValuesChanged += OnProfileChanged;
        }

        private void OnDisable()
        {
            if (mapProfile != null) mapProfile.OnValuesChanged -= OnProfileChanged;
        }

        void OnProfileChanged()
        {
            if (Application.isPlaying) return;
            isDirty = true;
            lastChangeTime = EditorApplication.timeSinceStartup;

            EditorApplication.QueuePlayerLoopUpdate();
        }

        private void Update()
        {
            if (Application.isPlaying) return;

            if (isDirty && EditorApplication.timeSinceStartup - lastChangeTime > 0.1f)
            {
                GenerateMap();
                isDirty = false;
            }
        }

        public void GenerateMap()
        {
            if (tilemap == null || mapProfile == null) return;

            TileBase[] buffer = Gen_Terrain.Generate(mapProfile, out _, out _, out _, out _);

            Gen_City.Generate(mapProfile, buffer);

            tilemap.ClearAllTiles();
            int resX = mapProfile.width / 2;
            int resY = mapProfile.height / 2;
            tilemap.SetTilesBlock(new BoundsInt(0, 0, 0, resX, resY, 1), buffer);
        }

        public void ClearMap()
        {
            if (tilemap) tilemap.ClearAllTiles();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MapController))]
    public class MapControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            MapController script = (MapController)target;

            GUILayout.Space(10);
            if (GUILayout.Button("Force Generate"))
            {
                script.GenerateMap();
            }
            if (GUILayout.Button("Clear Map"))
            {
                script.ClearMap();
            }
        }
    }
#endif
}