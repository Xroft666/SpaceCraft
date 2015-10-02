using System;
using System.Collections;
using System.Collections.Generic;

namespace BehaviourScheme
{
	public class BSAction : BSNode 
	{
		public SpaceSandbox.DeviceAction m_action;
		public BSQuery m_queryNode;

		public void ConnectToQuery( BSQuery query )
		{
			m_queryNode = query;
			query.connectedActionNode = this;
		}

		public void DisconnectFromQuery()
		{
			m_queryNode.connectedActionNode = null;
			m_queryNode = null;
		}

		public override void Traverse()
		{
			SpaceSandbox.DeviceQuery query = null;
			if( m_queryNode != null )
				query = m_queryNode.m_query;

			m_scheme.FireEvent( m_action, query );

			foreach( BSNode child in m_children )
				child.Traverse();
		}
	}
}
