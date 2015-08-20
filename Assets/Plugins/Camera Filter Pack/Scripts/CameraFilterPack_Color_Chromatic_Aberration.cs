////////////////////////////////////////////////////////////////////////////////////
//  CameraFilterPack v2.0 - by VETASOFT 2015 //////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Colors/Chromatic_Aberration")]
public class CameraFilterPack_Color_Chromatic_Aberration : MonoBehaviour {

	[Range(-0.02f, 0.02f)]
	public float Offset = 0.02f;
	
	[Range(1f, 1000f)]
	public float ScanlineResolution = 800f;
	
	[Range(0f, 1f)]
	public float ScanlineIntensity = 0.04f;


	#region Variables
	private Shader SCShader;
	private Material SCMaterial;
	
	private float ChangeOffset;
	private float ChangeResolution;
	private float ChangeIntensity;

	#endregion
	
	#region Properties
	Material material
	{
		get
		{
			if(SCMaterial == null)
			{
				SCMaterial = new Material(SCShader);
				SCMaterial.hideFlags = HideFlags.HideAndDontSave;	
			}
			return SCMaterial;
		}
	}
	#endregion
	void Start () 
	{
		ChangeOffset = Offset;
		ChangeResolution = ScanlineResolution;
		ChangeIntensity = ScanlineIntensity;

		SCShader = Shader.Find("CameraFilterPack/Color_Chromatic_Aberration");

		if(!SystemInfo.supportsImageEffects)
		{
			enabled = false;
			return;
		}
	}
	
	void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture)
	{
		if(SCShader != null)
		{
			material.SetFloat("_ScanlineRes", ScanlineResolution);
			material.SetFloat("_ScanlineIntens", ScanlineIntensity);
			material.SetFloat("_Distortion", Offset);

			Graphics.Blit(sourceTexture, destTexture, material);
		}
		else
		{
			Graphics.Blit(sourceTexture, destTexture);	
		}
		
		
	}
	void OnValidate()
	{
		ChangeOffset = Offset;
		ChangeIntensity = ScanlineIntensity;
		ChangeResolution = ScanlineResolution;
	}

	// Update is called once per frame
	void Update () 
	{
		if (Application.isPlaying)
		{
			Offset = ChangeOffset;
			ScanlineIntensity = ChangeIntensity;
			ScanlineResolution = ChangeResolution;
		}

		#if UNITY_EDITOR
		if (Application.isPlaying!=true)
		{
			SCShader = Shader.Find("CameraFilterPack/Color_Chromatic_Aberration");

		}
		#endif

	}
	
	void OnDisable ()
	{
		if(SCMaterial)
		{
			DestroyImmediate(SCMaterial);	
		}
		
	}
	
	
}