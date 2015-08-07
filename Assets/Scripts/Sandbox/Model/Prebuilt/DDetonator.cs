using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

public class DDetonator : Device 
{
	#region Functions

	public void DetonateExplosives(params object[] objects)
	{
		m_containerAttachedTo.Destroy();
	}

	#endregion

	public override void OnDeviceInstalled()
	{
		AddFunction("Detonate", DetonateExplosives );//Job.make(DetonateExplosives()) );
	}	
}
