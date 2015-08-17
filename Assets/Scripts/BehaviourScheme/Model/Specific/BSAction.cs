using System;
using System.Collections;

namespace BehaviourScheme
{
	public class BSAction : BSNode 
	{
		private SpaceSandbox.DeviceEvent job = null;

		public void SetAction( SpaceSandbox.DeviceEvent action )	{ job += action; }
		public void RemoveAction() { job = null; }

		public override void Traverse()
		{
			if( job != null )
				job.Invoke( m_parentNode.m_outputData );

			if( m_connectNode != null )
				m_connectNode.Traverse();
		}
	}
}
