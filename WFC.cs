using System.Collections.Generic;
using UnityEngine;

namespace BurritoWFC
{
    public static class WFC
    {
        public static TextureModel GetOverlapModel(WFCSettings _set)
            => GetOverlapModel(_set.sprite, _set.width, _set.height, _set.sampleTileSize, _set.periodicInput, _set.periodicOutput, _set.symmetry, _set.ground);
        public static TextureModel GetOverlapModel(Sprite _sprite, int _width, int _height, int _N) => GetOverlapModel(_sprite, _width, _height, _N, true, true, 8, 0);
        //ground set to 0 should do nothing, I believe the ground value is supposed to be for patterns which have a specific origin point, like the bottom of the texture?
        public static TextureModel GetOverlapModel(Sprite _sprite, int _width, int _height, int _N, bool _periodicInput, bool _periodicOutput, int _symmetry)
            => GetOverlapModel(_sprite, _width, _height, _N, _periodicInput, _periodicOutput, _symmetry, 0);
        public static TextureModel GetOverlapModel(Sprite _sprite, int _width, int _height, int _N, bool _periodicInput, bool _periodicOutput, int _symmetry, int _ground)
        {
            //when periodic output is turned off, it doesn't generate the top and rightmost N-1 pixels
            //this adds an extra N-1 pixels and then they will be cut off later
            if (!_periodicOutput)
            {
                _width += _N - 1;
                _height += _N - 1;
            }

            byte[,] sample = SpriteToSample(_sprite, out List<Color> colors);
            var overlapModel = new OverlappingModel(sample, _N, _width, _height, _periodicInput, _periodicOutput, _symmetry, _ground);
            return new TextureModel(_width, _height, overlapModel, colors, _periodicOutput, _N);
        }
        static byte[,] SpriteToSample(Sprite _sprite, out List<Color> _colors)
        {
            var tex = _sprite.texture;
            byte[,] output = new byte[tex.width, tex.height];
            _colors = new List<Color>();

            for (int x = 0; x < tex.width; x++)
            {
                for (int y = 0; y < tex.height; y++)
                {
                    //store the colors in a list and then give the indeces of
                    //the colors as bytes to be sent to the model to process
                    var pixel = tex.GetPixel(x, y);
                    int index = GetColorIndex(ref _colors, pixel);
                    output[x, y] = (byte)index;
                }
            }
            return output;
        }
        static int GetColorIndex(ref List<Color> _colors, Color _color)
        {
            for (int i = 0; i < _colors.Count; i++)
                if (_colors[i] == _color)
                    return i;

            _colors.Add(_color);
            return _colors.Count - 1;
        }

        public class TextureModel
        {
            OverlappingModel model;
            List<Color> colors;
            int width;
            int height;
            bool periodicOutput;
            int tileSize;
            public TextureModel(int _width, int _height, OverlappingModel _model, List<Color> _colors, bool _periodicOutput, int _tileSize)
            {
                model = _model;
                colors = _colors;
                width = _width;
                height = _height;
                periodicOutput = _periodicOutput;
                tileSize = _tileSize;
            }
            //this gets called first to run the model
            //when calling this function, start the retry count at 0
            public TextureModel Run() => RunModel(-1, 1000, 3, 0);
            public TextureModel Run(int _propagationLimit, int _maxRetries) => RunModel(-1, _propagationLimit, _maxRetries, 0);
            public TextureModel Run(int _seed, int _propagationLimit, int _maxRetries) => RunModel(_seed, _propagationLimit, _maxRetries, 0);
            TextureModel RunModel(int _seed, int _propagationLimit, int _maxRetries, int _retryCount)
            {
                int seed = _seed == -1 ? Random.Range(0, int.MaxValue) : _seed;
                Debug.Log($"seed {seed}");
                if (model.Run(seed, _propagationLimit) == false)
                {
                    //if running the model failed, try again until hitting max retries
                    if (_retryCount < _maxRetries)
                    {
                        //need to clear the model after running it so that it is back to base state to run again
                        //at least that seems to be the behaviour based on my observations of the bugs
                        model.ResetModel();
                        return RunModel(seed + 1, _propagationLimit, _maxRetries, _retryCount + 1);
                    }
                    Debug.LogError($"Failed to run overlap model after {_retryCount} retries");
                    return null;
                }
                return this;
            }
            //after running the model, call this to get a texture
            public Texture2D GetTextureFromModel()
            {
                //when periodic output is turned off, it doesn't generate the top and rightmost N-1 pixels
                //this removes the extra N-1 pixels which were added on to account for that
                int width = this.width;
                int height = this.height;
                if (!periodicOutput)
                {
                    width -= tileSize - 1;
                    height -= tileSize - 1;
                }

                Texture2D newTex = new Texture2D(width, height);
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        //retrieve the color indeces from the model result
                        //then convert it to colors from the color list
                        int v = (int)model.Sample(x, y);
                        if (v < colors.Count)
                            newTex.SetPixel(x, y, colors[v]);
                        else
                            newTex.SetPixel(x, y, Color.clear);
                    }
                }
                newTex.filterMode = FilterMode.Point;
                newTex.Apply();
                return newTex;

            }
            //after running the model, call this to get a color grid
            public Color[,] GetColorGridFromModel()
            {
                //when periodic output is turned off, it doesn't generate the top and rightmost 2 pixels
                //this removes the extra 2 pixels which were added on to account for that
                int width = this.width;
                int height = this.height;
                if (!periodicOutput)
                {
                    width -= 2;
                    height -= 2;
                }

                Color[,] output = new Color[width, height];
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        //retrieve the color indeces from the model result
                        //then convert it to colors from the color list
                        int v = (int)model.Sample(x, y);
                        if (v < colors.Count)
                            output[x, y] = colors[v];
                        else
                            output[x, y] = Color.clear;
                    }
                }
                return output;
            }
        }
    }
}
