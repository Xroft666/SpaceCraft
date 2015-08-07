using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

public class DPilotCockpit : Device 
{
	private Rigidbody2D m_rigidbody;
	private Transform m_transform;

	// temporary variable. Should be changed to something more physical realistic
	private float torqueSpeed = 10f;

	#region device's functions




	#endregion

	#region device's interface implementation

	public override void OnDeviceInstalled()
	{
	//	AddEvent( "OnTimerTrigger", new UnityEvent() );
	//	AddFunction("SetTarget", SetTarget );
	//	AddFunction("SearchForClosestTarget", SearchForClosestTarget );
	}

	public override void Initialize()
	{
		m_rigidbody = m_containerAttachedTo.View.GetComponent<Rigidbody2D>();
		m_transform = m_containerAttachedTo.View.GetComponent<Transform>();
	}

	public override void Update()
	{
		// Rotate object towards mouse position
		// Move forward\backward, steer right\left using WASD
		// Fire missiles using mouse click

		// keys bindings?
	}

	#endregion

	private void RotateTowardsMouse()
	{
		Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3( Input.mousePosition.x, 
		                                                       				Input.mousePosition.y, 
		                                                       				-Camera.main.transform.position.z));

		Vector3 dir = (mouseWorldPos - m_transform.position).normalized;
		m_transform.rotation = Quaternion.Lerp( m_transform.rotation, Quaternion.Euler(dir), Time.fixedDeltaTime * torqueSpeed );
	}

	private void Move()
	{
		// Engage the engines on the board
	}
}
