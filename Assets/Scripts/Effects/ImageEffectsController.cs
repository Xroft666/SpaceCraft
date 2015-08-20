using UnityEngine;
using System.Collections;

public class ImageEffectsController : MonoBehaviour 
{
	public Camera m_UICamera;

	private Material m_Material;

	Material material
	{
		get
		{
			if(m_Material == null)
			{
				m_Material = new Material(Shader.Find("Custom/UIWithImageEffects"));
				m_Material.hideFlags = HideFlags.HideAndDontSave;	
			}
			return m_Material;
		}
	}

	void Awake()
	{
		if( m_UICamera == null )
		{
			Debug.LogError("No UI camera assigned.");
			return;
		}

		m_UICamera.targetTexture = new RenderTexture( Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear );
	}

	void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture)
	{
		if( m_UICamera == null || m_UICamera.targetTexture == null )
		{
			Graphics.Blit(sourceTexture, destTexture);
			return;
		}

		material.SetTexture("_OverlayTex", m_UICamera.targetTexture);
		Graphics.Blit(sourceTexture, destTexture, material);
	}
}
