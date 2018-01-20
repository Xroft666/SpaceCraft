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

	public IEnumerator DetonateExplosives( DeviceQuery qry )
	{
		WorldManager.UnspawnContainer( m_container );

		// Do big splosion
		GameObject explosion = new GameObject("Explosion Area");
		explosion.layer = 11;

		SphereCollider collider = explosion.AddComponent<SphereCollider>();
		EventTriggerHandler handler = explosion.AddComponent<EventTriggerHandler>();

		explosion.transform.position = m_container.View.transform.position;
		collider.radius = explosionRadius;
		collider.isTrigger = true;

		handler.onTriggerEnter += OnObjectInExplosionArea;

		TransformMarker explosionGizmo = explosion.AddComponent<TransformMarker>();
		explosionGizmo.m_color = Color.red;
		explosionGizmo.m_radius = explosionRadius;

		yield return new WaitForFixedUpdate ();

		GameObject.Destroy( explosion);
	}

	#endregion

	#region Device interface

	public override void OnDeviceInstalled()
	{
		m_blueprint.AddAction("Detonate", DetonateExplosives );
	}

	public override void OnDeviceUninstalled()
	{
		m_blueprint.RemoveAction("Detonate");
	}

	#endregion

	private void OnObjectInExplosionArea( Collider other )
	{
		ContainerView view = other.GetComponent<ContainerView>();
		if( view == null )
			return;

		(view.m_contain as IDamagable).TakeDamage(detonateForce * 2f, explosionRadius, m_container.View.transform.position);


		Vector3 outwardsDir = other.transform.position - m_container.View.transform.position;
		Rigidbody rigid = other.gameObject.GetComponent<Rigidbody>();

		rigid.AddForce( outwardsDir.normalized * detonateForce * outwardsDir.magnitude / explosionRadius, ForceMode.Impulse);
	}

}
