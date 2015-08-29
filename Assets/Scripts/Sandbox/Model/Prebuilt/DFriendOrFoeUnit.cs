using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

public class DFriendOrFoeUnit : Device 
{
	private List<ContainerView> m_targets = new List<ContainerView>();


	#region device's functions

	private void AddTarget( params object[] data )
	{
		ContainerView view = (data[0] as Container).View;

		if( view.m_owner != m_containerAttachedTo.View.m_owner )
		{
		   	m_targets.Add( view );

			view.m_contain.onDestroy += () => { RemoveTarget( view.m_contain ); };
		}
	}

	private void RemoveTarget( params object[] data )
	{
		ContainerView view = (data[0] as Container).View;

		m_targets.Remove( view );
	}

	private void ResetTarget( params object[] data )
	{
		m_targets.Clear();
	}

	private void DesignateClosestTarget( params object[] objects )
	{
		ContainerView thisContainer = m_containerAttachedTo.View;

		m_targets.Sort( ( ContainerView x, ContainerView y ) =>
		{
			float distance1 = (thisContainer.transform.position - x.transform.position).magnitude;
			float distance2 = (thisContainer.transform.position - y.transform.position).magnitude;

			return distance1.CompareTo( distance2 );
		});
	}

	#endregion

	#region Predecates 

	public bool IsAnyTarget()
	{
		return m_targets.Count > 0;
	}

	#endregion

	#region device's interface implementation

	public override void OnDeviceInstalled()
	{
		AddEvent( "TargetPosition", null );

		AddAction("AddTarget", AddTarget );
		AddAction("RemoveTarget", RemoveTarget);
		AddAction("ResetTarget", ResetTarget);

		AddAction("DesignateClosestTarget", DesignateClosestTarget);

		AddCheck("IsAnyTarget", IsAnyTarget );
	}

	public override void Initialize()
	{

	}

	public override void Update()
	{
		DeviceEvent targetPos = GetEvent("TargetPosition");
		if( targetPos != null && m_targets.Count > 0 )
		{
			m_containerAttachedTo.IntegratedDevice.ScheduleEvent( targetPos, new System.Object[]{ m_targets[0].transform.position } );
		}
	}

	#endregion


}
