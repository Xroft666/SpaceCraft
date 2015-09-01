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
		public List<DeviceQuery> scheduledEventsDataList = new List<DeviceQuery>();

		public List<BSNode> m_nodes = new List<BSNode>();

		public BSEntry m_entryPoint;
		private Job m_runningJobSequence;

		public void AddScheduledEvent(DeviceEvent evt, DeviceQuery data)
		{
			scheduledEventsList.Add( evt );
			scheduledEventsDataList.Add( data );
		}

		public void ExecuteSceduledEvents()
		{
			if( m_runningJobSequence != null && m_runningJobSequence.running )
				return;

			m_runningJobSequence = Job.make( JobsContainer() );

			m_entryPoint.Traverse();

			for( int i = 0; i < scheduledEventsList.Count; i++ )
			{
				EventArgs args = null;
				if( scheduledEventsDataList[i] != null )
					args = scheduledEventsDataList[i].Invoke();

				m_runningJobSequence.createAndAddChildJob( scheduledEventsList[i].Invoke( args ));
			}

			m_runningJobSequence.start();

			ClearEventsAndData();
		}

		private IEnumerator JobsContainer()
		{
			yield break;
		}

		public void ClearEventsAndData()
		{
			scheduledEventsList.Clear(); 
			scheduledEventsDataList.Clear();

//			foreach( BSNode node in m_nodes )
//				node.m_outputData = null;
		}

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
			device.m_events[eventName] += node.Initialize ;
		
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

	}
}