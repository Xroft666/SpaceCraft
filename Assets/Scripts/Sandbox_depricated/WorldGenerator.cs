using UnityEngine;
using System.Collections;

public class WorldGenerator : MonoBehaviour 
{
	public Sprite rockSprite;
//	public Color tintColor;
	public Light lightSource;

	void Start ()
	{
        //TODO tmp removed
	    PerlinNoise noise = null;//new PerlinNoise(Random.Range(0, 100));

		for( int i = -100; i < 100; i++ )
			for( int j = -100; j < 100; j++ )
		{
			// normalized value from 0 to 1
//			float noiseValue =  noise.FractalNoise2D( i, j, 8, 100.5f, 0.5f) + 0.5f;
			Vector4 bump = RetrieveBump( noise, i, j );

			if( bump.w > 0.6f )
			{
				GameObject container = new GameObject("container_" + i + "_" + j );

				TileRenderer tileRenderer = container.AddComponent<TileRenderer>();
				tileRenderer.iPos = i;
				tileRenderer.jPos = j;
				tileRenderer.bump = BumpSpaceToWorld(bump, container.transform.right, container.transform.up, container.transform.forward);
				tileRenderer.lightSource = lightSource;

				SpriteRenderer spriteRenderer = container.AddComponent<SpriteRenderer>();
				spriteRenderer.sprite = rockSprite;

				container.transform.position = new Vector3( i * rockSprite.rect.width / 100f, j * rockSprite.rect.height / 100f, 0f);//bump.w * 100f);


				// black stroke
				float noiseTop = noise.FractalNoise2D( i, j+1, 8, 100.5f, 0.5f) + 0.5f;
				float noiseBottom = noise.FractalNoise2D( i, j-1, 8, 100.5f, 0.5f) + 0.5f;
				float noiseRight = noise.FractalNoise2D( i+1, j, 8, 100.5f, 0.5f) + 0.5f;
				float noiseLeft = noise.FractalNoise2D( i-1, j, 8, 100.5f, 0.5f) + 0.5f;
				
				float noiseNE = noise.FractalNoise2D( i+1, j+1, 8, 100.5f, 0.5f) + 0.5f;
				float noiseSE = noise.FractalNoise2D( i+1, j-1, 8, 100.5f, 0.5f) + 0.5f;
				float noiseSW = noise.FractalNoise2D( i-1, j-1, 8, 100.5f, 0.5f) + 0.5f;
				float noiseNW = noise.FractalNoise2D( i-1, j+1, 8, 100.5f, 0.5f) + 0.5f;
				
				if( noiseTop <= 0.6f || noiseBottom <= 0.6f || noiseRight <= 0.6f || noiseLeft <= 0.6f ||
				   noiseNE <= 0.6f || noiseSE <= 0.6f || noiseSW <= 0.6f || noiseNW <= 0.6f )
					tileRenderer.stroke = true;
			}
		}
	}

	Vector4 RetrieveBump(PerlinNoise noise, int i, int j)
	{
		float wave = noise.FractalNoise2D( i, j, 8, 100.5f, 0.5f) + 0.5f;
		
		float s11 = wave;
		
		float s01 = (noise.FractalNoise2D( i-1, j, 8, 100.5f, 0.5f) + 0.5f) ;
		float s21 = (noise.FractalNoise2D( i+1, j, 8, 100.5f, 0.5f) + 0.5f) ;
		float s10 = (noise.FractalNoise2D( i, j-1, 8, 100.5f, 0.5f) + 0.5f) ;
		float s12 = (noise.FractalNoise2D( i, j+1, 8, 100.5f, 0.5f) + 0.5f) ;
		
		Vector3 va = new Vector3(2f, 0f,(s21-s01) * 100f).normalized;
		Vector3 vb = new Vector3(0f, 2f,(s12-s10) * 100f).normalized;
		Vector3 bump = Vector3.Cross(va, vb);

		return new Vector4(bump.x, bump.y, bump.z, s11);
	}

	Vector4 BumpSpaceToWorld(Vector4 bump, Vector3 tangent, Vector3 binormal, Vector3 normal)
	{
		Vector3 bump_ws = bump.x * tangent + bump.y * binormal + bump.z * normal;
		
		return new Vector4(bump_ws.x,bump_ws.y, bump_ws.z, bump.w);
	}
}
