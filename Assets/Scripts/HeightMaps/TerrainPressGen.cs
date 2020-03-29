using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.NoiseGenerators;
using UnityEngine;

namespace Assets.Scripts.HeightMaps
{
    [RequireComponent(typeof(DiamondSquareNoiseGen))]
    public class TerrainPressGen : MonoBehaviour
    {
        //Code taken from TerrainAnim class.
        private DiamondSquareNoiseGen _diamondSquareNoiseGen;
        private Terrain _myTerr;
        private TerrainData _myTerrData;
        private float[,] _terrHeights;
        private int _xRes;
        private int _yRes;
        //Code taken from TerrainAnim class.
        void Awake()
        {
            _myTerr = GetComponent<Terrain>();
            _myTerrData = _myTerr.terrainData;
            // Get terrain dimensions in tiles (X tiles x Y tiles) 
            _xRes = _myTerrData.heightmapWidth;
            _yRes = _myTerrData.heightmapHeight;
            _diamondSquareNoiseGen = GetComponent<DiamondSquareNoiseGen>();

            RandomizeTerrain();
        }
        //Code taken from TerrainAnim class.
        private void RandomizeTerrain()
        {
            // Extract entire heightmap (expensive!)
            _terrHeights = _myTerrData.GetHeights(0, 0, _xRes, _yRes);
            ResetTerrainHeights();

            _diamondSquareNoiseGen.GenerateValues(ref _terrHeights, _xRes, _yRes);
            _myTerrData.SetHeights(0, 0, _terrHeights);
        }
        //Code taken from the TerrainAnim class. Written by the author of the exercise.
        private void ResetTerrainHeights()
        {
            for (int i = 0; i < _xRes; i++)
            {
                for (int j = 0; j < _yRes; j++)
                {
                    _terrHeights[i, j] = 0;
                }
            }
        }
    }
}
