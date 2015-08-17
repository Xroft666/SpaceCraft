
using System.Collections.Generic;

namespace BehaviourScheme
{
	public class BSBranch : BSMultiChildNode 
	{
		public override void Traverse()
		{
			for( int i = 0; i < m_children.Count - 1; i ++ )
			{
				// Check condition per connection
				// if no condition true, then execute the last one
				// so the number of conditions is (N of connection - 1)
				if( m_conditions[i].Invoke() )
				{
					m_children[i].Traverse();
					return;
				}
			}

			// Fallback connection
			m_children[m_children.Count - 1].Traverse();
		}
	}
}