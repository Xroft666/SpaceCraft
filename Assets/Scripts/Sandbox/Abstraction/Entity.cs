
namespace SpaceSandbox
{
	public class Entity  
	{
		private string m_entityName;
		public string EntityName
		{
			get{ return m_entityName; }
		}

		public virtual void TakeDamage() {}
		public virtual void Destroy() {}
	}
}
