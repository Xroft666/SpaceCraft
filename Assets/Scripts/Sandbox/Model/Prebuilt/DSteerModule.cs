using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

public class DSteerModule : Device 
{
	private Rigidbody2D m_rigidbody;

	// temporary variable. Should be changed to something more physical realistic
	private float torqueSpeed = 25f;

	#region device's functions

	public IEnumerator SteerTowards( EventArgs args )
	{
		PositionArgs pArgs = args as PositionArgs;

		DeviceEvent onSteerStart = GetEvent("OnSteerStart");
		if( onSteerStart != null )
			m_containerAttachedTo.IntegratedDevice.ScheduleEvent( onSteerStart, null );

		Vector2 worldPos = (Vector2) pArgs.position;

		m_rigidbody.angularVelocity = 0f;
		m_rigidbody.rotation = Mathf.Repeat( m_rigidbody.rotation, 360f );

		float prevAngle = 1f;
		float currentAngle = 0f;

		while ( Mathf.Abs(currentAngle) < Mathf.Abs(prevAngle) )
		{
			prevAngle = Mathf.DeltaAngle( m_rigidbody.rotation , CurrentAngle(worldPos)) ;
			m_rigidbody.rotation =
						Mathf.MoveTowardsAngle( m_rigidbody.rotation, 
			            CurrentAngle(worldPos), 
			            torqueSpeed * Time.deltaTime );

			currentAngle = Mathf.DeltaAngle( m_rigidbody.rotation , CurrentAngle(worldPos)) ;
	
			yield return null;
		}

		DeviceEvent onSteerComplete = GetEvent("OnSteerComplete");
		if( onSteerComplete != null )
			m_containerAttachedTo.IntegratedDevice.ScheduleEvent( onSteerComplete, null );
	}


	#endregion

	#region device's interface implementation

	public override void OnDeviceInstalled()
	{
		AddEvent("OnSteerStart", null );
		AddEvent("OnSteerComplete", null );

		AddAction("SteerTowards", SteerTowards );
	}

	public override void Initialize()
	{
		m_rigidbody = m_containerAttachedTo.View.GetComponent<Rigidbody2D>();
	}

	public override void Update()
	{

	}

	#endregion


	private float CurrentAngle( Vector3 worldPos )
	{
		Vector3 direction = ( (Vector2) worldPos - m_rigidbody.position).normalized;
		return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
	}
}
