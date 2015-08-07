namespace BehaviourScheme
{
	public class BSEntry : BSNode 
	{
		public override void Activate(params object[] objects)
		{
			m_connectNode.Activate(objects);
		}
	}
}
