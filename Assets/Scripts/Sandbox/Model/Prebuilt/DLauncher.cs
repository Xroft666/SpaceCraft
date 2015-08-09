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
	private string m_projectileName;

	#region device's functions

	public void SetProjectile(params object[] objects)
	{
		m_projectileName = (string) objects[0];
	}

	public void Fire(params object[] objects)
	{
		foreach( Entity ent in m_containerAttachedTo.GetCargoList() )
		{
			Container cont = ent as Container;
			if( cont != null && ent.EntityName == m_projectileName )
			{
				WorldManager.SpawnContainer(cont, 
				                            m_containerAttachedTo.View.transform.position,
				                            m_containerAttachedTo.View.transform.rotation);
			}
		}
	}
	#endregion

	#region device's interface implementation

	public override void OnDeviceInstalled()
	{
	//	AddEvent( "OnTimerTrigger", new UnityEvent() );
		AddFunction("SetProjectile", SetProjectile );
		AddFunction("Fire", Fire );
	}

	public override void Initialize()
	{

	}

	public override void Update()
	{

	}

	#endregion
	
}
