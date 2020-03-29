using UnityEngine;

namespace Assets.Scripts
{
    public class RandomizeHeights : MonoBehaviour
    {
        private Terrain terrain;
        private TerrainData tData;

        private int xRes;
        private int yRes;

        private float[,] heights;

        [SerializeField]
        private float WrinklingStrength = 0.5f;

        void Start()
        {
            terrain = transform.GetComponent<Terrain>();
            tData = terrain.terrainData;

            xRes = tData.heightmapWidth;
            yRes = tData.heightmapHeight;
        }

        void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 100, 25), "Wrinkle"))
            {
                RandomizePoints(WrinklingStrength);
            }

            if (GUI.Button(new Rect(10, 40, 100, 25), "Reset"))
            {
                ResetPoints();
            }
        }

        void RandomizePoints(float strength)
        {
            heights = tData.GetHeights(0, 0, xRes, yRes);

            for (int y = 0; y < yRes; y++)
            {
                for (int x = 0; x < xRes; x++)
                {
                    heights[x, y] = Random.Range(0.0f, strength) * 0.5f;
                }
            }
        }

        void ResetPoints()
        {
            var heights = tData.GetHeights(0, 0, xRes, yRes);
            for (int y = 0; y < yRes; y++)
            {
                for (int x = 0; x < xRes; x++)
                {
                    heights[x, y] = 0;
                }
            }

            tData.SetHeights(0, 0, heights);
        }
    }
}