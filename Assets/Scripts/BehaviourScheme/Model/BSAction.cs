using System;
using System.Collections;

namespace BehaviourScheme
{
	public class BSAction : BSNode 
	{
		private SpaceSandbox.DeviceEvent job = null;

		public void SetAction( SpaceSandbox.DeviceEvent action )	{ job += action; }
		public void RemoveAction() { job = null; }

		// executes a single action and continues to the next node
		public override void Activate(params object[] objects)
		{
			if( job != null )
				job.Invoke( objects );

			if( m_connectNode != null )
				m_connectNode.Activate(objects);
		}
	}
}
