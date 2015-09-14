using System;
using System.Collections;
using System.Collections.Generic;

using SpaceSandbox;
using UnityEngine;

namespace BehaviourScheme
{
	public class BSForeach : BSNode 
	{
		public SpaceSandbox.DeviceQuery m_listQuery;
		private System.Object[] m_objectsList;

		public override void Traverse()
		{
			m_objectsList = ((ArgsList) m_listQuery.Invoke()).objs;

			for( int i = 0; i < m_objectsList.Length; i++ )
				m_connectNode.Traverse();
		}
	}
}
