using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WFC_UI_Test : MonoBehaviour
{
    [SerializeField] RawImage image; //UI image to export to
    [SerializeField] Sprite sprite; //source sprite
    [SerializeField] int width = 20; //width of output texture
    [SerializeField] int height = 20; //height of output texture
    [SerializeField] int sampleTileSize = 3; //size of the sample you take from the sprite
    [SerializeField] bool periodicInput = true; //whether it loops around edges of the texture when sampling from sprite
    [SerializeField] bool periodicOutput = true; //whether it loops around edges of the texture  when creating the output
    [SerializeField] int symmetry = 8; //how many ways will the samples areas be flipped when creating the list of patterns
    [SerializeField] int propagationLimit = 10000; //max number of loops for collapsing the wave function before force stopping
    [SerializeField] int maxRetries = 3; //how many times to retry with a new seed if this seed fails
    [SerializeField] int seed = -1;  //seed -1 is for random seed, otherwise choose a specific seed
    public void GenerateWithInspector() 
        => image.texture = WFC.GetOverlapModel(sprite, width, height, sampleTileSize, periodicInput, periodicOutput, symmetry).Run(seed, propagationLimit, maxRetries);
    private void Start() 
        => GenerateWithInspector();
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            GenerateWithInspector();
    }
}
