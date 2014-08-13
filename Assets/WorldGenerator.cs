using UnityEngine;
using System.Collections;

public class WorldGenerator : MonoBehaviour 
{
	void Start () 
	{
		PerlinNoise noise = new PerlinNoise(Random.Range(0, 100));
		for( int i = -50; i < 50; i++ )
			for( int j = -50; j < 50; j++ )
		{
			float noiseValue =  noise.FractalNoise2D( i, j, 8, 1.5f, 3);

			if( noiseValue > 2f )
			{
				GameObject container = GameObject.CreatePrimitive( PrimitiveType.Cube );
				container.name = "container_" + i + "_" + j;
				container.transform.position = new Vector3( i, 0f, j);


			}
		}
	}
}
