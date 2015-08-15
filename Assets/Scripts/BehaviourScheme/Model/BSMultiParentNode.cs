
using System.Collections.Generic;

namespace BehaviourScheme
{
	public class BSMultuParentNode : BSMultiConditional 
	{		
		protected List<BSNode> m_parents = null;

		public void SetParent( BSNode node )
		{
			if( !m_parents.Contains( node ) )
				m_parents.Add( node );
		}

		public void RemoveParent( BSNode node )
		{
			m_parents.Remove( node  );
		}
	}
}