using System;
using System.Collections.Generic;

namespace BehaviourScheme
{
	public class BSPriority : BSMultiParentNode 
	{		
		public override void Traverse()
		{
			m_scheme.AddScheduledEvent( RescheduledEvent, null );
		}

		public void RescheduledEvent( EventArgs data )
		{
			// Traverse the first valid parent data,
			// considering that the list is prioritized

			foreach( BSNode node in m_parents )
			{
				if( node.m_outputData != null )
				{
					m_outputData = node.m_outputData;
					m_connectNode.Traverse();
					break;
				}
			}
		}
	}
}