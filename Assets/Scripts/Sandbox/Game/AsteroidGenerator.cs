using UnityEngine;
using System.Collections.Generic;

public class AsteroidGenerator 
{
	public static void GenerateCircular(float radius, float angleMin, float angleMax, out List<Vector2> vertices)
	{
		GenerateEllipsical( radius, radius, angleMin, angleMax, out vertices);
	}

	public static void GenerateEllipsical( float radius1, float radius2, float angleMin, float angleMax, out List<Vector2> vertices)
	{
		vertices = new List<Vector2>();

		for ( float a = 0f; a < 2f * Mathf.PI; a += Random.Range(angleMin, angleMax) * Mathf.PI / 180f )
		{
			Vector2 newPoint;
			newPoint.x = radius1 * Mathf.Cos(a);
			newPoint.y = radius2 * Mathf.Sin(a);  

			vertices.Add( newPoint );
		}

	}
}
