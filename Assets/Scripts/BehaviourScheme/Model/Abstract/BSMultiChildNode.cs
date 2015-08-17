
using System.Collections.Generic;

namespace BehaviourScheme
{
	public class BSMultiChildNode : BSMultiConditional 
	{		
		protected List<BSNode> m_children = new List<BSNode>();

		public override void AddChild( BSNode node )
		{
			m_children.Add( node );
			node.SetParent( this );
		}

		public override void RemoveChild( BSNode node )
		{
			m_children.Remove(node);
			node.RemoveParent( this );
		}
	}
}