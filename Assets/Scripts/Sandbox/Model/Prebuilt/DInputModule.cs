using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

public class DInputModule : Device 
{
	private KeyCode[] m_keysToListen = new KeyCode[]
	{
		KeyCode.W, KeyCode.A, KeyCode.D, KeyCode.S
	};

	#region device's interface implementation

	public override void OnDeviceInstalled()
	{
		AddEvent( "OnInputPressed", null );
		AddEvent( "OnInputReleased", null );

	//	AddFunction("SetTarget", SetTarget );
	//	AddFunction("SearchForClosestTarget", SearchForClosestTarget );
	}
	

	public override void Update()
	{
		// Triggering mouse input
		for( int i = 0; i < 3; i++ )
		{
			if( Input.GetMouseButtonDown(i) )
				GetEvent("OnInputPressed").Invoke("mouse" + i);
			if( Input.GetMouseButtonUp(i) )
				GetEvent("OnInputReleased").Invoke("mouse" + i);
		}

		foreach( KeyCode code in m_keysToListen )
		{
			if( Input.GetKeyDown(code) )
				GetEvent("OnInputPressed").Invoke();//code.ToString());
			if( Input.GetKeyUp(code) )
				GetEvent("OnInputReleased").Invoke();//code.ToString());
		}
	}

	#endregion

}
