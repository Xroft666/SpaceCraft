﻿using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

public class DRanger : Device 
{
	// Exportable variable
	public float detectionRange = 3f;

	private List<ContainerView> m_targets = new List<ContainerView>();

//	private CircleCollider2D m_collider = null;
	private SphereCollider m_collider = null;

	public override void OnDeviceInstalled()
	{
		AddEvent( "OnRangerEntered", null );
		AddEvent( "OnRangerEscaped", null );

		AddAction("DesignateClosestTarget", DesignateClosestTarget);	
		AddCheck("IsAnyTarget", IsAnyTarget );	
		AddQuery("CurrentTargetPosition", CurrentTargetPosition);
		AddQuery("CurrentTargetContainer", CurrentTargetContainer);
	}

	public override IEnumerator ActivateDevice ( EventArgs args )
	{
		m_isActive = true;
		if( m_collider != null )
			m_collider.enabled = true;

		m_targets.Clear();
		yield break;
	} 

	public override IEnumerator DeactivateDevice( EventArgs args)
	{
		m_isActive = false;
		if( m_collider != null )
			m_collider.enabled = false;

		m_targets.Clear();

		yield break;
	}

	private IEnumerator DesignateClosestTarget( EventArgs args )
	{
		ContainerView thisContainer = m_containerAttachedTo.View;
		
		m_targets.Sort( ( ContainerView x, ContainerView y ) =>
		               {
			float distance1 = (thisContainer.transform.position - x.transform.position).magnitude;
			float distance2 = (thisContainer.transform.position - y.transform.position).magnitude;
			
			return distance1.CompareTo( distance2 );
		});
		
		yield break;
	}

	#region Predecates 
	
	public bool IsAnyTarget( EventArgs args )
	{
		return m_targets.Count > 0;
	}
	
	#endregion
	
	#region Queries
	
	public PositionArgs CurrentTargetPosition()
	{
		if( m_targets.Count == 0 )
			return null;
		
		return new PositionArgs(){ position = m_targets[0].transform.position };
	}

	public ContainerArgs CurrentTargetContainer()
	{
		if( m_targets.Count == 0 )
			return null;

		return new ContainerArgs(){ container = m_targets[0].m_contain };
	}
	
	#endregion

	public override void Initialize ()
	{
		GameObject rangerGO = new GameObject(EntityName);
		
		rangerGO.transform.SetParent( m_containerAttachedTo.View.transform, false );

//		Rigidbody2D rigid = rangerGO.AddComponent<Rigidbody2D>();
		Rigidbody rigid = rangerGO.AddComponent<Rigidbody>();
		rigid.isKinematic = true;

//		m_collider = rangerGO.AddComponent<CircleCollider2D>();
		m_collider = rangerGO.AddComponent<SphereCollider>();
		m_collider.isTrigger = true;
		m_collider.radius = detectionRange;
		m_collider.enabled = m_isActive;
		
//		EventTrigger2DHandler trigger = rangerGO.AddComponent<EventTrigger2DHandler>();
		EventTriggerHandler trigger = rangerGO.AddComponent<EventTriggerHandler>();
		trigger.onTriggerEnter += OnColliderEntered;
		trigger.onTriggerExit += OnColliderEscaped;
	}

	public override void Destroy()
	{
//		Component.Destroy(m_collider);
		GameObject.Destroy( m_collider.gameObject );
	}

	private void OnColliderEntered( Collider other )//Collider2D other )
	{

		if( !IsColliderMine( other ))
		{
			ContainerView othersView = other.gameObject.GetComponent<ContainerView>();
			if( othersView == null )
			{
				Debug.LogWarning("Unexpected interaction with: " + other.gameObject.name);
				return;
			}

			if(  m_containerAttachedTo.View.m_owner == 0 || othersView.m_owner != m_containerAttachedTo.View.m_owner )
			{
				DeviceEvent onEnter = GetEvent("OnRangerEntered");
				if( onEnter != null )
					//m_containerAttachedTo.IntegratedDevice.ScheduleEvent( onEnter, null)
					//ScheduleEvent( onEnter, null);
					//onEnter(null);
					Job.make( onEnter(null), true);

				m_targets.Add( othersView );
				
				othersView.m_contain.onDestroy += () => 
				{
					m_targets.Remove( othersView );
				};
			}
		}
	}

	private void OnColliderEscaped( Collider other )//Collider2D other )
	{
		if( !IsColliderMine( other ))
		{
			ContainerView othersView = other.gameObject.GetComponent<ContainerView>();
			if( othersView == null )
			{
				Debug.LogWarning("Unexpected interaction with: " + other.gameObject.name);
				return;
			}

			DeviceEvent onExit = GetEvent("OnRangerEscaped");
			if( onExit != null )
			//	m_containerAttachedTo.IntegratedDevice.ScheduleEvent( onExit, null);
			//	ScheduleEvent( onExit, null);
			//	onExit(null);
				Job.make( onExit(null), true);

			m_targets.Remove( othersView );
		}
	}

	private bool IsColliderMine( Collider collider )//Collider2D collider)
	{
		// check all the colliders on the container
		return m_containerAttachedTo.View.gameObject == collider.transform.root.gameObject;
	}
}
