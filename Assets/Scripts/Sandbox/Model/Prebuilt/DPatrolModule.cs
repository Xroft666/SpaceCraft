using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

public class DPatrolModule : Device 
{
	// Exportable variables
	public Vector3[] m_patrolPoints;


	private int currentTargetIdx = 0;
	private float distanceTreshold = 0.25f;
	
	#region device's functions

	private IEnumerator ReachTarget( EventArgs args )
	{
		PositionArgs pArgs = args as PositionArgs;

		while( !IsCloseTo(pArgs.position) )
		{
			yield return null;
		}

		DeviceEvent reached = GetEvent("TargetReached");
		if( reached != null )
			m_containerAttachedTo.IntegratedDevice.ScheduleEvent( reached, null );
	}

	private IEnumerator SetNextPoint( EventArgs args )
	{
		NextPoint();

		yield break;
	}

	#endregion

	#region device's interface implementation

	public override void OnDeviceInstalled()
	{
		AddEvent( "TargetPosition", null );
		AddEvent( "TargetReached", null );

		AddAction( "ReachTarget", ReachTarget );
		AddAction( "SetNextPoint", SetNextPoint );

		AddQuery( "CurrentTarget", CurrentTarget );
	}

	public override void Initialize()
	{

	}

	public override void Update()
	{

	}

	#endregion

	#region Queries

	public PositionArgs CurrentTarget()
	{
		return new PositionArgs() { position = m_patrolPoints[currentTargetIdx] };
	}

	#endregion

	#region Checks

	public bool IsCloseToCurrentTarget()
	{
		return IsCloseTo( m_patrolPoints[currentTargetIdx] );
	}

	private bool IsCloseTo( Vector3 position )
	{
		return (position - m_containerAttachedTo.View.transform.position).magnitude <= distanceTreshold;
	}

	#endregion

	private void NextPoint()
	{
		currentTargetIdx = (int) Mathf.Repeat( currentTargetIdx + 1, m_patrolPoints.Length );
	}
}
