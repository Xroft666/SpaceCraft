
using System.Collections.Generic;

namespace BehaviourScheme
{
	public class BSEvaluate : BSNode 
	{
		public delegate float BSEvaluator();
		protected Dictionary<BSNode,BSEvaluator> m_transitions = null;
		
		
		public void AddChild( BSNode node, BSEvaluator predecate )
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
			float bestValue = -1f;
			BSNode bestNode = null;

			foreach( KeyValuePair<BSNode,BSEvaluator> transition in m_transitions )
			{
				float transValue = transition.Value();
				if( transValue > bestValue )
				{
					bestValue = transValue;
					bestNode = transition.Key;
				}
			}

			bestNode.Activate();
		}
		
		
		// disabling upcast functionality
		
		public new BSNode GetConnectedNode() { return null; }
		public new void AddChild( BSNode node ) {}
		public new void RemoveChild() {}
	}
}