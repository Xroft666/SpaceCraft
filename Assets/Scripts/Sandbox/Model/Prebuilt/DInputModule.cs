using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

public class DInputModule : Device 
{
	// expose this variable in logic UI (like in inspector)
	public KeyCode m_keyCode; 


	#region device's interface implementation

	public override void OnDeviceInstalled()
	{
		AddEvent( "OnInputPressed", null );
		AddEvent( "OnInputReleased", null );
	}
	

	public override void Update()
	{
		if( Input.GetKeyDown(m_keyCode) )
		{
			DeviceEvent onPressed = GetEvent("OnInputPressed");
			if( onPressed != null )
				onPressed.Invoke(m_keyCode);
		}
		if( Input.GetKeyUp(m_keyCode) )
		{
			DeviceEvent onReleased = GetEvent("OnInputReleased");
			if( onReleased != null )
				onReleased.Invoke(m_keyCode);
		}
	}

	#endregion

}
