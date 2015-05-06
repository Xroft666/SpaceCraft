
using System.Collections.Generic;

namespace SpaceSandbox
{
	public class Container : Entity, IDamagable 
	{
		public BlueprintScheme blueprint = null;

		public List<Device> equipment = new List<Device>();
		public List<Entity> cargo = new List<Entity>();
		public float volume;

		void IDamagable.TakeDamage()
		{
			// open the containment
			// broke some of it
			// explode the explosive
			// or get split into smaller containers
		}
	}
}