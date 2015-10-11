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
		AddTrigger( "OnInputPressed", null );
		AddTrigger( "OnInputReleased", null );
		AddTrigger( "OnInputHeld", null );

		AddTrigger( "OnMouseScreenPosition", null );
		AddTrigger( "OnMouseWorldPosition", null );

		AddQuery( "MouseWorldPosition", MouseWorldPosition );
	}

	public override void OnDeviceUninstalled()
	{
		RemoveTrigger( "OnInputPressed" );
		RemoveTrigger( "OnInputReleased" );
		RemoveTrigger( "OnInputHeld" );
		
		RemoveTrigger( "OnMouseScreenPosition");
		RemoveTrigger( "OnMouseWorldPosition" );
		
		RemoveQuery( "MouseWorldPosition" );
	}

	public override void Update()
	{
		// Keyboard / mouse keys events
		if( Input.GetKeyDown(m_keyCode) )
		{
			DeviceTrigger onPressed = GetTrigger("OnInputPressed");
			if( onPressed != null )
				onPressed();
		}
		if( Input.GetKeyUp(m_keyCode) )
		{
			DeviceTrigger onReleased = GetTrigger("OnInputReleased");
			if( onReleased != null )
				onReleased();
		}
		if( Input.GetKey(m_keyCode) )
		{
			DeviceTrigger onHeld = GetTrigger("OnInputHeld");
			if( onHeld != null )
				onHeld();
		}
	}

	#endregion

	#region Queries

	private ArgsObject MouseWorldPosition()
	{
		Vector3 mousePos = new Vector3( Input.mousePosition.x, 
		                               Input.mousePosition.y, 
		                               -Camera.main.transform.position.z);

		return new ArgsObject() { obj = Camera.main.ScreenToWorldPoint(mousePos)};
	}

	#endregion

}
