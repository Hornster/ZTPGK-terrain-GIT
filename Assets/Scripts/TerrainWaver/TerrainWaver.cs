using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.NoiseGenerators;
using UnityEngine;

namespace Assets.Scripts.TerrainWaver
{
    public class TerrainWaver : MonoBehaviour
    {
        private Terrain _myTerr;
        private TerrainData _myTerrData;
        private int _xRes;
        private int _yRes;
        /// <summary>
        /// Size of the segment that will be animated on the map.
        /// </summary>
        [SerializeField]
        private Vector2Int _segmentSize;
        /// <summary>
        /// Coords of start of the animation.
        /// </summary>
        private Vector2Int _startCoords;

        /// <summary>
        /// How high the effect can reach.
        /// </summary>
        [SerializeField] private float _scale = 0.1f;

        [SerializeField] private float _speed = 0.1f;

        private float[,] _animatedFragment;
        private float[,] _animatedFragmentBackup;

        void Start()
        {
            // Get terrain and terrain data handles
            _myTerr = GetComponent<Terrain>();
            _myTerrData = _myTerr.terrainData;

            // Get terrain dimensions in tiles (X tiles x Y tiles) 
            _xRes = _myTerrData.heightmapWidth;
            _yRes = _myTerrData.heightmapHeight;

            if (_segmentSize.x < 0)
            {
                _segmentSize.x = -_segmentSize.x;
            }

            if (_segmentSize.y < 0)
            {
                _segmentSize.y = -_segmentSize.y;
            }

            if (_segmentSize.x > _xRes)
            {
                _segmentSize.x = _xRes;
            }

            if (_segmentSize.y > _yRes)
            {
                _segmentSize.y = _yRes;
            }
            _startCoords.x = (_xRes >> 1) - (_segmentSize.x >> 1);
            _startCoords.y = (_yRes >> 1) - (_segmentSize.y >> 1);
            _animatedFragment = _myTerrData.GetHeights(_startCoords.x, _startCoords.y, _segmentSize.x, _segmentSize.y);
            _animatedFragmentBackup = new float[_segmentSize.x, _segmentSize.y];
            CopyMap();
        }

        private void CopyMap()
        {
            for (int x = 0; x < _segmentSize.x; x++)
            {
                for (int y = 0; y < _segmentSize.y; y++)
                {
                    _animatedFragmentBackup[x, y] = _animatedFragment[x, y];
                }
            }
        }

        void Update()
        {
            _animatedFragment = _myTerrData.GetHeights(_startCoords.x, _startCoords.y, _segmentSize.x, _segmentSize.y);
            
            for (int x = 0; x < _segmentSize.x; x++)
            {
                for (int y = 0; y < _segmentSize.y; y++)
                {
                    float value = _animatedFragmentBackup[x, y] + Mathf.Sin(Time.time + x * _speed) * _scale;
                    _animatedFragment[x, y] = value;
                }
            }
            _myTerrData.SetHeights(_startCoords.x, _startCoords.y, _animatedFragment);
        }
    }
}
