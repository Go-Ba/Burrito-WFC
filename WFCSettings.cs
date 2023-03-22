using UnityEngine;

namespace BurritoWFC
{
    [System.Serializable]
    public class WFCSettings
    {
        [Tooltip("source sprite")]
        [SerializeField] public Sprite sprite;
        [Tooltip("width of output texture")]
        [SerializeField] public int width = 20;
        [Tooltip("height of output texture")]
        [SerializeField] public int height = 20;
        [Tooltip("size of the sample you take from the sprite")]
        [SerializeField] public int sampleTileSize = 3;
        [Tooltip("whether it loops around edges of the texture when sampling from sprite")]
        [SerializeField] public bool periodicInput = true;
        [Tooltip("whether it loops around edges of the texture  when creating the output")]
        [SerializeField] public bool periodicOutput = true;
        [Tooltip("how many ways will the samples areas be flipped when creating the list of patterns")]
        [SerializeField] public int symmetry = 8;
        [Tooltip("max number of loops for collapsing the wave function before force stopping")]
        [SerializeField] public int propagationLimit = 10000;
        [Tooltip("how many times to retry with a new seed if this seed fails")]
        [SerializeField] public int maxRetries = 3;
        [Tooltip("seed -1 is for random seed, otherwise choose a specific seed")]
        [SerializeField] public int seed = -1;
        [Tooltip("Set to 0 for standard behaviour. The ground value is supposed to be for patterns which have a specific origin point, like the bottom of the texture.")]
        [SerializeField] public int ground = 0;
    }
}
