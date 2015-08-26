using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

public class DLauncher : Device 
{
	// temporary variable
	// for now we connect Consumable with Consumers by names
	// which should be somewhat different
	// Exportable variable
	public string m_projectileName;
	public float fireRate = 0.5f;	// projectiles a second

	private float m_timer = 0f;

	public void SetProjectile(params object[] objects)
	{
		m_projectileName = (string) objects[0];
	}

	#region device's functions

	public void Fire(params object[] objects)
	{
		if( !IsEligableToShoot() )
			return;

		m_timer = 0f;

		Entity projectileEntity = null;
		foreach( Entity ent in m_containerAttachedTo.GetCargoList() )
		{
			Container cont = ent as Container;
			if( cont != null && ent.EntityName == m_projectileName )
			{
				projectileEntity = ent;
				ContainerView projectile =  WorldManager.SpawnContainer(cont, 
				                            m_containerAttachedTo.View.transform.position,
				                            m_containerAttachedTo.View.transform.rotation);

//				projectile.gameObject.layer = 10;

				Collider2D collider = projectile.GetComponent<Collider2D>();
//				collider.isTrigger = true;

				Rigidbody2D rigid = projectile.GetComponent<Rigidbody2D>();
				rigid.velocity = m_containerAttachedTo.View.GetComponent<Rigidbody2D>().velocity;

				projectile.transform.FindChild("body").localScale = new Vector3(0.25f, 1f, 1f);

				break;
			}
		}

		if( projectileEntity != null )
			m_containerAttachedTo.RemoveFromCargo( projectileEntity );
	}
	#endregion

	#region device's interface implementation

	public override void OnDeviceInstalled()
	{
		AddAction("Fire", Fire );
	}

	public override void Initialize()
	{

	}

	public override void Update()
	{
		m_timer += Time.deltaTime; 
	}

	#endregion

	private bool IsEligableToShoot()
	{
		return m_timer > 1f / fireRate;
	}
}
