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

		Name = evt == null ? "missing" : evt.Method.Name;
	}

	public void ExecuteImmediately()
	{
		Job.make( UnpackExecution(), true );
	}

	public IEnumerator UnpackExecution()
	{
		if( taskAction == null )
			return DummyExecution();

		return taskAction.Invoke( taskData);
	}

	public IEnumerator DummyExecution()
	{
		//Debug.Log("Device function is missing!");
		throw new Exception("Device function is missing!");
		yield return null;
	}
}

