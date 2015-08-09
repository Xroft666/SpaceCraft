using System;

namespace SpaceSandbox
{
	public class Entity : Object
	{
		public Entity()
		{

		}

		public Entity( string name )
		{
			EntityName = name;
		}

		public string EntityName
		{
			get; set;
		}

		public virtual void TakeDamage() {}
		public virtual void Destroy() {}
	}
}
