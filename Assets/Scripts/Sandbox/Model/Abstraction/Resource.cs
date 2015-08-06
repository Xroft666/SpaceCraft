
namespace SpaceSandbox
{
    public class Resource : Entity
	{
		public int Amount { get; private set; }
		public Material Material { get; private set; }

		public override void Destroy ()
		{
			// Call physics here for explosions
		}
	}
}
