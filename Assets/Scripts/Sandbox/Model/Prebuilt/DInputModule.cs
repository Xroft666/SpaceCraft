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
		m_blueprint.AddTrigger( "OnInputPressed", null );
		m_blueprint.AddTrigger( "OnInputReleased", null );
		m_blueprint.AddTrigger( "OnInputHeld", null );

		m_blueprint.AddTrigger( "OnMouseScreenPosition", null );
		m_blueprint.AddTrigger( "OnMouseWorldPosition", null );

		m_blueprint.AddQuery( "MouseWorldPosition", MouseWorldPosition );
	}

	public override void OnDeviceUninstalled()
	{
		m_blueprint.RemoveTrigger( "OnInputPressed" );
		m_blueprint.RemoveTrigger( "OnInputReleased" );
		m_blueprint.RemoveTrigger( "OnInputHeld" );
		
		m_blueprint.RemoveTrigger( "OnMouseScreenPosition");
		m_blueprint.RemoveTrigger( "OnMouseWorldPosition" );
		
		m_blueprint.RemoveQuery( "MouseWorldPosition" );
	}

	public override void Update()
	{
		// Keyboard / mouse keys events
		if( Input.GetKeyDown(m_keyCode) )
		{
			DeviceTrigger onPressed = m_blueprint.GetTrigger("OnInputPressed");
			if( onPressed != null )
				onPressed();
		}
		if( Input.GetKeyUp(m_keyCode) )
		{
			DeviceTrigger onReleased = m_blueprint.GetTrigger("OnInputReleased");
			if( onReleased != null )
				onReleased();
		}
		if( Input.GetKey(m_keyCode) )
		{
			DeviceTrigger onHeld = m_blueprint.GetTrigger("OnInputHeld");
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

    public override string ToString()
    {
        return "Input";
    }
}
