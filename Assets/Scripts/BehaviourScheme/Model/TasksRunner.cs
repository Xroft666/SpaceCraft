using System;
using System.Collections;
using System.Collections.Generic;

using BehaviourScheme;
using SpaceSandbox;

using UnityEngine;

public class TasksRunner
{
	public Queue<string> executingCommandList = new Queue<string>();
	public delegate void OnStackInitialize( IEnumerable<string> commands );
	public OnStackInitialize onInitialize;
	public Action OnJobComplete;

	private Queue<Task> scheduledTaskList = new Queue<Task>();
	private Job m_execution;

	public bool IsRunning
	{
		get { return m_execution != null && m_execution.running; }
	} 
	

	public void ScheduleEvent(DeviceAction evt, DeviceQuery data)
	{
		Task task = new Task(evt, data);
		scheduledTaskList.Enqueue( task );
	}
	
	public void ExecuteTasksQeue()
	{	
		if( scheduledTaskList.Count == 0 )
			return;
	
		m_execution = Job.make( Execution() );

		while( scheduledTaskList.Count != 0 )
		{
			Task task = scheduledTaskList.Dequeue();		

			Job job = m_execution.createAndAddChildJob( task.UnpackExecution() );
			job.jobComplete += JobComplete;

			executingCommandList.Enqueue(task.Name);
		}

		m_execution.start();

		if( onInitialize != null )
			onInitialize( executingCommandList );
	}

	public IEnumerator Execution()
	{
		yield return null;	
	}

	private void JobComplete( bool killed )
	{
		if( !killed )
		{
			if( OnJobComplete != null )
				OnJobComplete();
		
			executingCommandList.Dequeue();
		}
	}

	public void KillTasks()
	{
		if( m_execution != null )
			m_execution.kill();

		if( onInitialize != null )
			onInitialize.Invoke(new List<string>());

		executingCommandList.Clear();
		scheduledTaskList.Clear();
	}
}

