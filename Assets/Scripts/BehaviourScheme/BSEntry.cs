namespace BehaviourScheme
{
	public class BSEntry : BSNode 
	{
		public override void Activate()
		{
			m_connectNode.Activate();
		}
	}
}
