
using System.Collections.Generic;

namespace BehaviourScheme
{
	public class BSMultiChildNode : BSMultiConditional 
	{		
		protected List<BSNode> m_children = null;

		public void AddChild( BSNode node )
		{
			m_children.Add( node );
			node.SetParent( this );
		}

		public void RemoveChild( BSNode node )
		{
			m_children.Remove(node);
			node.RemoveParent( this );
		}
	}
}