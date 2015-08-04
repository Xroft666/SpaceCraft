namespace BehaviourScheme
{
	public class BSEntry : BSNode 
	{
		public override void Activate(params SpaceSandbox.Entity[] objects)
		{
			m_connectNode.Activate(objects);
		}
	}
}
