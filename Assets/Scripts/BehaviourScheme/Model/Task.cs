using System;
using System.Collections;
using System.Collections.Generic;

using BehaviourScheme;
using SpaceSandbox;

using UnityEngine;

public class Task 
{
	private DeviceAction taskAction;
	private DeviceQuery taskData;

	public string Name
	{
		get;
		private set;
	}

	public bool IsRunning
	{
		get { return true; }
	}
	
	public Task (DeviceAction evt, DeviceQuery qry)
	{
		taskAction = evt;
		taskData = qry;

		Name = evt.Method.Name;
	}

	public void ExecuteImmediately()
	{
		Job.make( UnpackExecution(), true );
	}

	public IEnumerator UnpackExecution()
	{
	//	EventArgs args = null;
	//	if( taskData != null )
	//		args = taskData.Invoke();
		
		return taskAction.Invoke( taskData);//args );
	}
}

