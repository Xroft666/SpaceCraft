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
		WorldManager.UnspawnContainer( m_containerAttachedTo );
	}

	#endregion

	public override void OnDeviceInstalled()
	{
		AddAction("Detonate", DetonateExplosives );
	}	
}
