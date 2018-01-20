using System;
using System.Collections;
using System.Collections.Generic;

using SpaceSandbox;

namespace BehaviourScheme
{
	public class BSAction : BSNode 
	{
		public event Action<string, DeviceQuery> onEventFired;

		public string m_actionName;
		public BSQuery m_queryNode;

		public override BSNode GetCopy ()
		{
			return new BSAction ();
		}

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
			Device queryDevice = null;
			string queryName = null;
			DeviceQuery queryData = null;

			if( m_queryNode != null )
			{
				queryDevice = m_queryNode.m_device;
				queryName = m_queryNode.m_queryName;
				queryData = queryDevice.m_blueprint.GetQuery (queryName);
			}

			if (onEventFired != null)
				onEventFired (m_actionName, queryData);

			foreach( BSNode child in m_children )
				child.Traverse();
		}
	}
}
