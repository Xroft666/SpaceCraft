using System;
using System.Collections;

namespace BehaviourScheme
{
	public class BSAction : BSNode 
	{
		private SpaceSandbox.DeviceEvent m_action = null;
		private SpaceSandbox.DeviceQuery m_query = null;

		public void SetAction( SpaceSandbox.DeviceEvent action, SpaceSandbox.DeviceQuery query )	
		{ 
			m_action = action; 
			m_query = query;
		}

		public void RemoveAction() 
		{ 
			m_action = null; 
			m_query = null;
		}

		public override void Traverse()
		{
			if( m_action != null )
				m_scheme.AddScheduledEvent( m_action, m_query );
			//	Job.make( job( m_parentNode.m_outputData ) );

			if( m_connectNode != null )
				m_connectNode.Traverse();
		}
	}
}
