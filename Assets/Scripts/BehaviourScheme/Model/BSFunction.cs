using System.Collections;
using System.Collections.Generic;

namespace BehaviourScheme
{
	// Properties I would like to have for states
	// - States represent high level device functions, which can be inclusive state machines
	// - States can be time - yelded, which means they can take just one frame fore execution
	// or it is being activated until succeeded
	// - Exit nodes represent event higher level device triggers

	public class BSFunction : BSNode 
	{
		private List<BSNode> m_includedEntries = new List<BSNode>();
		private List<BSNode> m_includedExits = new List<BSNode>();
		private List<BSNode> m_includedNodes = new List<BSNode>();

		public new void IncludeNode( BSNode node )
		{
			if( (node as BSExit) != null )
				m_includedExits.Add( node );
			else if( (node as BSEntry) != null )
				m_includedEntries.Add( node );
			else
				m_includedNodes.Add( node );
		}

		// traverse to this node (on entry)
		public override void Activate(params object[] objects)
		{

		}


		public void Evaluate()
		{

		}

		private void OnExitNode(params object[] objects)
		{

		}
	}
}