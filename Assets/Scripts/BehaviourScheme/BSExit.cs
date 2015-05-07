
using SpaceSandbox;

namespace BehaviourScheme
{
	public class BSExit : BSNode 
	{
		private BlueprintScheme m_scheme = null;

		public void SetScheme( BlueprintScheme scheme )
		{
			m_scheme = scheme;
		}

		public override void Activate()
		{
			m_scheme.OnExitNode( this );
		}
	}
}
