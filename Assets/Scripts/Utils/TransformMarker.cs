using UnityEngine;
using System.Collections;

public class TransformMarker : MonoBehaviour 
{
	void OnDrawGizmos()
	{
		Color col = Color.yellow;
		col.a = 0.25f;

		Gizmos.color = col;
		Gizmos.DrawSphere(transform.position, 0.25f);
	}
}
