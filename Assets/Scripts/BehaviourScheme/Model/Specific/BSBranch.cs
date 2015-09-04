
using System.Collections.Generic;

namespace BehaviourScheme
{
	public class BSBranch : BSMultiChildNode 
	{
		public override void Traverse()
		{
			for( int i = 0; i < m_children.Count - 1; i ++ )
			{
				System.EventArgs args = null;
				if( m_conditionData[i] != null )
					args = m_conditionData[i].Invoke();

				if( m_conditions[i].Invoke( args ) )
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