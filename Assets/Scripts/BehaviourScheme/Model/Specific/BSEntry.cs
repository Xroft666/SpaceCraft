using System;
using System.Collections;

namespace BehaviourScheme
{
	public class BSEntry : BSMultiChildNode
	{

		public override void Traverse()
		{
			foreach( BSNode node in m_children )
				node.Traverse();

//			if( m_connectNode != null )
//				m_connectNode.Traverse();
		}
	}
}
