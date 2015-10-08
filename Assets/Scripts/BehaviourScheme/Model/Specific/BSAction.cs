using System;
using System.Collections;
using System.Collections.Generic;

using SpaceSandbox;

namespace BehaviourScheme
{
	public class BSAction : BSNode 
	{
		//public SpaceSandbox.DeviceAction m_action;
		public Device m_device;
		public string m_actionName;

		public BSQuery m_queryNode;

		public void ConnectToQuery( BSQuery query )
		{
			m_queryNode = query;
			query.connectedNode = this;
		}

		public void DisconnectFromQuery()
		{
			m_queryNode.connectedNode = null;
			m_queryNode = null;
		}

		public override void Traverse()
		{
			//DeviceQuery query = null;
			Device queryDevice = null;
			string queryName = null;

			if( m_queryNode != null )
			{
			//	query = m_queryNode.m_query;
				queryDevice = m_queryNode.m_device;
				queryName = m_queryNode.m_queryName;
			}

		//	m_scheme.FireEvent( m_action, query );

			

			m_scheme.FireEvent( m_device, m_actionName, queryDevice, queryName );

			foreach( BSNode child in m_children )
				child.Traverse();
		}
	}
}
