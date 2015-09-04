
using System.Collections.Generic;

namespace BehaviourScheme
{
	public class BSSelect : BSMultiParentNode 
	{		
		public override void Traverse()
		{
			for( int i = 0; i < m_parents.Count; i ++ )
			{
				if( m_conditions[i].Invoke(null) )
				{
					m_connectNode.Traverse( );
					break;
				}
			}
		}
	}
}