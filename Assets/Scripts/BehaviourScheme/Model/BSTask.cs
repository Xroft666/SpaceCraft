using System;
using System.Collections;
using System.Collections.Generic;

using BehaviourScheme;
using SpaceSandbox;

using UnityEngine;

public class CommandTask 
{
	public bool IsRunning
	{
		get { return m_runningJobSequence.running; }
	} 

	public delegate void OnStackInitialize( IEnumerable<string> commands );
	public OnStackInitialize onInitialize;
	public Action OnJobComplete;

	private bool m_isRunning = false;

//	public BSEntry m_entry;
	private Stack<string> executingCommandList = new Stack<string>();

	private List<DeviceAction> scheduledEventsList = new List<DeviceAction>();
	private List<DeviceQuery> scheduledEventsDataList = new List<DeviceQuery>();

	private Job m_runningJobSequence;

	public void RegisterSubTask(DeviceAction evt, DeviceQuery qry)
	{
		scheduledEventsList.Add(evt);
		scheduledEventsDataList.Add(qry);
	}

	public void ComposeJob()
	{
		m_runningJobSequence = Job.make( RunExecution() );

		for( int i = 0; i < scheduledEventsList.Count; i++ )
		{
			EventArgs args = null;
			if( scheduledEventsDataList[i] != null )
				args = scheduledEventsDataList[i].Invoke();
			
			Job thisJob = m_runningJobSequence.createAndAddChildJob( scheduledEventsList[i].Invoke( args ) );
			thisJob.jobComplete += JobComplete;

			//executingCommandList.Push(scheduledEventsList[i].Method.Name);
		}

		m_runningJobSequence.start();

		//if( onInitialize != null )
		//	onInitialize( executingCommandList );

	}

	public IEnumerator RunExecution() { yield return null; }

	private void JobComplete( bool killed )
	{
		if( !killed )
		{
			if( OnJobComplete != null )
				OnJobComplete();
		
		//	executingCommandList.Pop();
		}
	}
}

