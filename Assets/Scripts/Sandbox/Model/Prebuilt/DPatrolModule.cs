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
	private float distanceTreshold = 1.5f;

	private NavMeshPath path = new NavMeshPath();
	
	#region device's functions

	private IEnumerator ReachTarget( EventArgs args )
	{
		PositionArgs pArgs = args as PositionArgs;

		if( NavMesh.CalculatePath( m_containerAttachedTo.View.transform.position, pArgs.position, NavMesh.AllAreas, path ) )
		{
			for( int i = 0; i < path.corners.Length - 1; i++ )
				Debug.DrawLine( path.corners[i], path.corners[i+1], Color.blue, 1f );

		}
		else
		{
			Debug.LogWarning("Path not found");
		}

		while( !IsCloseTo(pArgs.position) )
		{
			yield return null;
		}

		DeviceEvent reached = GetEvent("TargetReached");
		if( reached != null )
			reached();
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
