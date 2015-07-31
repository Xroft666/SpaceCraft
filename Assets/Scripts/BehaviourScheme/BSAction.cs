using System;
using System.Collections;

namespace BehaviourScheme
{
	public class BSAction : BSNode 
	{
		private Job m_yieldAction = null;

		public void SetYieldAction( Job yieldAction )	{ m_yieldAction = yieldAction; }
		public void RemoveYieldAction() { m_yieldAction = null; }

		// executes a single action and continues to the next node
		public override void Activate(params SpaceSandbox.Entity[] objects)
		{
			if( m_yieldAction != null )
			{
				UnityEngine.Debug.LogWarning("Empty Action");
				m_yieldAction.start();
			}

			GetConnectedNode().Activate(objects);
		}

		public override IEnumerator ActivateAndWait()
		{
			if( m_yieldAction != null )
			{
				UnityEngine.Debug.LogWarning("Empty Yield Action");
				yield return m_yieldAction.startAsCoroutine();
			}

			yield return this.m_connectNode.ActivateAndWait();
		}
	}
}
