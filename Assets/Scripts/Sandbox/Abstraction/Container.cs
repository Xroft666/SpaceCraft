
using System.Collections.Generic;

namespace SpaceSandbox
{
	public class Container : Entity, IDamagable 
	{
		/// <summary>
		/// The m_generated device. Each container represents a compund device,
		/// with blueprint logic attached.
		/// </summary>
		private Device m_integratedDevice = null;
		/// <summary>
		/// The m_blueprint. The blueprint logic scheme storage.
		/// </summary>
		private BlueprintScheme m_blueprint = null;

		/// <summary>
		/// The m_cargo. The inventory of this specific container
		/// </summary>
		private List<Entity> m_cargo = null;

		/// <summary>
		/// Gets the integrated device. If never asked, will generate device
		/// </summary>
		/// <value>The integrated device.</value>
		public Device IntegratedDevice
		{
			get
			{
				if( m_integratedDevice == null )
					m_integratedDevice = new Device();
				
				return m_integratedDevice;
			}
		}

		public void LoadSoftware(BlueprintScheme blueptint)
		{
			m_blueprint = blueptint;
		}

		public void AddToCargo( Entity entity )
		{
			if( m_cargo == null )
				m_cargo = new List<Entity>();

			m_cargo.Add( entity );
		}

		public List<Entity> GetCargoList()
		{
			return m_cargo;
		}

		/// <summary>
		/// Takes the damage. Just an example of the interface usage.
		/// </summary>
		void IDamagable.TakeDamage()
		{

		}

		/// <summary>
		/// ContainerRepresentation -> Container -> Device calls execution flow
		/// </summary>

		public void Initialize() 
		{
			IntegratedDevice.Initialize();
		}

		public void Update() 
		{
			IntegratedDevice.Update();
		}

		public void Delete() 
		{
			IntegratedDevice.Delete();
		}
	}
}