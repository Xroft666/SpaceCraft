using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;


public class DEngine : Device 
{
	private Rigidbody2D m_rigidbody = null;
	private bool isEngaged = false;

	// temporary variable. Should be changed to something more physical realistic
	private float speed = 10f;

	#region device's functions

	public void EngageEngine( params Entity[] objects )
	{
		isEngaged = true;
	}

	public void DisengageEngine( params Entity[] objects )
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


//		AddEvent( "OnTimerTrigger", new UnityEvent() );
		AddFunction("EngageEngine", EngageEngine );
		AddFunction("DisengageEngine", DisengageEngine );

		m_rigidbody = m_containerAttachedTo.View.gameObject.AddComponent<Rigidbody2D>();
	}

	public override void Initialize()
	{

	}

	public override void Update()
	{
		// Move the object and consume fuel

		Vector3 dir = m_rigidbody.transform.up * speed * Time.fixedDeltaTime;
		if( isEngaged )
			m_rigidbody.MovePosition ( m_rigidbody.position + new Vector2( dir.x, dir.y) );
	}

	public override void Delete()
	{
		GameObject.Destroy(m_rigidbody);
	}

	#endregion
}
