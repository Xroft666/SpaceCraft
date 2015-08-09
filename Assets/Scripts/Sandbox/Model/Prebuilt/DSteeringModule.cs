using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

public class DSteeringModule : Device 
{
	private Rigidbody2D m_rigidbody;
	private Transform m_transform;

	// temporary variable. Should be changed to something more physical realistic
	private float torqueSpeed = 3f;

	#region device's functions

	public void SteerTowards( params object[] objects )
	{
		RotateTowards( (Vector3) objects[0] );
	}


	#endregion

	#region device's interface implementation

	public override void OnDeviceInstalled()
	{
		AddAction("SteerTowards", SteerTowards );
	}

	public override void Initialize()
	{
		m_rigidbody = m_containerAttachedTo.View.GetComponent<Rigidbody2D>();
		m_transform = m_containerAttachedTo.View.GetComponent<Transform>();
	}

	public override void Update()
	{

	}

	#endregion

	private void RotateTowards( Vector3 worldPos )
	{
		Vector3 dir = (worldPos - m_transform.position).normalized;
		float zEuler = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

		m_transform.rotation = 
			Quaternion.Lerp( m_transform.rotation, Quaternion.Euler( 0f, 0f, zEuler - 90f ), Time.fixedDeltaTime * torqueSpeed );
	}
}
