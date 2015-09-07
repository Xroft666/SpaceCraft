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


	#region device's functions

	public IEnumerator Fire( EventArgs args )
	{
		Entity projectileEntity = null;
		ContainerView projectile = null;

		Ship cont = m_containerAttachedTo.m_cargo.GetItem(m_projectileName) as Ship;
		if( cont != null )
		{
			projectileEntity = cont as Entity;
			projectile =  WorldManager.SpawnContainer(cont, 
			                            m_containerAttachedTo.View.transform.position + m_containerAttachedTo.View.transform.up,
			                            m_containerAttachedTo.View.transform.rotation,
			                            m_containerAttachedTo.View.m_owner );

			Collider2D collider = projectile.GetComponent<Collider2D>();

			Rigidbody2D rigid = projectile.GetComponent<Rigidbody2D>();
			rigid.velocity = m_containerAttachedTo.View.GetComponent<Rigidbody2D>().velocity;

			projectile.transform.FindChild("body").localScale = new Vector3(0.25f, 1f, 1f);

		}

		if( projectileEntity != null )
		{
			m_containerAttachedTo.RemoveFromCargo( projectileEntity.EntityName );

			while( !WorldManager.IsContainerDestroyed(projectile) )
			{
				yield return null;
			}
		}

	}
	#endregion

	private bool CanFire()
	{
		return true;
	}

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

	}

	#endregion
}
