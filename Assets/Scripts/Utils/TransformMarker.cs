using UnityEngine;
using System.Collections;

public class TransformMarker : MonoBehaviour 
{
	public float m_radius = 0.25f;
	public Color m_color = Color.yellow;

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
		Color col = m_color;
		col.a = 0.25f;

		Gizmos.color = col;
		Gizmos.DrawSphere(transform.position, m_radius);
	}
}
