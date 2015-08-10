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

	private void Start()
	{
		m_contain.Initialize();
	}

	private void Update()
	{
		m_contain.Update();
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position + transform.up);
	}
}
