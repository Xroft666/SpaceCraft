using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

public class DFriendOrFoeUnit : Device 
{



	#region device's functions

	private IEnumerator AddTarget( EventArgs args )
	{
		ContainerArgs cArgs = args as ContainerArgs;

		ContainerView view = cArgs.container.View;



		yield break;
	}

	private IEnumerator RemoveTarget( EventArgs args )
	{
		ContainerArgs cArgs = args as ContainerArgs;

		ContainerView view = cArgs.container.View;



		yield break;
	}

	private IEnumerator ResetTarget( EventArgs args )
	{
//		m_targets.Clear();

		yield break;
	}



	#endregion



	#region device's interface implementation

	public override void OnDeviceInstalled()
	{
		AddEvent( "TargetPosition", null );

		AddAction("AddTarget", AddTarget );
		AddAction("RemoveTarget", RemoveTarget);
		AddAction("ResetTarget", ResetTarget);


	}

	public override void Initialize()
	{

	}

	public override void Update()
	{
//		DeviceEvent targetPos = GetEvent("TargetPosition");
//		if( targetPos != null && m_targets.Count > 0 )
//		{
//			m_containerAttachedTo.IntegratedDevice.ScheduleEvent( targetPos, CurrentTarget );
//		}
	}

	#endregion


}
