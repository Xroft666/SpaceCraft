
using System.Collections.Generic;

namespace SpaceSandbox
{
	public class Container : Entity, IDamagable 
	{
		private BlueprintScheme blueprint = null;

		private List<Device> m_equipment = new List<Device>();
		private List<Entity> m_cargo = null;

		public void SetUpEquipment( List<Device> devices )
		{
			m_equipment = devices ;
		}

		public void InstallDevice( Device device )
		{
			m_equipment.Add( device );
		}


		public Device ConvertToDevice()
		{
			Device device = new Device();
			device.SetUpIntegratedDevices( m_equipment );

			return device;
		}

		public List<Entity> GetCargoList()
		{
			return m_cargo;
		}

		void IDamagable.TakeDamage()
		{
			// open the containment
			// broke some of it
			// explode the explosive
			// or get split into smaller containers
		}
	}
}