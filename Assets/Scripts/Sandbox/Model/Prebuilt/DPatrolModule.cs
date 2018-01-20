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

	private UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();
	
	#region device's functions

	private IEnumerator ReachTarget( DeviceQuery qry )//EventArgs args )
	{
		ArgsObject pArgs = qry.Invoke() as ArgsObject;

		while( !IsCloseTo((Vector3) pArgs.obj) )
		{
			yield return null;
		}

		DeviceTrigger reached = m_blueprint.GetTrigger("TargetReached");
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
		UnityEngine.AI.NavMesh.CalculatePath( m_container.View.transform.position, (Vector3) pArgs.obj, UnityEngine.AI.NavMesh.AllAreas, path );

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
		m_blueprint.AddTrigger( "TargetPosition", null );
		m_blueprint.AddTrigger( "TargetReached", null );

		m_blueprint.AddAction( "ReachTarget", ReachTarget );
		m_blueprint.AddAction( "SetNextPoint", SetNextPoint );
		m_blueprint.AddAction( "GetWaypointsList", GetWaypointsList );
		m_blueprint.AddAction( "SetTargetPosition", SetTargetPosition );
		m_blueprint.AddAction( "SetNextNavigationPoint", SetNextNavigationPoint);

		m_blueprint.AddQuery( "CurrentTarget", CurrentTarget );
		m_blueprint.AddQuery( "GetWaypoints", GetWaypoints );
		m_blueprint.AddQuery( "CurrentNavigationPosition", CurrentNavigationPosition );
	}

	public override void OnDeviceUninstalled()
	{
		m_blueprint.RemoveTrigger( "TargetPosition" );
		m_blueprint.RemoveTrigger( "TargetReached" );
		
		m_blueprint.RemoveAction( "ReachTarget" );
		m_blueprint.RemoveAction( "SetNextPoint" );
		m_blueprint.RemoveAction( "GetWaypointsList" );
		m_blueprint.RemoveAction( "SetTargetPosition" );
		m_blueprint.RemoveAction( "SetNextNavigationPoint");
		
		m_blueprint.RemoveQuery( "CurrentTarget" );
		m_blueprint.RemoveQuery( "GetWaypoints" );
		m_blueprint.RemoveQuery( "CurrentNavigationPosition" );
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
		UnityEngine.AI.NavMesh.CalculatePath( m_container.View.transform.position, m_targetPosition, UnityEngine.AI.NavMesh.AllAreas, path );	
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
		return (position - m_container.View.transform.position).magnitude <= distanceTreshold;
	}

	#endregion

	private void NextPoint()
	{
		currentTargetIdx = (int) Mathf.Repeat( currentTargetIdx + 1, m_patrolPoints.Length );
		m_targetPosition = m_patrolPoints[currentTargetIdx];
	}
}
