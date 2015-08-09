using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

public class DDetonator : Device 
{
	// Exportable and temporar variable
	public float detonateForce = 5f;
	public float explosionRadius = 1f;

	#region Functions

	public void DetonateExplosives(params object[] objects)
	{
		WorldManager.UnspawnContainer( m_containerAttachedTo );

		// Do big splosion
		GameObject explosion = new GameObject("Explosion Area");
		CircleCollider2D collider = explosion.AddComponent<CircleCollider2D>();
		EventTrigger2DHandler handler = explosion.AddComponent<EventTrigger2DHandler>();

		explosion.transform.position = m_containerAttachedTo.View.transform.position;
		collider.radius = explosionRadius;
		collider.isTrigger = true;

		handler.onTriggerEnter += OnObjectInExplosionArea;

		GameObject.Destroy( explosion, 0.1f );
	}

	#endregion

	public override void OnDeviceInstalled()
	{
		AddAction("Detonate", DetonateExplosives );
	}	

	public override void Initialize()
	{
		EventTrigger2DHandler trigger = m_containerAttachedTo.View.gameObject.AddComponent<EventTrigger2DHandler>();
		trigger.onTriggerEnter += OnCollideWithSomething;
	}

	private void OnObjectInExplosionArea( Collider2D other )
	{
		Vector3 outwardsDir = other.transform.position - m_containerAttachedTo.View.transform.position;
		Rigidbody2D rigid = other.gameObject.GetComponent<Rigidbody2D>();

		rigid.AddForce( outwardsDir.normalized * detonateForce * outwardsDir.magnitude / explosionRadius, ForceMode2D.Impulse );
	}

	private void OnCollideWithSomething( Collider2D other )
	{
		DetonateExplosives();
	}
}
