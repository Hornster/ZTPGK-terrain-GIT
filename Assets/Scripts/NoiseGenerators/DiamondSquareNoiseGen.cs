using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Assets.Scripts.NoiseGenerators
{
    /// <summary>
    /// Implementation of the diamond square algorithm.
    /// </summary>
    public class DiamondSquareNoiseGen : MonoBehaviour
    {
        /// <summary>
        /// Min random value for starting edges.
        /// </summary>
        [SerializeField]
        private float _startingEdgesMin = 1.0f;
        /// <summary>
        /// Max random value for starting edges.
        /// </summary>
        [SerializeField]
        private float _startingEdgesMax = 10.0f;
        /// <summary>
        /// Min random value for middle edges addition value.
        /// </summary>
        [SerializeField]
        private float _randomValueMin = 2.0f;
        /// <summary>
        /// Max random value for middle edges addition value.
        /// </summary>
        [SerializeField]
        private float _randomValueMax = 12.0f;
        /// <summary>
        /// The weight of generated values.
        /// </summary>
        [SerializeField] private float _weight = 1.0f;
        /// <summary>
        /// Value by which the factor is divided each step of the algorithm.
        /// </summary>
        [SerializeField] private float _factorDivisor = 10;
        /// <summary>
        /// Scaling factor used to decrease the range of random value added to each vertex.
        /// </summary>
        private float _scalingFactor = 1.0f;
        /// <summary>
        /// Amount of vertexes used in calculating the mean for diamond and square cases.
        /// </summary>
        private const int VertexUsed = 4;
        /// <summary>
        /// Generates values for 
        /// </summary>
        /// <param name="heightMap"></param>
        /// <param name="resX"></param>
        /// <param name="resY"></param>
        public void GenerateValues(ref float[,] heightMap, int resX, int resY)
        {
            if (resX != resY || (resX - 1) % 2 != 0)
            {
                throw new Exception("Incompatible map format. Map should be square, vertex amount 2^n+1 per edge. Same x and y.");
            }

            InitializeEdges(ref heightMap, resX, resY);
            var squareStep = new Vector2Int(resX - 1, resY - 1);

            int smallerSize = resX > resY ? resX : resY;
            smallerSize = (int)Math.Floor(Mathf.Log(smallerSize, 2));

            for (int i = 0; i < smallerSize; i++)
            {
                DiamondStage(ref heightMap, resX, resY, squareStep);
                SquareStage(ref heightMap, resX, resY, squareStep);

                squareStep = CalcSquareStep(squareStep.x, squareStep.y);
                _scalingFactor /= _factorDivisor;
            }

            NormalizeHeights(ref heightMap, resX, resY);
        }

        public float Weight => _weight;

        /// <summary>
        /// Divides provided values by 2.
        /// </summary>
        /// <param name="sizeX">Size X of the map.</param>
        /// <param name="sizeY">Size Y of the map.</param>
        /// <returns></returns>
        private Vector2Int CalcSquareStep(int sizeX, int sizeY)
        {
            return new Vector2Int((int)Math.Floor((float)(sizeX >> 1)), (int)Math.Floor((float)(sizeY >> 1)));
        }

        private void NormalizeHeights(ref float[,] heightMap, int resX, int resY)
        {
            float normalizationValue = _randomValueMax > _startingEdgesMax ? _randomValueMax : _startingEdgesMax;
            float normalizationCoeff = 1 / normalizationValue;
            for (int x = 0; x < resX; x++)
            {
                for (int y = 0; y < resY; y++)
                {
                    heightMap[x, y] *= normalizationCoeff;
                }
            }
        }

        private void DiamondStage(ref float[,] heightMap, int resX, int resY, Vector2Int squareStep)
        {
            for (int xPos = squareStep.x >> 1; xPos < resX; xPos += squareStep.x)
            {
                for (int yPos = squareStep.y >> 1; yPos < resY; yPos += squareStep.y)
                {
                    heightMap[xPos, yPos] += heightMap[xPos, yPos] = CalcAverageForPointDiamondCase(ref heightMap, new Vector2Int(resX, resY), squareStep, new Vector2Int(xPos, yPos)); ;
                }
            }
        }

        private void SquareStage(ref float[,] heightMap, int resX, int resY, Vector2Int squareStep)
        {
            for (int xPos = 0; xPos < resX; xPos += squareStep.x)
            {
                for (int yPos = squareStep.y >> 1; yPos < resY; yPos += squareStep.y)
                {
                    heightMap[xPos, yPos] += heightMap[xPos, yPos] = CalcAverageForPointSquareCase(ref heightMap, new Vector2Int(resX, resY), squareStep, new Vector2Int(xPos, yPos)); ;
                }
            }

            for (int xPos = squareStep.x >> 1; xPos < resX; xPos += squareStep.x)
            {
                for (int yPos = 0; yPos < resY; yPos += squareStep.y)
                {
                    heightMap[xPos, yPos] += CalcAverageForPointSquareCase(ref heightMap, new Vector2Int(resX, resY), squareStep, new Vector2Int(xPos, yPos));
                }
            }
        }

        private float CalcAverageForPointSquareCase(ref float[,] heightMap, Vector2Int mapSize, Vector2Int stepCount, Vector2Int currPos)
        {
            stepCount = new Vector2Int(stepCount.x >> 1, stepCount.y >> 1);
            var up = new Vector2Int(currPos.x, currPos.y - stepCount.y);
            var left = new Vector2Int(currPos.x - stepCount.x, currPos.y);
            var down = new Vector2Int(currPos.x, currPos.y + stepCount.y);
            var right = new Vector2Int(currPos.x + stepCount.x, currPos.y);

            float avg = 0;

            if (up.y < 0)
            {
                up.y = mapSize.y - 1 - stepCount.y;
            }
            avg += heightMap[up.x, up.y];

            if (down.y >= mapSize.y)
            {
                down.y = stepCount.y;
            }
            avg += heightMap[down.x, down.y];

            if (left.x < 0)
            {
                left.x = mapSize.x - 1 - stepCount.x;
            }
            avg += heightMap[left.x, left.y];

            if (right.x >= mapSize.x)
            {
                right.x = stepCount.x;
            }
            avg += heightMap[right.x, right.y];

            avg /= VertexUsed;
            avg *= _weight;
            avg += GetRandomValue() * _scalingFactor;

            return avg;
        }

        private float CalcAverageForPointDiamondCase(ref float[,] heightMap, Vector2Int mapSize, Vector2Int stepCount, Vector2Int currPos)
        {
            stepCount = new Vector2Int(stepCount.x >> 1, stepCount.y >> 1);
            var upL = new Vector2Int(currPos.x - stepCount.x, currPos.y - stepCount.y);
            var upR = new Vector2Int(currPos.x + stepCount.x, currPos.y - stepCount.y);
            var downL = new Vector2Int(currPos.x - stepCount.x, currPos.y + stepCount.y);
            var downR = new Vector2Int(currPos.x + stepCount.x, currPos.y + stepCount.y);


            float avg = 0;
            //No need for checks since we have a square map always.
            
            avg += heightMap[upL.x, upL.y];
            avg += heightMap[upR.x, upR.y];
            avg += heightMap[downL.x, downL.y];
            avg += heightMap[downR.x, downR.y];

            avg /= VertexUsed;
            avg *= _weight;

            avg += GetRandomValue() * _scalingFactor;

            return avg * _weight;
        }

        private float GetRandomValue()
        {
            return Random.Range(_randomValueMin, _randomValueMax)* _scalingFactor * _weight;
        }

        private void InitializeEdges(ref float[,] heightMap, int resX, int resY)
        {
            resX = resX - 1;
            resY = resY - 1;
            heightMap[0, 0] +=UnityEngine.Random.Range(_startingEdgesMin, _startingEdgesMax);
            heightMap[resX, 0] += UnityEngine.Random.Range(_startingEdgesMin, _startingEdgesMax);
            heightMap[resX, resY] += UnityEngine.Random.Range(_startingEdgesMin, _startingEdgesMax);
            heightMap[0, resY] += UnityEngine.Random.Range(_startingEdgesMin, _startingEdgesMax);
        }
    }
}
