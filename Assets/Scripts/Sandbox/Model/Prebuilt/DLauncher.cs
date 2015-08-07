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

	public void SetProjectile(params Entity[] objects)
	{
	//	m_projectileName = (string) objects[0];
	}

	public void Fire(params Entity[] objects)
	{
	// Pick and spawn a missile that is stored in the cargo
	// Call Initialize on it

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
