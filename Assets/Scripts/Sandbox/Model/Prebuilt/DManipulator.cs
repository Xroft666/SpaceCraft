using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

public class DManipulator : Device 
{

	public float fireRate = 1f;
	private float m_timer = 0f;

	#region device's functions

	private void Load( params object[] data )
	{
		if( !IsEligableToShoot() )
			return;
		
		m_timer = 0f;

		StorageObject( (Container) data[0]) ;
	}

	private void UnloadAll( params object[] data )
	{
		if( !IsEligableToShoot() )
			return;
		
		m_timer = 0f;

		RemoveFromStorage();
	}
	

	#endregion

	#region Predecates 



	#endregion

	#region device's interface implementation

	public override void OnDeviceInstalled()
	{
		AddAction("Load", Load );
		AddAction("UnloadAll", UnloadAll );
	}

	public override void Initialize()
	{

	}

	public override void Update()
	{
		m_timer += Time.deltaTime; 
	}

	#endregion

	private void StorageObject( Container container )
	{
		// Should extract device/resource from the containe and place it to the storage

		m_containerAttachedTo.AddToCargo( container );
	}

	private void RemoveFromStorage()
	{
		/// Considering that cargo will hold Devices and Resource entities,
		/// this should make containers for them and spawn them


		foreach( Entity ent in m_containerAttachedTo.GetCargoList() )
		{
			Container cont = ent as Container;
			if( cont != null )
				WorldManager.SpawnContainer( cont, m_containerAttachedTo.View.transform.position, Quaternion.identity );

			m_containerAttachedTo.RemoveFromCargo( ent );
		}
	}

	private bool IsEligableToShoot()
	{
		return m_timer > 1f / fireRate;
	}
}
