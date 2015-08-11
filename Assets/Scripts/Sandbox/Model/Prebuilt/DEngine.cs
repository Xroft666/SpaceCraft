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
	public float speed = 1f;

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

	public void Move( params object[] objects )
	{
		MoveForward();
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
		AddAction("Move", Move );

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
		Component.Destroy(m_rigidbody);
	}

	#endregion

	private void MoveForward()
	{
		// Move the object and consume fuel

		Vector3 dir = m_containerAttachedTo.View.transform.up * speed * Time.deltaTime;

		m_rigidbody.AddForce( dir * speed * 100f, ForceMode2D.Force );
	}
}
