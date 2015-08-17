using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

public class DSteerModule : Device 
{
	private Rigidbody2D m_rigidbody;
	private Vector3 m_targetDirection = Vector3.up;

	// temporary variable. Should be changed to something more physical realistic
	private float torqueSpeed = 100f;
	private float angleTreshold = 5f;

	#region device's functions

	public void SteerTowards( params object[] objects )
	{
		Vector2 worldPos = (Vector2) (Vector3) objects[0];
		m_targetDirection = ( worldPos - m_rigidbody.position).normalized;
		RotateTowards();
	}


	#endregion

	#region device's interface implementation

	public override void OnDeviceInstalled()
	{
		AddEvent("OnSteering", null );
		AddEvent("OnSteerComplete", null );

		AddAction("SteerTowards", SteerTowards );
	}

	public override void Initialize()
	{
		m_rigidbody = m_containerAttachedTo.View.GetComponent<Rigidbody2D>();
	}

	public override void Update()
	{
		float angle = Mathf.Abs(m_rigidbody.rotation - CurrentAngle());
		if( Mathf.Repeat( angle, 360f) < angleTreshold )
		{
			DeviceEvent onSteerComplete = GetEvent("OnSteerComplete");
			if( onSteerComplete != null )
				m_containerAttachedTo.IntegratedDevice.ScheduleEvent( onSteerComplete, null );
		}
		else
		{
			DeviceEvent onSteering = GetEvent("OnSteering");
			if( onSteering != null )
				m_containerAttachedTo.IntegratedDevice.ScheduleEvent( onSteering, null );
		}
	}

	#endregion

	private void RotateTowards()
	{
		m_rigidbody.angularVelocity = 0f;
		m_rigidbody.rotation = Mathf.MoveTowardsAngle( m_rigidbody.rotation, CurrentAngle(), torqueSpeed * Time.deltaTime );
	}

	private float CurrentAngle()
	{
		return Mathf.Atan2(m_targetDirection.y, m_targetDirection.x) * Mathf.Rad2Deg - 90f;
	}
}
