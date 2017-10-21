using System;
using SpaceSandbox;

using System.Collections.Generic;

namespace BehaviourScheme
{
	public class BSNode 
	{
		public NodeView m_view;

		public string m_type;
		public string m_name;

		public List<BSNode> m_parents;
		public List<BSNode> m_children;

		public BSNode()
		{
			m_parents = new List<BSNode>();
        	m_children = new List<BSNode>();
		}
		
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

		public void SetParent( BSNode node )
		{
			if( !m_parents.Contains( node ) )
				m_parents.Add( node );
		}
		
		public void RemoveParent( BSNode node )
		{
			m_parents.Remove( node  );
		}

		public virtual void Traverse(){}
	}
}
