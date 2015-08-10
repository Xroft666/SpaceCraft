using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

public class DSteerModule : Device 
{
	private Transform m_transform;

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
		m_transform = m_containerAttachedTo.View.GetComponent<Transform>();
	}

	public override void Update()
	{

	}

	#endregion

	private void RotateTowards( Vector3 worldPos )
	{
		Quaternion finalRotation = RatationToTargetPoint(worldPos);

		m_transform.rotation = Quaternion.RotateTowards( m_transform.rotation, finalRotation, torqueSpeed * Time.deltaTime );

		if( Quaternion.Angle( m_transform.rotation, finalRotation ) < angleTreshold )
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

	private Quaternion RatationToTargetPoint( Vector3 worldPos )
	{
		Vector3 dir = (worldPos - m_transform.position).normalized;
		float zEuler = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

		return Quaternion.Euler( 0f, 0f, zEuler - 90f );
	}
}
