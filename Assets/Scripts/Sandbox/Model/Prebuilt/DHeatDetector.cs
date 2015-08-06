using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

public class DHeatDetector : Device 
{

	private ContainerView m_target;
	private Rigidbody2D m_rigidbody;
	private Transform m_transform;

	// temporary variable. Should be changed to something more physical realistic
	private float torqueSpeed = 10f;

	#region device's functions

	private void SetTarget( params Entity[] objects )
	{
		m_target = (objects[0] as Container).View;
	}

	private void SearchForClosestTarget(params Entity[] objects)
	{
		Dictionary<string, Entity> memoryObjects = m_containerAttachedTo.Blueprint.Memory.GetAllObjects();

		ContainerView thisContainer = m_containerAttachedTo.View;
		ContainerView closestContainer = null;

		float minDistance = float.MaxValue;

		foreach( KeyValuePair<string, Entity> pair in memoryObjects )
		{
			Container container = pair.Value as Container;
			if( container == null )
				continue;

			float distance = (thisContainer.transform.position - container.View.transform.position).magnitude;
			if( distance < minDistance )
			{
				minDistance = distance;
				closestContainer = container.View;
			}
		}

		if( closestContainer == null )
		{
			Debug.Log("HeatDetector: No targets nearby.");
			return;
		}

		m_target = closestContainer;
	}

	#endregion

	#region device's interface implementation

	public override void OnDeviceInstalled()
	{
	//	AddEvent( "OnTimerTrigger", new UnityEvent() );
		AddFunction("SetTarget", SetTarget );
		AddFunction("SearchForClosestTarget", SearchForClosestTarget );
	}

	public override void Initialize()
	{
		m_rigidbody = m_containerAttachedTo.View.GetComponent<Rigidbody>();
		m_transform = m_containerAttachedTo.View.GetComponent<Transform>();
	}

	public override void Update()
	{
		RotateObjectTowardsTarget();
	}

	#endregion

	private void RotateObjectTowardsTarget()
	{
		Vector3 dir = (m_target.transform.position - m_transform.position).normalized;
		m_transform.rotation = Quaternion.Lerp( m_transform.rotation, Quaternion.Euler(dir), Time.fixedDeltaTime * torqueSpeed ); 
	}
}
