
using System;

namespace BehaviourScheme
{
	public class BSAction : BSNode 
	{
		private Action m_Action = null;

		public void SetAction( Action action )
		{
			m_Action = action;
		}

		public void RemoveAction()
		{
			m_Action = null;
		}

		// executes a single action and continues to the next node
		public override void Activate()
		{
			if( m_Action != null )
				m_Action();

			GetConnectedNode().Activate();
		}
	}
}
