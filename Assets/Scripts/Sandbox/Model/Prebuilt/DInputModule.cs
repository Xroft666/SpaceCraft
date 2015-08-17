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
		AddEvent( "OnInputHeld", null );

		AddEvent( "OnMouseScreenPosition", null );
		AddEvent( "OnMouseWorldPosition", null );
	}
	

	public override void Update()
	{
		// Keyboard / mouse keys events
		if( Input.GetKeyDown(m_keyCode) )
		{
			DeviceEvent onPressed = GetEvent("OnInputPressed");
			if( onPressed != null )
				m_containerAttachedTo.IntegratedDevice.ScheduleEvent( onPressed, new System.Object[]{m_keyCode} );
		}
		if( Input.GetKeyUp(m_keyCode) )
		{
			DeviceEvent onReleased = GetEvent("OnInputReleased");
			if( onReleased != null )
				m_containerAttachedTo.IntegratedDevice.ScheduleEvent( onReleased, new System.Object[]{m_keyCode} );
		}
		if( Input.GetKey(m_keyCode) )
		{
			DeviceEvent onHeld = GetEvent("OnInputHeld");
			if( onHeld != null )
				m_containerAttachedTo.IntegratedDevice.ScheduleEvent( onHeld, new System.Object[]{m_keyCode} );
		}

		// Mouse cursor events
		DeviceEvent screenPos = GetEvent("OnMouseScreenPosition");
		if( screenPos != null )
			m_containerAttachedTo.IntegratedDevice.ScheduleEvent( screenPos, new System.Object[]{Input.mousePosition} );

		DeviceEvent worldScreenPos = GetEvent("OnMouseWorldPosition");
		if( worldScreenPos != null )
		{
			Vector3 mousePos = new Vector3( Input.mousePosition.x, 
			                              	Input.mousePosition.y, 
			                               -Camera.main.transform.position.z);
			                                             
			m_containerAttachedTo.IntegratedDevice.ScheduleEvent( worldScreenPos, new System.Object[]{Camera.main.ScreenToWorldPoint(mousePos)} );
		}
	}

	#endregion

}
