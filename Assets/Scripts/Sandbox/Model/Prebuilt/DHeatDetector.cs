using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

public class DHeatDetector : Device 
{
	// Exportable variable
	public ContainerView m_target;


	#region device's functions

	private void SetTarget( params object[] objects )
	{
		m_target = (objects[0] as Container).View;
	}

	private void ResetTarget( params object[] objects )
	{
		m_target = null;
	}

	private void SearchForClosestTarget(params object[] objects)
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
		AddEvent( "TargetPosition", null );

		AddAction("SetTarget", SetTarget );
		AddAction("ResetTarget", ResetTarget);
		AddAction("SearchForClosestTarget", SearchForClosestTarget );
	}

	public override void Initialize()
	{

	}

	public override void Update()
	{
		DeviceEvent targetPos = GetEvent("TargetPosition");
		if( targetPos != null && m_target != null )
			targetPos.Invoke( m_target.transform.position );

	}

	#endregion


}
