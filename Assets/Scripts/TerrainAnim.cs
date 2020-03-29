using Assets.Scripts.NoiseGenerators;
using System;
using UnityEngine;

//! Sample terrain animator/generator
namespace Assets.Scripts
{
    public class TerrainAnim : MonoBehaviour
    {
        [SerializeField] private DiamondSquareNoiseGen _diamondSquareNoiseGen;
        private Terrain _myTerr;
        private TerrainData _myTerrData;
        private int _xRes;
        private int _yRes;
        private float[,] _terrHeights;
        float[,] originalTerrainSectionHeight;
        public int numberOfPasses = 5;
        public int radiusOfAnimation = 50;
        public float scaleMin = 4.0f;
        public float scaleMax = 12.0f;
        /// <summary>
        /// Weight of this generator's results.
        /// </summary>
        public float weight = 1.0f;


        void Start()
        {
            if (_diamondSquareNoiseGen == null)
            {
                throw new Exception($"DiamondSquareNoiseGen not found! Attach the component to {this}.");
            }
            // Get terrain and terrain data handles
            _myTerr = GetComponent<Terrain>();
            _diamondSquareNoiseGen = GetComponent<DiamondSquareNoiseGen>();
            _myTerrData = _myTerr.terrainData;

            // Get terrain dimensions in tiles (X tiles x Y tiles) 
            _xRes = _myTerrData.heightmapWidth;
            _yRes = _myTerrData.heightmapHeight;

            // Set heightmap
            RandomizeTerrain();
        }

        void Update()
        {
            // Call animation function
            AnimTerrain();
        }

        // Set the terrain using noise pattern
        private void RandomizeTerrain()
        {
            // Extract entire heightmap (expensive!)
            _terrHeights = _myTerrData.GetHeights(0, 0, _xRes, _yRes);

            // STUDENT'S CODE //
            // ...
            float[] scale = new float[numberOfPasses];
            for (int k = 0; k < numberOfPasses; k++)
            {
                scale[k] = UnityEngine.Random.Range(scaleMin, scaleMax);
            }

            ResetTerrainHeights();

            _diamondSquareNoiseGen.GenerateValues(ref _terrHeights, _xRes, _yRes);

            for (int i = 0; i < 5; i++)
            {
                Debug.Log(_terrHeights[i, 0]);
            }

            for (int i = 0; i < _xRes; i++)
            {
                float xCoeff = (float)i / _xRes;

                for (int j = 0; j < _yRes; j++)
                {
                    float yCoeff = (float)j / _yRes;
                    float currentHeight = 0.0f;
                    for (int k = 0; k < numberOfPasses; k++)
                    {
                        currentHeight += Mathf.PerlinNoise(xCoeff * scale[k], yCoeff * scale[k]);
                    }

                    currentHeight /= (float)numberOfPasses;
                    currentHeight *= weight;
                    _terrHeights[i, j] += currentHeight;
                    float totalWeight = _diamondSquareNoiseGen.Weight + weight;
                    //_terrHeights[i, j] /= totalWeight;
                }

                // Set terrain heights (_terrHeights[coordX, coordY] = heightValue) in a loop
                // You can sample perlin's noise (Mathf.PerlinNoise (xCoeff, yCoeff)) usingcoefficients
                // between 0.0f and 1.0f
                // You can combine 2-3 layers of noise with different resolutions and amplitudes fora better effect

                // END OF STUDENT'S CODE //

                // Set entire heightmap (expensive!)

            }
            _myTerrData.SetHeights(0, 0, _terrHeights);
            originalTerrainSectionHeight = _myTerrData.GetHeights(147, 168, radiusOfAnimation * 2, radiusOfAnimation * 2);
        }

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

        // Animate part of the terrain
        private void AnimTerrain()
        {
            // STUDENT'S CODE //

            // Extract PART of the terrain e.g. 40x40 tiles (select corner (x, y) and extractedpatch size)
            // GetHeights(5, 5, 10, 10) will extract 10x10 tiles at position (5, 5)
            // Animate it using Time.time and trigonometric functions
            // 3d generalizaton of sinc(x) function can be used to create the teardrop effect(sinc(x) = sin(x) / x)
            // It is reasonable to store animated part of the terrain in temporary variable e.g.in RandomizeTerrain()
            // function. Later, in AnimTerrain() this temporary area can be combined withcalculated Z (height) value.
            // Make sure you make a deep copy instead of shallow one (Clone(), assign operator).
            // Set PART of the terrain (use extraction parameters)

            // END OF STUDENT'S CODE //
            _terrHeights = _myTerrData.GetHeights(147, 168, radiusOfAnimation * 2, radiusOfAnimation * 2);
            Vector2 middle = new Vector2(radiusOfAnimation, radiusOfAnimation);

            for (int i = 0; i < radiusOfAnimation * 2; i++)
            {
                for (int j = 0; j < radiusOfAnimation * 2; j++)
                {
                    Vector2 point = new Vector2(i, j);
                    double distance = Vector2.Distance(point, middle);
                    double difference = (radiusOfAnimation - distance) / radiusOfAnimation;
                    if (difference < 0)
                        difference = 0;
                    _terrHeights[i, j] = (float)(originalTerrainSectionHeight[i, j] * (Math.Sin(Time.time + distance / 10) / 2f) * difference) + originalTerrainSectionHeight[i, j];
                }
            }
            _myTerrData.SetHeights(147, 168, _terrHeights);
        }
    }
}