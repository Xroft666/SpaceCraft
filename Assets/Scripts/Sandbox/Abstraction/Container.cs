
using System.Collections.Generic;

namespace SpaceSandbox
{
	public class Container : Entity 
	{
		private ContainerRepresentation m_representation = null;
		public ContainerRepresentation Representation
		{
			get { return m_representation; }
		}

		/// <summary>
		/// The m_generated device. Each container represents a compund device,
		/// with blueprint logic attached.
		/// </summary>
		private Device m_integratedDevice = null;

		/// <summary>
		/// The m_blueprint. The blueprint logic scheme storage.
		/// </summary>
		private BlueprintScheme m_blueprint = new BlueprintScheme();
		public BlueprintScheme Blueprint
		{
			get { return m_blueprint; }
			set { m_blueprint = value; }
		}


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
		public override void TakeDamage()
		{
			UnityEngine.Debug.Log( EntityName + " took damage.");
			foreach( Entity entity in m_cargo )
				entity.TakeDamage();
		}

		public override void Destroy()
		{
			UnityEngine.Debug.Log( EntityName + " is destroyed.");
			foreach( Entity entity in m_cargo )
				entity.TakeDamage();
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

		public void OnObjectEntered( Container container )
		{
			m_blueprint.Memory.AddObject(container.EntityName, container);
		}

		public void OnObjectEscaped( Container container )
		{
			m_blueprint.Memory.RemoveObject(container.EntityName);
		}
	}
}