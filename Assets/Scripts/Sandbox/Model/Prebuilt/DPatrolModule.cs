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

	public IEnumerator SetPatrolPoints( EventArgs args )
	{
		PositionsListArgs plArgs = args as PositionsListArgs;

		m_patrolPoints = plArgs.positions;

		yield break;
	}

	public IEnumerator CleanPatrolPoints( EventArgs args )
	{
		m_patrolPoints = null;

		yield break;
	}
	

	#endregion

	#region device's interface implementation

	public override void OnDeviceInstalled()
	{
		AddEvent( "TargetPosition", null );

		AddAction("SetPatrolPoints", SetPatrolPoints );
		AddAction("CleanPatrolPoints", CleanPatrolPoints);

	}

	public override void Initialize()
	{

	}

	public override void Update()
	{
		if( m_patrolPoints == null || m_patrolPoints.Length == 0 )
			return;

		if( IsCloseTo( m_patrolPoints[currentTargetIdx] ) )
			NextPoint();

		FeedPosition();
	}

	#endregion

	private bool IsCloseTo( Vector3 position )
	{
		return (position - m_containerAttachedTo.View.transform.position).magnitude <= distanceTreshold;
	}

	private void NextPoint()
	{
		currentTargetIdx = (int) Mathf.Repeat( currentTargetIdx + 1, m_patrolPoints.Length );
	}

	private void FeedPosition()
	{
		DeviceEvent targetPos = GetEvent("TargetPosition");
		if( targetPos != null )
			m_containerAttachedTo.IntegratedDevice.ScheduleEvent( targetPos, new PositionArgs() { position = m_patrolPoints[currentTargetIdx]} );
	}
}
