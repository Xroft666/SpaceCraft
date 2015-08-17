using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

public class DFriendOrFoeUnit : Device 
{
	// Exportable variable
	public ContainerView m_target = null;
	public List<ContainerView> m_targets = new List<ContainerView>();


	#region device's functions

	private void SetTarget( params object[] data )
	{
		m_target = (data[0] as Container).View ;
	}

	private void ResetTarget( params object[] data )
	{
		m_target = null;
	}

	private void AddTarget( params object[] data )
	{
		m_targets.Add( (data[0] as Container).View );
		if( m_target == null )
			m_target = m_targets[0];
	}

	private void RemoveTarget( params object[] data )
	{
		m_targets.Remove( (data[0] as Container).View );

		if( m_targets.Count == 0 )
			m_target = null;
		else
			m_target = m_targets[0];
	}

	private void DesignateClosestTarget( params object[] objects )
	{
		ContainerView thisContainer = m_containerAttachedTo.View;
		ContainerView closestContainer = null;

		float minDistance = float.MaxValue;

		foreach( ContainerView view in m_targets )
		{

			float distance = (thisContainer.transform.position - view.transform.position).magnitude;
			if( distance < minDistance )
			{
				minDistance = distance;
				closestContainer = view;
			}
		}

		if( closestContainer == null )
			Debug.LogWarning("No targets nearby. Designating null");

		m_target = closestContainer;
	}

	#endregion

	#region Predecates 

	public bool IsAnyTarget()
	{
		return m_target != null || m_targets.Count > 0;
	}

	#endregion

	#region device's interface implementation

	public override void OnDeviceInstalled()
	{
		AddEvent( "TargetPosition", null );

		AddAction("SetTarget", SetTarget );
		AddAction("ResetTarget", ResetTarget);
		AddAction("AddTarget", SetTarget );
		AddAction("RemoveTarget", ResetTarget);
		AddAction("DesignateClosestTarget", ResetTarget);

		AddCheck("IsAnyTarget", IsAnyTarget );
	}

	public override void Initialize()
	{

	}

	public override void Update()
	{
		DeviceEvent targetPos = GetEvent("TargetPosition");
		if( targetPos != null && m_target != null )
		{
			m_containerAttachedTo.IntegratedDevice.ScheduleEvent( targetPos, new System.Object[]{ m_target.transform.position } );
		}
	}

	#endregion


}
