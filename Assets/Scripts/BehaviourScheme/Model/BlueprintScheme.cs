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
		public BlueprintScheme ()
		{
			Memory = new MemoryStack();
		}

		public MemoryStack Memory { get; private set; }

		public List<DeviceEvent> scheduledEventsList = new List<DeviceEvent>();
		public List<EventArgs> scheduledEventsDataList = new List<EventArgs>();

		public List<BSNode> m_nodes = new List<BSNode>();

		public void AddScheduledEvent(DeviceEvent evt, EventArgs data)
		{
			scheduledEventsList.Add( evt );
			scheduledEventsDataList.Add( data );
		}

		public IEnumerator ExecuteSceduledEvents()
		{
			for( int i = 0; i < scheduledEventsList.Count; i++ )
			{
				yield return Job.make( scheduledEventsList[i].Invoke( scheduledEventsDataList[i]) ).startAsCoroutine();
			}

			yield break;
		}

		public void ClearEventsAndData()
		{
			scheduledEventsList.Clear(); 
			scheduledEventsDataList.Clear();

			foreach( BSNode node in m_nodes )
				node.m_outputData = null;
		}

		public BSAction CreateAction( string functionName, Device device)
		{
			bool existenceFlag = false;

			// Search for an integrated inclusive device
			string[] hierarchy = functionName.Split('/');
			for( int i = 0; i < hierarchy.Length - 1; i++ )
			{
				existenceFlag = false;
				foreach( Device inclusiveDevice in device.GetDevicesList() )
				{
					if( inclusiveDevice.EntityName.Equals( hierarchy[i] ) )
					{
						existenceFlag = true;
						device = inclusiveDevice;
						break;
					}
				}
			}

			if( !existenceFlag && hierarchy.Length > 1 )
			{
				Debug.LogError(hierarchy[hierarchy.Length-2] + " device wasn't installed");
				return null;
			}

			BSAction node = new BSAction() { m_scheme = this };
			node.SetAction( device.GetFunction( hierarchy[hierarchy.Length-1] ) );

			m_nodes.Add(node);
			return node;
		}
		
		public BSEntry CreateEntry( string eventName, Device device)
		{
			bool existenceFlag = false;

			// Search for an integrated inclusive device
			string[] hierarchy = eventName.Split('/');
			for( int i = 0; i < hierarchy.Length - 1; i++ )
			{
				existenceFlag = false;
				foreach( Device inclusiveDevice in device.GetDevicesList() )
				{
					if( inclusiveDevice.EntityName.Equals( hierarchy[i] ) )
					{
						existenceFlag = true;
						device = inclusiveDevice;
						break;
					}
				}
			}

			if( !existenceFlag && hierarchy.Length > 1 )
			{
				Debug.LogError(hierarchy[hierarchy.Length-2] + " device wasn't installed");
				return null;
			}

			BSEntry node = new BSEntry() { m_scheme = this };
			device.m_events[hierarchy[hierarchy.Length-1]] += node.Initialize ;
		
			m_nodes.Add(node);
			return node;
		}
		
		public BSExit CreateExit( string eventName, Device device)
		{
			BSExit node = new BSExit() { m_scheme = this };

			device.AddEvent(eventName, null);

			m_nodes.Add(node);
			return node;
		}

		public BSBranch CreateBranch()
		{
			BSBranch node = new BSBranch() { m_scheme = this };

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

	}
}