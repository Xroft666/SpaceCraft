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

		public BSFunction CreateFunction( string stateName, Device device )
		{
			BSFunction node = new BSFunction();

			device.AddFunction( stateName, node.Activate );
			return node;
		}

		public BSAction CreateAction( string functionName, Device device)
		{
			BSAction node = new BSAction();

			// Search for an integrated inclusive device
			string[] hierarchy = functionName.Split('/');
			for( int i = 0; i < hierarchy.Length - 1; i++ )
			{
				foreach( Device inclusiveDevice in device.GetDevicesList() )
				{
					if( inclusiveDevice.EntityName.Equals( hierarchy[i] ) )
					{
						device = inclusiveDevice;
						break;
					}
				}
			}

			node.SetAction( device.GetFunction( hierarchy[hierarchy.Length-1] ) );

			return node;
		}
		
		public BSEntry CreateEntry( string eventName, Device device)
		{
			BSEntry node = new BSEntry();

			// Search for an integrated inclusive device
			string[] hierarchy = eventName.Split('/');
			for( int i = 0; i < hierarchy.Length - 1; i++ )
			{
				foreach( Device inclusiveDevice in device.GetDevicesList() )
				{
					if( inclusiveDevice.EntityName.Equals( hierarchy[i] ) )
					{
						device = inclusiveDevice;
						break;
					}
				}
			}

			device.m_events[hierarchy[hierarchy.Length-1]] += node.Activate ;
		
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

	}
}