using System;
using System.Collections;
using System.Collections.Generic;

using SpaceSandbox;

namespace BehaviourScheme
{
	public class BSForeach : BSNode 
	{
		private SpaceSandbox.DeviceQuery m_listQuery;

		private List<System.Object> m_objectsList;
		private System.Object m_CurrentObject;

		public override void Traverse()
		{
			m_objectsList = ((ArgsList) m_listQuery.Invoke()).objs;

			for( int i = 0; i < m_objectsList.Count; i++ )
			{
				m_CurrentObject = m_objectsList[i];
				m_connectNode.Traverse();
			}
		}

		public ArgsObject CurrentTargetPosition()
		{	
			return new ArgsObject(){ obj = m_CurrentObject };
		}
	}
}
