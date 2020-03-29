using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.HeightMaps
{
    /// <summary>
    /// Class used to "print" data from graphics files onto the ground.
    /// </summary>
    [RequireComponent(typeof(HeightMap))]
    public class GroundPress : MonoBehaviour
    {
        private Terrain _terrain;
        private HeightMap _heightMap;
        /// <summary>
        /// Path to graphics to load.
        /// </summary>
        [SerializeField]
        private string _graphicsPath;

        [SerializeField] private float _weight = 0.1f;
        void Start()
        {
            _terrain = GetComponent<Terrain>();

            if (_terrain == null)
            {
                throw new Exception("Error - terrain component not found.");
            }

            _heightMap = GetComponent<HeightMap>();

            if (_heightMap == null)
            {
                throw new Exception("Error - HeightMap not found!");
            }

            _heightMap.ReadFile(_graphicsPath);
            ImprintShape();
        }

        private void ImprintShape()
        {
            var terrData = _terrain.terrainData;
            var heightMapData = _heightMap.GetHeightMap().ToArray();
            //Start and end width position on applying the height map to the terrain.
            (int startWidth, int endWidth) = CalcBeginningPos(terrData.heightmapWidth, _heightMap.Width);
            //Start and end height position on applying the height map to the terrain.
            (int startHeight, int endHeight) = CalcBeginningPos(terrData.heightmapHeight, _heightMap.Height);

            int heightMapWidth = endWidth - startWidth;
            int heightMapHeight = endHeight - startHeight;
            
            var terrHeights = terrData.GetHeights(startWidth, startHeight, heightMapWidth, heightMapHeight);

            for (int y = 0; y < heightMapHeight; y++)
            {
                for (int x = 0; x < heightMapWidth; x++)
                {
                    terrHeights[y, x] += heightMapData[y][x] * _weight;
                }
            }

            terrData.SetHeights(startWidth, startHeight, terrHeights);
        }

        private (int start, int end) CalcBeginningPos(int maxMapSize, int maxHeightMapSize)
        {
            if (maxMapSize < maxHeightMapSize)
            {
                return (0, maxMapSize);
            }
            else
            {
                var startValue = (int) Mathf.Floor((maxMapSize - maxHeightMapSize) * 0.5f);
                return (startValue, startValue + maxHeightMapSize);
            }
        }
    }
}
