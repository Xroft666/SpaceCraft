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

	private List<ContainerView> m_targets = new List<ContainerView>();

	private SphereCollider m_collider = null;

	public override void OnDeviceInstalled()
	{
		m_blueprint.AddTrigger( "OnRangerEntered", null );
		m_blueprint.AddTrigger( "OnRangerEscaped", null );

		m_blueprint.AddAction("DesignateClosestTarget", DesignateClosestTarget);	
		m_blueprint.AddCheck("IsAnyTarget", IsAnyTarget );	
		m_blueprint.AddQuery("CurrentTargetPosition", CurrentTargetPosition);
		m_blueprint.AddQuery("CurrentTargetContainer", CurrentTargetContainer);
	}

	public override void OnDeviceUninstalled()
	{
		m_blueprint.RemoveTrigger( "OnRangerEntered" );
		m_blueprint.RemoveTrigger( "OnRangerEscaped" );
		
		m_blueprint.RemoveAction("DesignateClosestTarget");	
		m_blueprint.RemoveCheck("IsAnyTarget" );	
		m_blueprint.RemoveQuery("CurrentTargetPosition");
		m_blueprint.RemoveQuery("CurrentTargetContainer");
	}

	public override void ActivateDevice ( )
	{
		base.ActivateDevice ();

		if( m_collider != null )
			m_collider.enabled = true;

		m_targets.Clear();
	} 

	public override void DeactivateDevice( )
	{
		base.DeactivateDevice ();
		if( m_collider != null )
			m_collider.enabled = false;

		m_targets.Clear();
	}

	private IEnumerator DesignateClosestTarget( DeviceQuery qry )
	{
		ContainerView thisContainer = m_container.View;
		
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
	
	public ArgsObject CurrentTargetPosition()
	{
		if( m_targets.Count == 0 )
			return null;
		
		return new ArgsObject(){ obj = m_targets[0].transform.position };
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
		
		rangerGO.transform.SetParent( m_container.View.transform, false );

		Rigidbody rigid = rangerGO.AddComponent<Rigidbody>();
		rigid.isKinematic = true;

		m_collider = rangerGO.AddComponent<SphereCollider>();
		m_collider.isTrigger = true;
		m_collider.radius = detectionRange;
		m_collider.enabled = m_isActive;

		EventTriggerHandler trigger = rangerGO.AddComponent<EventTriggerHandler>();
		trigger.onTriggerEnter += OnColliderEntered;
		trigger.onTriggerExit += OnColliderEscaped;
	}

	public override void Destroy()
	{
		GameObject.Destroy( m_collider.gameObject );
	}

	private void OnColliderEntered( Collider other )
	{
		if( !IsColliderMine( other ))
		{
			ContainerView othersView = other.gameObject.GetComponentInParent<ContainerView>();
			if( othersView == null )
			{
				Debug.LogWarning("Unexpected interaction with: " + other.gameObject.name, other.gameObject);
				return;
			}

			if(  m_container.View.m_owner == 0 || othersView.m_owner != m_container.View.m_owner )
			{
				DeviceTrigger onEnter = m_blueprint.GetTrigger("OnRangerEntered");
				if( onEnter != null )
					onEnter();

				m_targets.Add( othersView );
				
				othersView.m_contain.onDestroy += () => 
				{
					m_targets.Remove( othersView );
				};
			}
		}
	}

	private void OnColliderEscaped( Collider other )
	{
		if( !IsColliderMine( other ))
		{
			ContainerView othersView = other.gameObject.GetComponentInParent<ContainerView>();
			if( othersView == null )
			{
				Debug.LogWarning("Unexpected interaction with: " + other.gameObject.name);
				return;
			}

			DeviceTrigger onExit = m_blueprint.GetTrigger("OnRangerEscaped");
			if( onExit != null )
				onExit.Invoke();

			m_targets.Remove( othersView );
		}
	}

	private bool IsColliderMine( Collider collider )
	{
		// check all the colliders on the container
		return m_container.View.gameObject == collider.transform.root.gameObject;
	}
}
