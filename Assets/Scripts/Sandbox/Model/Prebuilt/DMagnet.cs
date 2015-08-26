using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

public class DMagnet : Device 
{
	// Exportable variable
	public List<ContainerView> m_targets = new List<ContainerView>();


	#region device's functions

	private void AddTarget( params object[] data )
	{
		m_targets.Add( (data[0] as Container).View );
	}

	private void RemoveTarget( params object[] data )
	{
		m_targets.Remove( (data[0] as Container).View );
	}
	

	#endregion

	#region Predecates 

	public bool IsAnyTarget()
	{
		return m_targets.Count > 0;
	}

	#endregion

	#region device's interface implementation

	public override void OnDeviceInstalled()
	{
		AddEvent( "TargetPosition", null );

		AddAction("AddTarget", AddTarget );
		AddAction("RemoveTarget", RemoveTarget);

		AddCheck("IsAnyTarget", IsAnyTarget );
	}

	public override void Initialize()
	{

	}

	public override void Update()
	{

	}

	#endregion


}
