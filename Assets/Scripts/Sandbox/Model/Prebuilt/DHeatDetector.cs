using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

public class DHeatDetector : Device 
{

	private ContainerRepresentation m_target = null;

	#region device's functions

	private void SetTarget( params Entity[] objects )
	{
		m_target = (objects[0] as Container).View;
	}

	private void SearchForClosestTarget(params Entity[] objects)
	{
		Dictionary<string, Entity> memoryObjects = m_containerAttachedTo.Blueprint.Memory.GetAllObjects();

		ContainerRepresentation thisContainer = m_containerAttachedTo.View;
		ContainerRepresentation closestContainer = null;

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

	#endregion
}
