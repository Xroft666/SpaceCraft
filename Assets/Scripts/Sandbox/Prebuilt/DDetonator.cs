using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

public class DDetonator : Device 
{
	#region Functions

	public IEnumerator DetonateExplosives()
	{
		// Here we could calculate the energy that being produced
		// and make a explosion range of that specific amount

		// we need to go through container's storage and evaluate how much
		// explosive resources it contains

		(m_containerAttachedTo as IDamagable).Destroy();

		yield return null;
	}

	#endregion

	public override void OnDeviceInstalled()
	{
		AddFunction("Detonate", Job.make(DetonateExplosives()) );
	}

	public override void Initialize()
	{

	}

	public override void Update()
	{

	}

	public override void Delete()
	{
	
	}

	public override void OnObjectEntered() 
	{

    }
	
	public override void OnObjectEscaped() 
	{

    }
}
