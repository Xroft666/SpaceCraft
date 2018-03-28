using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

public class DMagnet : Device 
{
	// Exportable variable
	public float m_magnetPower = 10f;
	
//	private Rigidbody2D myRigid;
	private Rigidbody myRigid;

	#region device's functions

	private IEnumerator Attract( DeviceQuery qry )//EventArgs args )
	{
		ContainerArgs cArgs = qry.Invoke() as ContainerArgs;

//		Rigidbody2D rigid = cArgs.container.View.GetComponent<Rigidbody2D>();
		Rigidbody rigid = cArgs.container.View.GetComponent<Rigidbody>();

		while( (cArgs.container.View.transform.position - m_container.View.transform.position).magnitude > 1f )
		{
			rigid.AddForce( (myRigid.position - rigid.position).normalized * m_magnetPower);

			yield return null;
		}

		rigid.velocity = Vector3.zero;

		yield break;
	}

	private IEnumerator Repulse( DeviceQuery qry )//EventArgs args )
	{
		ContainerArgs cArgs = qry.Invoke() as ContainerArgs;

//		Rigidbody2D rigid = cArgs.container.View.GetComponent<Rigidbody2D>();
		Rigidbody rigid = cArgs.container.View.GetComponent<Rigidbody>();

		rigid.AddForce( (rigid.position - myRigid.position).normalized * m_magnetPower * 10f, ForceMode.Impulse);


		yield break;
	}


	private IEnumerator Load( DeviceQuery qry )//EventArgs args )
	{
		
		ContainerArgs cArgs = qry.Invoke() as ContainerArgs;

		yield return new WaitForSeconds(2f);
		
		StorageObject( cArgs.container ) ;
		
		yield break;
	}
	
	private IEnumerator UnloadAll( DeviceQuery qry )//EventArgs args )
	{

		yield return new WaitForSeconds(2f);

		RemoveFromStorage();
		
		yield break;
	}

	#endregion

	#region Predecates 

	private bool IsStorageble( EventArgs args )
	{
		ContainerArgs cArgs = args as ContainerArgs;
		Asteroid aster = cArgs.container as Asteroid;

		if( aster == null )
		{
			Debug.LogWarning("Trying to magnet: " + cArgs.container.EntityName );
			return false;
		}

		float volume = aster.Containment.Amount;

		return volume <= 0.2f;
	}

	#endregion

	#region device's interface implementation

	public override void OnDeviceInstalled()
	{
		m_blueprint.AddAction("Attract", Attract );
		m_blueprint.AddAction("Repulse", Repulse);

		m_blueprint.AddAction("Load", Load );
		m_blueprint.AddAction("UnloadAll", UnloadAll );
		
		m_blueprint.AddCheck("IsStorageble", IsStorageble);
	}

	public override void OnDeviceUninstalled()
	{
		m_blueprint.RemoveAction("Attract" );
		m_blueprint.RemoveAction("Repulse");
		
		m_blueprint.RemoveAction("Load" );
		m_blueprint.RemoveAction("UnloadAll" );
		
		m_blueprint.RemoveCheck("IsStorageble");
	}

	public override void Initialize()
	{
//		myRigid = m_containerAttachedTo.View.GetComponent<Rigidbody2D>();
		myRigid = m_container.View.GetComponent<Rigidbody>();
	}

	public override void Update()
	{

//		
//		foreach( Rigidbody2D obj in m_repusleTargets )
//			obj.AddForce( (obj.position - myRigid.position).normalized * m_magnetPower );
	}

	#endregion

	private void StorageObject( Container container )
	{
		// Should extract device/resource from the containe and place it to the storage
		
		m_container.AddToCargo( container );
		WorldManager.UnspawnContainer( container );
	}
	
	private void RemoveFromStorage()
	{
		/// Considering that cargo will hold Devices and Resource entities,
		/// this should make containers for them and spawn them
		
		
	//	foreach( Entity ent in m_containerAttachedTo.GetCargoList() )
	//	{
	//		Container cont = ent as Container;
	//		if( cont != null )
	//			WorldManager.SpawnContainer( cont, m_containerAttachedTo.View.transform.position, Quaternion.identity );
	//		
	//		m_containerAttachedTo.RemoveFromCargo( ent );
	//	}

	//	for( int i = 0; i < m_containerAttachedTo.m_cargo.m_items.Count; i++ )
	//	{
	//		switch( m_containerAttachedTo.m_cargo.m_items[i].resource.Type )
	//		{
	//		case Entity.EntityType.Item:
	//
	//			for( int j = 0; j < m_containerAttachedTo.m_cargo.m_items[i].curItemCount; j++ )
	//			{
	//				m_containerAttachedTo.m_cargo.RemoveItem( m_containerAttachedTo.m_cargo.m_items[i].resource.EntityName );
	//				WorldManager.SpawnContainer( m_containerAttachedTo.m_cargo.m_items[i].resource as Container, m_containerAttachedTo.View.transform.position, Quaternion.identity );
	//			}
	//			break;
	//		case Entity.EntityType.Crumby:
	//		case Entity.EntityType.Liquid:
	//
	//
	//
	//			break;
	//		}
	//	}
	}

    public override string ToString()
    {
        return "Magnet";
    }
}
