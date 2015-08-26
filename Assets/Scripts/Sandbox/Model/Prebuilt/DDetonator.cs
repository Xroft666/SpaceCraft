using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

public class DDetonator : Device 
{
	// Exportable and temporar variable
	public float detonateForce = 5f;
	public float explosionRadius = 1.5f;

	#region Functions

	public void DetonateExplosives(params object[] objects)
	{
		WorldManager.UnspawnContainer( m_containerAttachedTo );

		// Do big splosion
		GameObject explosion = new GameObject("Explosion Area");
		explosion.layer = 11;

		CircleCollider2D collider = explosion.AddComponent<CircleCollider2D>();
		EventTrigger2DHandler handler = explosion.AddComponent<EventTrigger2DHandler>();

		explosion.transform.position = m_containerAttachedTo.View.transform.position;
		collider.radius = explosionRadius;
		collider.isTrigger = true;

		handler.onTriggerEnter += OnObjectInExplosionArea;

		TransformMarker explosionGizmo = explosion.AddComponent<TransformMarker>();
		explosionGizmo.m_color = Color.red;
		explosionGizmo.m_radius = explosionRadius;

		GameObject.Destroy( explosion, 0.1f );
	}

	#endregion

	#region Device interface

	public override void OnDeviceInstalled()
	{
		AddAction("Detonate", DetonateExplosives );
	}	

	public override void Initialize()
	{

	}

	public override void Destroy ()
	{

	}

	#endregion

	private void OnObjectInExplosionArea( Collider2D other )
	{
		ContainerView view = other.GetComponent<ContainerView>();
		if( view == null )
			return;

		view.m_contain.TakeDamage(detonateForce, explosionRadius, m_containerAttachedTo.View.transform.position);


		Vector3 outwardsDir = other.transform.position - m_containerAttachedTo.View.transform.position;
		Rigidbody2D rigid = other.gameObject.GetComponent<Rigidbody2D>();

		rigid.AddForce( outwardsDir.normalized * detonateForce * outwardsDir.magnitude / explosionRadius, ForceMode2D.Impulse );
	}

}
