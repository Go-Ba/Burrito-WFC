using System.Collections.Generic;
using UnityEngine;

public static class WFC
{
    public static TextureModel GetOverlapModel(Sprite _sprite, int _width, int _height, int _N) => GetOverlapModel(_sprite, _width, _height, _N, true, true, 8, 0);
    //ground set to 0 should do nothing, I believe the ground value is supposed to be for patterns which have a specific origin point, like the bottom of the texture?
    public static TextureModel GetOverlapModel(Sprite _sprite, int _width, int _height, int _N, bool _periodicInput, bool _periodicOutput, int _symmetry)
        => GetOverlapModel(_sprite, _width, _height, _N, _periodicInput, _periodicOutput, _symmetry, 0);
    public static TextureModel GetOverlapModel(Sprite _sprite, int _width, int _height, int _N, bool _periodicInput, bool _periodicOutput, int _symmetry, int _ground)
    {
        byte[,] sample = SpriteToSample(_sprite, out List<Color> colors);
        var overlapModel = new OverlappingModel(sample, _N, _width, _height, _periodicInput, _periodicOutput, _symmetry, _ground);
        return new TextureModel(_width, _height, overlapModel, colors);
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
        public TextureModel(int _width, int _height, OverlappingModel _model, List<Color> _colors)
        {
            model = _model;
            colors = _colors;
            width = _width;
            height = _height;
        }
        //when calling this function, start the retry count at 0
        public Texture2D Run() => GetTextureFromModel(-1, 1000, 3, 0);
        public Texture2D Run(int _propagationLimit, int _maxRetries) => GetTextureFromModel(-1, _propagationLimit, _maxRetries, 0);
        public Texture2D Run(int _seed, int _propagationLimit, int _maxRetries) => GetTextureFromModel(_seed, _propagationLimit, _maxRetries, 0);
        Texture2D GetTextureFromModel(int _seed, int _propagationLimit, int _maxRetries, int _retryCount)
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
                    return GetTextureFromModel(seed + 1, _propagationLimit, _maxRetries, _retryCount + 1);
                }
                Debug.LogError($"Failed to run overlap model after {_retryCount} retries");
                return null;
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
    }
}
