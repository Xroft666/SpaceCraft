
using System.Collections.Generic;

namespace BehaviourScheme
{
	public class BSMultiParentNode : BSMultiConditional 
	{		
		protected List<BSNode> m_parents = new List<BSNode>();

		public override void SetParent( BSNode node )
		{
			if( !m_parents.Contains( node ) )
				m_parents.Add( node );
		}

		public override void RemoveParent( BSNode node )
		{
			m_parents.Remove( node  );
		}
	}
}