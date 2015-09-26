using System;
using System.Collections;
using System.Collections.Generic;

namespace BehaviourScheme
{
	public class BSAction : BSNode 
	{
		private SpaceSandbox.DeviceAction m_action = null;
		private SpaceSandbox.DeviceQuery m_query = null;

		public void SetAction( SpaceSandbox.DeviceAction action, SpaceSandbox.DeviceQuery query )	
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
			m_scheme.FireEvent( m_action, m_query );

			foreach( BSNode child in m_children )
				child.Traverse();
		}
	}
}
