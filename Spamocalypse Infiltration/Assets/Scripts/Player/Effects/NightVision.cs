using UnityEngine;

[AddComponentMenu("Image Effects/Night Vision")]

/// <summary>
/// Night vision effect for the player. Adapted from the Unity3D wiki.
/// <see cref="http://wiki.unity3d.com/index.php/GreenNightVision"/>
/// </summary>

[ExecuteInEditMode]
public class NightVision : MonoBehaviour {

	[Tooltip("The Night Vision effect shader")]
	public Shader shader;

	[Tooltip("The colour of the effect")]
	public Color luminence = Color.gray;

	[Tooltip("The noise of the signal")]
	public float noiseFactor = 0.005f;

	Material mat;
 
	void Start() 
	{
		shader = Shader.Find( "Custom/Night Vision" );
		mat = new Material (shader);
		mat.SetVector( "lum", new Vector4( luminence.g, luminence.g, luminence.g, luminence.g) );
		mat.SetFloat("noiseFactor", noiseFactor);
	}
 
	// Called by camera to apply image effect
	void OnRenderImage (RenderTexture source, RenderTexture destination) 
	{ 	
		
		mat.SetFloat("time", Mathf.Sin(Time.time * Time.deltaTime));
		Graphics.Blit( source, destination, mat );
	}
}
