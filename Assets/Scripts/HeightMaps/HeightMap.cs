using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.HeightMaps
{
    public class HeightMap : MonoBehaviour
    {
        private List<List<float>> _readHeightMap = null;
        public int Height { get; private set; }
        public int Width { get; private set; }

        public bool ReadFile(string address)
        {
            try
            {
                var texture = Resources.Load<Texture2D>(address);
                var textureData = texture.GetPixels();
                int texWidth = texture.width;
                int texHeight = texture.height;
                Height = texHeight;
                Width = texWidth;

                _readHeightMap = new List<List<float>>(texWidth);

                for (int y = 0; y < texHeight; y++)
                {
                    var singleRow = new List<float>(texWidth);
                    for (int x = 0; x < texWidth; x++)
                    {
                        singleRow.Add(GetGrayScale(textureData[texWidth * y + x]));
                    }

                    _readHeightMap.Add(singleRow);
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// Returns height map from read file. Call after calling ReadFile. Will throw exception if no files were previously loaded.
        /// </summary>
        /// <returns></returns>
        public List<List<float>> GetHeightMap()
        {
            if (_readHeightMap == null)
            {
                throw new Exception("No image previously loaded! Cannot return non-existing bitmap!");
            }

            return _readHeightMap;
        }
        private float GetGrayScale(Color color)
        {
            float grayScaleValue = color.r + color.g + color.b;
            grayScaleValue /= 3;
            grayScaleValue *= color.a;

            return grayScaleValue;
        }
    }
}
