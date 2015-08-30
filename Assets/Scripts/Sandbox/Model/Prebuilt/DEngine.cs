using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;


public class DEngine : Device 
{

	// temporary variable. Should be changed to something more physical realistic
	public float speed = 100f;
	public Vector3 m_lookDirection = Vector3.up;
	public Space m_space;

	private Rigidbody2D m_rigidbody = null;

	#region device's functions


	public IEnumerator MoveForward( EventArgs args )
	{
		ApplyForce();

		yield break;
	}

	#endregion

	#region device's interface implementation

	public override void OnDeviceInstalled()
	{
		// Pathfinding functionality?
		// Meta movoement functions?
		// Steering ability?

		base.OnDeviceInstalled();

		AddAction("MoveForward", MoveForward );
	}

	public override void Initialize()
	{
		m_rigidbody = m_containerAttachedTo.View.gameObject.GetComponent<Rigidbody2D>();

	}

	public override void Update()
	{
		if( m_isActive )
			m_containerAttachedTo.IntegratedDevice.ScheduleEvent( MoveForward, null );
	}

	public override void Destroy()
	{
	
	}

	#endregion

	private void ApplyForce()
	{
		// Move the object and consume fuel

		Vector3 dir = m_lookDirection.normalized;
		if( m_space == Space.Self )
			dir = m_containerAttachedTo.View.transform.TransformDirection( m_lookDirection ).normalized;

		m_rigidbody.AddForce( dir * speed * Time.deltaTime, ForceMode2D.Force );
	}
}
