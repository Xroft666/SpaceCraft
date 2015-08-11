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
	private float torqueSpeed = 100f;
	private float angleTreshold = 5f;

	#region device's functions

	public void SteerTowards( params object[] objects )
	{
		RotateTowards( (Vector3) objects[0] );
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

	}

	#endregion

	private void RotateTowards( Vector2 worldPos )
	{
		m_rigidbody.angularVelocity = 0f;

		Vector2 dir = (worldPos - m_rigidbody.position).normalized;
		float zEuler = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

		m_rigidbody.rotation = Mathf.MoveTowardsAngle( m_rigidbody.rotation, zEuler, torqueSpeed * Time.deltaTime );

		float angle = Mathf.Abs(m_rigidbody.rotation - zEuler);
		if( Mathf.Repeat( angle, 360f) < angleTreshold )
		{
			DeviceEvent onSteerComplete = GetEvent("OnSteerComplete");
			if( onSteerComplete != null )
				onSteerComplete.Invoke();
		}
		else
		{
			DeviceEvent onSteering = GetEvent("OnSteering");
			if( onSteering != null )
				onSteering.Invoke();
		}
	}
}
