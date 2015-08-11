
using System.Collections.Generic;

namespace SpaceSandbox
{
	public class Container : Entity 
	{
		public ContainerView View { get; set; }


		/// <summary>
		/// The m_cargo. The inventory of this specific container
		/// </summary>
		private List<Entity> m_cargo = new List<Entity>();

		/// <summary>
		/// The m_generated device. Each container represents a compund device,
		/// with blueprint logic attached.
		/// </summary>
		private Device m_integratedDevice = null;

		/// <summary>
		/// Gets the integrated device. If never asked, will generate device
		/// </summary>
		/// <value>The integrated device.</value>
		public Device IntegratedDevice
		{
			get
			{
				if( m_integratedDevice == null )
				{
					m_integratedDevice = new Device();
					m_integratedDevice.AssignContainer( this );
				}
				
				return m_integratedDevice;
			}
		}

		public void AddToCargo( Entity entity )
		{
			m_cargo.Add( entity );
		}

		public List<Entity> GetCargoList()
		{
			return m_cargo;
		}

		public void RemoveFromCargo( Entity entity )
		{
			m_cargo.Remove( entity );
		}

		/// <summary>
		/// Takes the damage. Just an example of the interface usage.
		/// </summary>
		public override void TakeDamage()
		{
			foreach( Entity entity in m_cargo )
				entity.TakeDamage();

			m_integratedDevice.TakeDamage();
		}

		public override void Destroy()
		{
			foreach( Entity entity in m_cargo )
				entity.Destroy();

			m_integratedDevice.Destroy();
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
	}
}