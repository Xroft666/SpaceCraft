using System;

namespace BehaviourScheme
{
	public class BSNode 
	{
		public SpaceSandbox.BlueprintScheme m_scheme;

		protected BSNode m_parentNode = null;
		protected BSNode m_connectNode = null;

		public EventArgs m_outputData = null;

		public BSNode GetConnectedNode()
		{
			return m_connectNode;
		}

		public virtual void AddChild( BSNode node )
		{
			m_connectNode = node;
			node.SetParent( this );
		}

		public virtual void RemoveChild( BSNode node )
		{
			m_connectNode.RemoveParent( this );
			m_connectNode = null;
		}

		public virtual void SetParent( BSNode node )
		{
			m_parentNode = node;
		}

		public virtual void RemoveParent( BSNode node )
		{
			m_parentNode = null;
		}

		public virtual void Traverse(){}
	}
}
