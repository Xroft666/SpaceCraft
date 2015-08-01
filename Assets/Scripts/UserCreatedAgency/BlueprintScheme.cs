using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Events;

using BehaviourScheme;

namespace SpaceSandbox
{
	public class BlueprintScheme : Entity 
	{
		private MemoryStack m_memory = new MemoryStack();

		public MemoryStack Memory
		{
			get { return m_memory; }
			set { m_memory = value; }
		}

		private List<BSNode> m_nodes = new List<BSNode>();

		private List<DeviceEvent> m_plannedActions = new List<DeviceEvent>();

		public void CreateState( string stateName, Device device )
		{
			BSState node = new BSState();

			device.AddFunction( stateName, node.Activate );

			m_nodes.Add( node );
		}

		public BSAction CreateAction( string functionName, Device device )
		{
			BSAction node = new BSAction();

			node.SetAction( device.GetFunction( functionName ) );

			m_nodes.Add( node );

			return node;
		}
		
		public BSEntry CreateEntry( string eventName, Device device )
		{
			BSEntry node = new BSEntry();

			m_nodes.Add( node );

			DeviceEvent trigger = device.GetEvent( eventName );
			trigger += node.Activate ;

			return node;
		}
		
		public BSExit CreateExit( string eventName, Device device)
		{
			BSExit node = new BSExit();

			device.AddEvent(eventName, null);

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

		public void AddActionToPlanner( DeviceEvent job )
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

		}



		public void OnPlannerCompleted()
		{

		}

		public void OnPlannedFailed()
		{

		}
	}
}