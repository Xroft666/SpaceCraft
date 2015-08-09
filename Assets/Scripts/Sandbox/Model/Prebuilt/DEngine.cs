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

	#endregion

	#region device's interface implementation

	public override void OnDeviceInstalled()
	{
		// Pathfinding functionality?
		// Meta movoement functions?
		// Steering ability?


		AddFunction("EngageEngine", EngageEngine );
		AddFunction("DisengageEngine", DisengageEngine );


	}

	public override void Initialize()
	{
		m_rigidbody = m_containerAttachedTo.View.gameObject.AddComponent<Rigidbody2D>();
		m_rigidbody.gravityScale = 0f;
	}

	public override void Update()
	{
		// Move the object and consume fuel

		Vector3 dir = m_containerAttachedTo.View.transform.up * speed * Time.fixedDeltaTime;
		if( isEngaged )
			m_rigidbody.MovePosition ( m_rigidbody.position + new Vector2( dir.x, dir.y) );
	}

	public override void Destroy()
	{
		Component.Destroy(m_rigidbody);
	}

	#endregion
}
