
using System.Collections.Generic;

namespace BehaviourScheme
{
	public class BSSelect : BSNode 
	{		
		public delegate bool BSPredecate();
		protected Dictionary<BSNode,BSPredecate> m_transitions = null;


		public void AddChild( BSNode node, BSPredecate predecate )
		{
			m_transitions[node] = predecate;
			node.SetParent( this );
		}

		public void RemoveChild( BSNode node )
		{
			m_transitions.Remove(node);
			node.RemoveParent();
		}

		public override void Activate()
		{
			foreach( KeyValuePair<BSNode,BSPredecate> transition in m_transitions )
			{
				if( transition.Value() )
				{
					transition.Key.Activate();
					break;
				}
			}
		}


		// disabling upcast functionality

		public new BSNode GetConnectedNode() { return null; }
		public new void AddChild( BSNode node ) {}
		public new void RemoveChild() {}
	}
}