using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

public class DRanger : Device 
{
	// Exportable variable
	public float detectionRange = 3f;

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

	}

	public override void Initialize ()
	{
		GameObject rangerGO = new GameObject("Ranger");
		rangerGO.layer = 10;
		
		rangerGO.transform.SetParent( m_containerAttachedTo.View.transform, false );

		Rigidbody2D rigid = rangerGO.AddComponent<Rigidbody2D>();
		rigid.isKinematic = true;

		m_collider = rangerGO.AddComponent<CircleCollider2D>();
		m_collider.isTrigger = true;
		m_collider.radius = detectionRange;
		
		EventTrigger2DHandler trigger = rangerGO.AddComponent<EventTrigger2DHandler>();
		trigger.onTriggerEnter += OnColliderEntered;
		trigger.onTriggerExit += OnColliderEscaped;
	}

	public override void Destroy()
	{
//		Component.Destroy(m_collider);
		GameObject.Destroy( m_collider.gameObject );
	}

	private void OnColliderEntered( Collider2D other )
	{
		DeviceEvent onEnter = GetEvent("OnRangerEntered");
		if( onEnter != null )
		{
			ContainerView othersView = other.gameObject.GetComponent<ContainerView>();
			if( othersView == null )
				Debug.LogError("Unexpected interaction with: " + other.gameObject.name);
			onEnter.Invoke( othersView.m_contain );
		}
	}

	private void OnColliderEscaped( Collider2D other )
	{
		DeviceEvent onExit = GetEvent("OnRangerEscaped");
		if( onExit != null )
		{
			ContainerView othersView = other.gameObject.GetComponent<ContainerView>();
			if( othersView == null )
				Debug.LogError("Unexpected interaction with: " + other.gameObject.name);
			onExit.Invoke( othersView.m_contain );
		}
	}
	
}
