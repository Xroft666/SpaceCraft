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
	public Vector3 m_targetPosition;

	public Vector3[] m_navPoint;
	private int currentNavigationIdx = 0;

	private int currentTargetIdx = 0;
	private float distanceTreshold = 1.5f;

	private NavMeshPath path = new NavMeshPath();
	
	#region device's functions

	private IEnumerator ReachTarget( DeviceQuery qry )//EventArgs args )
	{
		ArgsObject pArgs = qry.Invoke() as ArgsObject;

		while( !IsCloseTo((Vector3) pArgs.obj) )
		{
			yield return null;
		}

		DeviceTrigger reached = GetTrigger("TargetReached");
		if( reached != null )
			reached();
	}

	private IEnumerator SetNextPoint( DeviceQuery qry )//EventArgs args )
	{
		NextPoint();

		yield break;
	}

	private IEnumerator SetTargetPosition( DeviceQuery qry )//EventArgs args )
	{
		ArgsObject pArgs = qry.Invoke() as ArgsObject;

		m_targetPosition = (Vector3) pArgs.obj;
		
		yield break;
	}

	public IEnumerator GetWaypointsList( DeviceQuery qry )//EventArgs args)
	{
		ArgsObject pArgs = qry.Invoke() as ArgsObject;
		NavMesh.CalculatePath( m_containerAttachedTo.View.transform.position, (Vector3) pArgs.obj, NavMesh.AllAreas, path );

		currentNavigationIdx = 0;
		m_navPoint = path.corners;

		for( int i = 0; i < path.corners.Length - 1; i++ )
			Debug.DrawLine( path.corners[i], path.corners[i+1], Color.blue, 1f );

		yield break;
	}

	public IEnumerator SetNextNavigationPoint( DeviceQuery qry )//EventArgs args )
	{
		currentNavigationIdx++;

		yield break;
	}


	#endregion

	#region device's interface implementation

	public override void OnDeviceInstalled()
	{
		AddTrigger( "TargetPosition", null );
		AddTrigger( "TargetReached", null );

		AddAction( "ReachTarget", ReachTarget );
		AddAction( "SetNextPoint", SetNextPoint );
		AddAction( "GetWaypointsList", GetWaypointsList );
		AddAction( "SetTargetPosition", SetTargetPosition );
		AddAction( "SetNextNavigationPoint", SetNextNavigationPoint);

		AddQuery( "CurrentTarget", CurrentTarget );
		AddQuery( "GetWaypoints", GetWaypoints );
		AddQuery( "CurrentNavigationPosition", CurrentNavigationPosition );
	}

	public override void OnDeviceUninstalled()
	{
		RemoveTrigger( "TargetPosition" );
		RemoveTrigger( "TargetReached" );
		
		RemoveAction( "ReachTarget" );
		RemoveAction( "SetNextPoint" );
		RemoveAction( "GetWaypointsList" );
		RemoveAction( "SetTargetPosition" );
		RemoveAction( "SetNextNavigationPoint");
		
		RemoveQuery( "CurrentTarget" );
		RemoveQuery( "GetWaypoints" );
		RemoveQuery( "CurrentNavigationPosition" );
	}

	public override void Initialize()
	{

	}

	public override void Update()
	{

	}

	#endregion

	#region Queries

	public ArgsObject CurrentTarget()
	{
		return new ArgsObject() { obj = m_targetPosition };
	}

	public ArgsList GetWaypoints()
	{
		NavMesh.CalculatePath( m_containerAttachedTo.View.transform.position, m_targetPosition, NavMesh.AllAreas, path );	
		currentNavigationIdx = 0;
		m_navPoint = path.corners;

		for( int i = 0; i < path.corners.Length - 1; i++ )
			Debug.DrawLine( path.corners[i], path.corners[i+1], Color.blue, 1f );

		System.Object[] objects = new System.Object[path.corners.Length];
		for( int i = 0; i < path.corners.Length; i++ )
			objects[i] = path.corners[i];

		return new ArgsList() { objs = objects };
	}

	public ArgsObject CurrentNavigationPosition()
	{	
		Vector3 curPoint = m_navPoint[currentNavigationIdx];
		return new ArgsObject()  { obj = curPoint  };
	}

	#endregion

	#region Checks

	public bool IsCloseToCurrentTarget()
	{
		return IsCloseTo( m_targetPosition );
	}

	private bool IsCloseTo( Vector3 position )
	{
		return (position - m_containerAttachedTo.View.transform.position).magnitude <= distanceTreshold;
	}

	#endregion

	private void NextPoint()
	{
		currentTargetIdx = (int) Mathf.Repeat( currentTargetIdx + 1, m_patrolPoints.Length );
		m_targetPosition = m_patrolPoints[currentTargetIdx];
	}
}
