using UnityEngine;
using System.Collections;

public class TransformMarker : MonoBehaviour 
{
	void Start()
	{
#if !UNITY_EDITOR
		GameObject marker = GameObject.CreatePrimitive( PrimitiveType.Sphere );
		marker.transform.SetParent( transform, false );
		marker.transform.localScale *= 0.25f;

		Component.Destroy( marker.GetComponent<Collider>() );
#endif
	}

	void OnDrawGizmos()
	{
		Color col = Color.yellow;
		col.a = 0.25f;

		Gizmos.color = col;
		Gizmos.DrawSphere(transform.position, 0.25f);
	}
}
