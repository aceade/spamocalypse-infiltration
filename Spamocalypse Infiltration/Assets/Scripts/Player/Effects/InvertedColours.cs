using UnityEngine;
using System.Collections;

/// <summary>
/// Inverted colours for the lols. Based on shader by Daniel DeEntremont
/// </summary>
[ExecuteInEditMode]
public class InvertedColours : MonoBehaviour 
{
    public Shader shader;

    public Color luminence = Color.gray;

    public Material mat;

    void Start()
    {
        GameTagManager.LogMessage("Shader for inversion is {0}", shader);
        mat = new Material (shader);
        mat.SetVector( "lum", new Vector4( luminence.g, luminence.g, luminence.g, luminence.g) );
    }

    // Called by camera to apply image effect
    void OnRenderImage (RenderTexture source, RenderTexture destination) 
    {   
        if (mat != null)
        {
            mat.SetFloat("time", Mathf.Sin(Time.time * Time.deltaTime));
            Graphics.Blit( source, destination, mat );
        }

    }
}
