
using System.Collections.Generic;

namespace BehaviourScheme
{
	public class BSBranch : BSMultiChildNode 
	{		
		public override void Traverse()
		{
			for( int i = 0; i < m_children.Count; i++ )
			{
				if( m_conditions[i].Invoke() )
				{
					m_children[i].Traverse();
					break;
				}
			}
		}
	}
}