using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Events;

using BehaviourScheme;

namespace SpaceSandbox
{
	public class BlueprintScheme : Entity 
	{
		private List<BSEntry> m_entries = new List<BSEntry>();
		private List<BSState> m_states = new List<BSState>();
		private List<BSExit> m_exits = new List<BSExit>();


		private List<BSNode> m_nodes = new List<BSNode>();

		private List<Job> m_plannedActions = new List<Job>();

		public void CreateState()
		{
			BSState node = new BSState();

			m_states.Add( node );
			m_nodes.Add( node );
		}

		public void CreateAction( Device device, string functionName )
		{
			BSAction node = new BSAction();

			node.SetAction( () => device.Activate(functionName) );

			m_nodes.Add( node );
		}
		
		public void CreateEntry( Device device, string eventName )
		{
			BSEntry node = new BSEntry();

			m_entries.Add( node );
			m_nodes.Add( node );

			UnityEvent trigger = device.GetEvent( eventName );
			trigger.AddListener( node.Activate );
		}
		
		public void CreateExit()
		{
			BSExit node = new BSExit();

			node.ExitEvent.AddListener( 
			delegate
			{ 
				OnExitNode(node);
			});

			m_exits.Add( node );
			m_nodes.Add( node );
		}

		public void CreateSelect()
		{
			BSSelect node = new BSSelect();
			m_nodes.Add( node );
		}

		public void CreateEvaluate()
		{
			BSEvaluate node = new BSEvaluate();
			m_nodes.Add( node );
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

		public void OnExitNode( BSNode exitNode )
		{

		}
	}
}