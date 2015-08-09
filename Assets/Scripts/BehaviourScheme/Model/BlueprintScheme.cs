using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Events;

using BehaviourScheme;

namespace SpaceSandbox
{
	public class BlueprintScheme : Entity 
	{
		public BSFunction RootFuntion { get; private set; }
		public MemoryStack Memory { get; private set; }

		private List<DeviceEvent> m_plannedActions = new List<DeviceEvent>();

		public BSFunction CreateFunction( string stateName, Device device )
		{
			BSFunction node = new BSFunction();

			device.AddFunction( stateName, node.Activate );
			return node;
		}

		public BSAction CreateAction( string functionName, Device device )
		{
			BSAction node = new BSAction();

			node.SetAction( device.GetFunction( functionName ) );

			return node;
		}
		
		public BSEntry CreateEntry( string eventName, Device device )
		{
			BSEntry node = new BSEntry();

			device.m_events[eventName] += node.Activate ;
		
			return node;
		}
		
		public BSExit CreateExit( string eventName, Device device)
		{
			BSExit node = new BSExit();

			device.AddEvent(eventName, null);

			return node;
		}

		public BSSelect CreateSelect()
		{
			BSSelect node = new BSSelect();

			return node;
		}

		public BSEvaluate CreateEvaluate()
		{
			BSEvaluate node = new BSEvaluate();

			return node;
		}


		public void ConnectElements( BSNode left, BSNode right )
		{
			left.AddChild( right );
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

			// should be using STATES as yelded actions instead of enums 
		}



		public void OnPlannerCompleted()
		{

		}

		public void OnPlannedFailed()
		{

		}
	}
}