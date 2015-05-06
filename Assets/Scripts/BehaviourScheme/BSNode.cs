
using System.Collections.Generic;

namespace BehaviourScheme
{
	public class BSNode 
	{
		protected BSNode m_parentNode = null;
		protected BSNode m_connectNode = null;

		public BSNode GetConnectedNode()
		{
			return m_connectNode;
		}

		public void AddChild( BSNode node )
		{
			m_connectNode = node;
			node.SetParent( this );
		}

		public void RemoveChild()
		{
			m_connectNode.RemoveParent();
			m_connectNode = null;
		}

		public void SetParent( BSNode node )
		{
			m_parentNode = node;
		}

		public void RemoveParent()
		{
			m_parentNode = null;
		}


		public virtual void Activate(){}
	}
}
