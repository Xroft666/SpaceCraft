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
	private float distanceTreshold = 1f;
	
	#region device's functions

	public void SetPatrolPoints( params object[] objects )
	{
		if( objects.Length == 0 )
			return;

		m_patrolPoints = new Vector3[objects.Length];

		for( int i = 0; i < objects.Length; i++ )
			m_patrolPoints[i] = (Vector3) objects[i];
	}

	public void CleanPatrolPoints( params object[] objects )
	{
		m_patrolPoints = null;
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
			targetPos.Invoke( m_patrolPoints[currentTargetIdx] );
	}
}
