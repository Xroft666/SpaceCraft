using UnityEngine;
using System.Collections;

public class TileRenderer : MonoBehaviour 
{
	public PerlinNoise noise;
	public int iPos;
	public int jPos;
	public Vector4 bump;
	public Light lightSource = null;

	public bool stroke = false;
	private float colorOffset = 0f;

	private SpriteRenderer spriteRenderer = null;

	void Start()
	{
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

		if( stroke )
		{
			spriteRenderer.color = Color.black;
		}
		else
		{
			int ranSeed = Random.Range(0, 50);
			if( ranSeed == 1)
				colorOffset = 0.25f;
			if( ranSeed == 2 )
				colorOffset = -0.25f;

			Color color = lightSource.color * (0.5f + colorOffset) * (((int)((((bump.w - 0.6f + 0.25) * 2.5f) * 256f) / 25f)) * 25f) / 256f;
			spriteRenderer.color = new Color(color.r + colorOffset, color.g + colorOffset, color.b + colorOffset);
		}
	}
	
//	void Update () 
//	{
//		if( !stroke )
//		{
//			//Color color = lightSource.color * (0.5f + colorOffset) * (((int)((((bump.w - 0.6f + 0.25) * 2.5f) * 256f) / 25f)) * 25f) / 256f;
//
//
//			float lightFactor =	((Vector3.Dot(new Vector3(bump.x, bump.y, bump.z), lightSource.transform.forward) + 1f) / 2f) * 256f; 
//			int lightGradaded = (int) (lightFactor / 25f);
//			lightFactor = lightGradaded * 25f / 256f;
//
//			Color color = lightSource.color * lightFactor;
//			spriteRenderer.color = new Color(color.r + colorOffset, color.g + colorOffset, color.b + colorOffset);
//
//			//spriteRenderer.color = new Color(bump.x, bump.y, bump.z);
//		}
//	}	
}
