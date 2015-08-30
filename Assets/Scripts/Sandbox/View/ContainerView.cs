using UnityEngine;

using SpaceSandbox;
using System.Collections.Generic;

using BehaviourScheme;

/// <summary>
/// Container view representation mono class.
/// Passes through all the Unity-related events to the model Container class
/// </summary>

public class ContainerView : MonoBehaviour 
{
	public Container m_contain;
	public int m_owner;

	private void Start()
	{
		m_contain.Initialize();

#if !UNITY_EDITOR
		GameObject marker = GameObject.CreatePrimitive( PrimitiveType.Cube );
		marker.transform.SetParent( transform, false );
		marker.transform.localScale *= 0.25f;
		marker.transform.localPosition = Vector3.up * 0.4f;
		
		Component.Destroy( marker.GetComponent<Collider>() );
#endif
	}

	private void Update()
	{
		m_contain.Update();
	}

	void LateUpdate()
	{
		m_contain.LateUpdate();
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position + transform.up);

		m_contain.OnDrawGizmos();
	}
}
