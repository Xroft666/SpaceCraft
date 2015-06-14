using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

public class DRanger : Device 
{
	private CircleCollider2D m_collider = null;

	#region Functions

	public void SetRange( float range )
	{
		m_collider.radius = range;
	}

	#endregion

	public override void OnDeviceInstalled()
	{
		AddEvent( "OnRangerEntered", new UnityEvent() );
		AddEvent( "OnRangerEscaped", new UnityEvent() );

		m_collider = m_containerAttachedTo.Representation.gameObject.AddComponent<CircleCollider2D>();
	}

	public override void Initialize()
	{

	}

	public override void Update()
	{

	}

	public override void Delete()
	{
		GameObject.Destroy(m_collider);
	}

	public override void OnObjectEntered() 
	{
		GetEvent("OnRangerEntered").Invoke();
    }
	
	public override void OnObjectEscaped() 
	{
		GetEvent("OnRangerEscaped").Invoke();
    }
}
