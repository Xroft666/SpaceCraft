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

	private IEnumerator SearchForClosestTarget()
	{
		Dictionary<string, Entity> memoryObjects = m_containerAttachedTo.Blueprint.Memory.GetAllObjects();

		ContainerRepresentation thisContainer = m_containerAttachedTo.Representation;
		ContainerRepresentation closestContainer = null;

		float minDistance = float.MaxValue;

		foreach( KeyValuePair<string, Entity> pair in memoryObjects )
		{
			Container container = pair.Value as Container;
			if( container == null )
				continue;

			float distance = (thisContainer.transform.position - container.Representation.transform.position).magnitude;
			if( distance < minDistance )
			{
				minDistance = distance;
				closestContainer = container.Representation;
			}
		}

		if( closestContainer == null )
		{
			Debug.Log("HeatDetector: No targets nearby.");
			yield break;
		}

		m_target = closestContainer;
	}

	#endregion

	#region device's interface implementation

	public override void OnDeviceInstalled()
	{
	//	AddEvent( "OnTimerTrigger", new UnityEvent() );
		AddFunction("SearchForClosestTarget", Job.make(SearchForClosestTarget()) );
	}

	public override void Initialize()
	{

	}

	public override void Update()
	{

	}

	public override void Delete()
	{

	}

	#endregion
}
