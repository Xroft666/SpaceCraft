using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

public class DLauncher : Device 
{

	#region device's functions

	public void Fire()
	{
	// Pick and spawn a missile that is stored in the cargo
	// Call Initialize on it

	}

	#endregion

	#region device's interface implementation

	public override void OnDeviceInstalled()
	{
	//	AddEvent( "OnTimerTrigger", new UnityEvent() );
	//	AddFunction("SetTarget", SetTarget );
	//	AddFunction("SearchForClosestTarget", SearchForClosestTarget );
	}

	public override void Initialize()
	{

	}

	public override void Update()
	{

	}

	#endregion
	
}
