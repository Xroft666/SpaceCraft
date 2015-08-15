using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;


public class DEngine : Device 
{
	// Exported value just as in inspector
	public bool isEngaged = false;
	// temporary variable. Should be changed to something more physical realistic
	public float speed = 100f;
	public Vector3 m_lookDirection = Vector3.up;
	public Space m_space;

	private Rigidbody2D m_rigidbody = null;

	#region device's functions

	public void EngageEngine( params object[] objects )
	{
		isEngaged = true;
	}

	public void DisengageEngine( params object[] objects )
	{
		isEngaged = false;
	}

	public void MoveForward( params object[] objects )
	{
		ApplyForce();
	}

	#endregion

	#region device's interface implementation

	public override void OnDeviceInstalled()
	{
		// Pathfinding functionality?
		// Meta movoement functions?
		// Steering ability?


		AddAction("EngageEngine", EngageEngine );
		AddAction("DisengageEngine", DisengageEngine );

		AddAction("MoveForward", MoveForward );
	}

	public override void Initialize()
	{
		m_rigidbody = m_containerAttachedTo.View.gameObject.GetComponent<Rigidbody2D>();

	}

	public override void Update()
	{
		if( isEngaged )
			MoveForward();
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
