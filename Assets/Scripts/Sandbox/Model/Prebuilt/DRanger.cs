using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

public class DRanger : Device 
{
	// Exportable variable
	public float detectionRange = 3f;

	private CircleCollider2D m_collider = null;

	public override void OnDeviceInstalled()
	{
		AddEvent( "OnRangerEntered", null );
		AddEvent( "OnRangerEscaped", null );
	}

	public override IEnumerator ActivateDevice ( EventArgs args )
	{
		m_isActive = true;
		if( m_collider != null )
			m_collider.enabled = true;

		yield break;
	} 

	public override IEnumerator DeactivateDevice( EventArgs args)
	{
		m_isActive = false;
		if( m_collider != null )
			m_collider.enabled = false;

		yield break;
	}

	public override void Initialize ()
	{
		GameObject rangerGO = new GameObject(EntityName);
		
		rangerGO.transform.SetParent( m_containerAttachedTo.View.transform, false );

		Rigidbody2D rigid = rangerGO.AddComponent<Rigidbody2D>();
		rigid.isKinematic = true;

		m_collider = rangerGO.AddComponent<CircleCollider2D>();
		m_collider.isTrigger = true;
		m_collider.radius = detectionRange;
		m_collider.enabled = m_isActive;
		
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
		if( onEnter != null && !IsColliderMine( other ))
		{
			ContainerView othersView = other.gameObject.GetComponent<ContainerView>();
			if( othersView == null )
			{
				Debug.LogWarning("Unexpected interaction with: " + other.gameObject.name);
				return;
			}

			m_containerAttachedTo.IntegratedDevice.ScheduleEvent( onEnter, new ContainerArgs(){ container = othersView.m_contain } );
		}
	}

	private void OnColliderEscaped( Collider2D other )
	{
		DeviceEvent onExit = GetEvent("OnRangerEscaped");
		if( onExit != null && !IsColliderMine( other ))
		{
			ContainerView othersView = other.gameObject.GetComponent<ContainerView>();
			if( othersView == null )
			{
				Debug.LogWarning("Unexpected interaction with: " + other.gameObject.name);
				return;
			}
		
			m_containerAttachedTo.IntegratedDevice.ScheduleEvent( onExit, new ContainerArgs(){ container = othersView.m_contain } );
		}
	}

	private bool IsColliderMine(Collider2D collider)
	{
		// check all the colliders on the container
		return m_containerAttachedTo.View.gameObject == collider.gameObject;
	}
}
