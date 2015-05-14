using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Events;

using BehaviourScheme;

namespace SpaceSandbox
{
	public class BlueprintScheme : Entity 
	{
		private List<BSNode> m_nodes = new List<BSNode>();

		private List<Job> m_plannedActions = new List<Job>();

		public void CreateState( string stateName, Device device )
		{
			BSState node = new BSState();

			device.AddFunction( stateName, Job.make(node.ActivateAndWait()) );

			m_nodes.Add( node );
		}

		public BSAction CreateAction( string functionName, Device device )
		{
			BSAction node = new BSAction();

			node.SetAction( () => device.Activate(functionName) );

			m_nodes.Add( node );

			return node;
		}
		
		public BSEntry CreateEntry( string eventName, Device device )
		{
			BSEntry node = new BSEntry();

			m_nodes.Add( node );

			UnityEvent trigger = device.GetEvent( eventName );
			trigger.AddListener( node.Activate );

			return node;
		}
		
		public BSExit CreateExit( string eventName, Device device)
		{
			BSExit node = new BSExit();

			UnityEvent exitEvent = new UnityEvent();

			node.ExitEvent = exitEvent;
			device.AddEvent(eventName, exitEvent);

			m_nodes.Add( node );

			return node;
		}

		public BSSelect CreateSelect()
		{
			BSSelect node = new BSSelect();
			m_nodes.Add( node );

			return node;
		}

		public BSEvaluate CreateEvaluate()
		{
			BSEvaluate node = new BSEvaluate();
			m_nodes.Add( node );

			return node;
		}


		public void ConnectElements( BSNode left, BSNode right )
		{
			left.AddChild( right );
		}


		public void UpdateScheme()
		{

		}

		public void AddActionToPlanner( Job job )
		{
			m_plannedActions.Add( job );
		}

		public void AddActionToPlanner( Device device, string functionName )
		{
			m_plannedActions.Add( device.GetFunction( functionName ));
		}

		public void ExecutePlanner()
		{
			// a planner can be interrupted if (high priority event kicks in?)
			// a planner can be interrupted if an action (or a check) fails?

			Job.make( ActionsExcecution(), true );
		}

		private IEnumerator ActionsExcecution()
		{
			foreach( Job job in m_plannedActions )
			{
				yield return job.startAsCoroutine();
			}

			OnPlannerCompleted();
		}

		public void OnPlannerCompleted()
		{

		}

		public void OnPlannedFailed()
		{

		}
	}
}