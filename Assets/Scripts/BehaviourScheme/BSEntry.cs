namespace BehaviourScheme
{
	public class BSEntry : BSNode 
	{
		public override void Activate(params SpaceSandbox.Entity[] objects)
		{
//			UnityEngine.Debug.Log("BSEntry invoked");
			m_connectNode.Activate(objects);
		}
	}
}
