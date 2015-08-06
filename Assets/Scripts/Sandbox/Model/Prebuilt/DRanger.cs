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
		AddEvent( "OnRangerEntered", null );
		AddEvent( "OnRangerEscaped", null );

		m_collider = m_containerAttachedTo.View.gameObject.AddComponent<CircleCollider2D>();
	}

	public override void Delete()
	{
		GameObject.Destroy(m_collider);
	}

	public override void OnObjectEntered( Container container ) 
	{
		GetEvent("OnRangerEntered").Invoke( container );
    }
	
	public override void OnObjectEscaped( Container container ) 
	{
		GetEvent("OnRangerEscaped").Invoke( container );
    }
}
