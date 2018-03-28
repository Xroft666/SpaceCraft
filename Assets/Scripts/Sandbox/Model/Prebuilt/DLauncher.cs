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

	public IEnumerator Fire( DeviceQuery qry )//EventArgs args )
	{
		Entity projectileEntity = null;
		ContainerView projectile = null;

		Ship cont = m_container.m_cargo.GetItem(m_projectileName) as Ship;
		if( cont != null )
		{
			projectileEntity = cont as Entity;
			projectile =  WorldManager.SpawnContainer(cont, 
			                            m_container.View.transform.position + m_container.View.transform.forward,
			                            m_container.View.transform.rotation,
			                            m_container.View.m_owner );

			Collider collider = projectile.GetComponent<Collider>();

			Rigidbody rigid = projectile.GetComponent<Rigidbody>();
			rigid.velocity = m_container.View.GetComponent<Rigidbody>().velocity;

			projectile.transform.Find("body").localScale = new Vector3(0.25f, 1f, 1f);

			Ship containerController = projectile.m_contain as Ship;
			if(containerController != null )
			{
				containerController.m_device.m_isActive = true;
				containerController.m_device.Initialize();
			}
		}

		if( projectileEntity != null )
		{
			m_container.RemoveFromCargo( projectileEntity.EntityName );

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
		m_blueprint.AddAction("Fire", Fire );
	}

	public override void OnDeviceUninstalled()
	{
		m_blueprint.RemoveAction("Fire" );
	}

	public override void Initialize()
	{

	}

	public override void Update()
	{

	}

    #endregion

    public override string ToString()
    {
        return "Launcher";
    }
}
