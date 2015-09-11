using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using BehaviourScheme;

namespace SpaceSandbox
{
	public class BlueprintScheme : Entity 
	{
		public Queue<CommandTask> scheduledTaskList = new Queue<CommandTask>();
		public CommandTask currentBuiltUpTask = null;

		public List<BSNode> m_nodes = new List<BSNode>();

		public BSEntry m_entryPoint;

		public bool IsRunning
		{
			get { return currentBuiltUpTask.IsRunning; }
		}

		#region Planner 

		public void ScheduleTask( DeviceEvent entry )
		{
			currentBuiltUpTask = new CommandTask();

			entry.Invoke();
			scheduledTaskList.Enqueue( currentBuiltUpTask );
		}

		public void ScheduleEvent(DeviceAction evt, DeviceQuery data)
		{
			if( currentBuiltUpTask != null )
			{
				currentBuiltUpTask.RegisterSubTask(evt, data);
			}
			else
			{
				EventArgs args = null;
				if( data != null )
					data.Invoke();

				Job.make( evt.Invoke( args ), true );
			}
		}

		public void ExecuteCommandsList()
		{

				currentBuiltUpTask = scheduledTaskList.Dequeue();
				currentBuiltUpTask.ComposeJob();

			currentBuiltUpTask = null;
		}

		#endregion

		#region Nodes Creation 


		public BSAction CreateAction( string functionName, Device device, DeviceQuery query = null)
		{

			BSAction node = new BSAction() { m_scheme = this };
			node.SetAction( device.GetFunction(functionName), query );

			m_nodes.Add(node);
			return node;
		}
		
		public BSEntry CreateEntry( string eventName, Device device)
		{
			BSEntry node = new BSEntry() { m_scheme = this };
			device.m_events[eventName] += node.Traverse;
		
			m_nodes.Add(node);
			return node;
		}
		
		public BSExit CreateExit( string eventName, Device device)
		{
			BSExit node = new BSExit() { m_scheme = this, m_entry = device.GetEvent(eventName) };

			m_nodes.Add(node);
			return node;
		}

		public BSBranch CreateBranch()
		{
			BSBranch node = new BSBranch() { m_scheme = this };

			m_nodes.Add(node);
			return node;
		}

		public BSSequence CreateSequence()
		{
			BSSequence node = new BSSequence() { m_scheme = this };
			
			m_nodes.Add(node);
			return node;
		}

		public BSPriority CreatePriority()
		{
			BSPriority node = new BSPriority() { m_scheme = this };

			m_nodes.Add(node);
			return node;
		}

		public BSEvaluate CreateEvaluate()
		{
			BSEvaluate node = new BSEvaluate() { m_scheme = this };

			m_nodes.Add(node);
			return node;
		}

		public void ConnectElements( BSNode left, BSNode right )
		{
			left.AddChild( right );
		}

		#endregion
	}
}