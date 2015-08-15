using System.Collections;
using System.Collections.Generic;

namespace BehaviourScheme
{
	public class BSNode 
	{
		public delegate bool BSPredecate();
		protected BSPredecate m_condition = null;

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

		public void RemoveChild( BSNode node )
		{
			m_connectNode.RemoveParent( this );
			m_connectNode = null;
		}

		public void SetParent( BSNode node )
		{
			m_parentNode = node;
		}

		public void RemoveParent( BSNode node )
		{
			m_parentNode = null;
		}

		public virtual void Activate(params object[] objects){}
	}
}
