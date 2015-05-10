
using System.Collections.Generic;

namespace SpaceSandbox
{
	public class Container : Entity, IDamagable 
	{
		private BlueprintScheme m_blueprint = new BlueprintScheme();

		private List<Device> m_equipment = new List<Device>();
		private List<Entity> m_cargo = new List<Entity>();

		public void LoadSoftware(BlueprintScheme blueptint)
		{
			m_blueprint = blueptint;
		}

		public void InstallEquipment( List<Device> devices )
		{
			m_equipment = devices ;
			Initialize();
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


		public void Initialize() 
		{
			foreach( Device device in m_equipment )
				device.Initialize();
		}

		public void Update() 
		{
			foreach( Device device in m_equipment )
				device.Update();
		}

		public void Delete() 
		{
			foreach( Device device in m_equipment )
				device.Delete();
		}
	}
}