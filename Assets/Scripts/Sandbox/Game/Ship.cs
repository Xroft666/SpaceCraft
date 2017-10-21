using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using SpaceSandbox;

public class Ship : Container
{
	public float m_health = 100f;

	public Cargo m_cargo { get; private set; }	

	public Device IntegratedDevice { get; private set; }

	public Ship( float cargoCapacity )
	{
		m_cargo = new Cargo(cargoCapacity, this);
		
		IntegratedDevice = new Device();
		IntegratedDevice.AssignContainer( this );
		
		IntegratedDevice.AddCheck( "IsCargoFull", m_cargo.IsCargoFull );

		IntegratedDevice.Blueprint.m_entryPoint = IntegratedDevice.Blueprint.CreateEntry("RootEntry", IntegratedDevice);

		IntegratedDevice.m_isActive = false;
	}
	
	public Ship( Ship otherShip )
	{
		m_cargo = new Cargo( otherShip.m_cargo.Capacity, this );
		IntegratedDevice = otherShip.IntegratedDevice;
		IntegratedDevice.AssignContainer( this );
	}

	
	public void AddToCargo( Entity entity )
	{
		m_cargo.AddItem( entity );
	}
	
	public void RemoveFromCargo( string name )
	{
		m_cargo.RemoveItem( name );
	}

	/// <summary>
	/// Takes the damage. Just an example of the interface usage.
	/// </summary>
	public override void TakeDamage( float damage, float radius, UnityEngine.Vector2 point )
	{
		m_health = Mathf.Clamp(m_health - damage, 0f, 100f);
	}
	
	public override void Destroy()
	{
		m_health = 0f;

		IntegratedDevice.Destroy();

		base.Destroy();
	}
	
	/// <summary>
	/// ContainerRepresentation -> Container -> Device calls execution flow
	/// </summary>
	
	public override void Initialize() 
	{
		IntegratedDevice.Initialize();
	}

	public override void InitializeView()
	{
		GameObject newContainer = new GameObject( EntityName );
				
		ContainerView view = newContainer.AddComponent<ContainerView>();
		View = view;
		view.m_contain = this;
		
		GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
		body.name = "body";
		Component.Destroy(body.GetComponent<Collider>());
		body.GetComponent<MeshRenderer>().sharedMaterial = new UnityEngine.Material(Shader.Find("Sprites/Diffuse"));

		body.transform.SetParent( view.transform, false );

		Rigidbody rigid = newContainer.gameObject.AddComponent<Rigidbody>();
		rigid.constraints = RigidbodyConstraints.FreezePositionY | 
							RigidbodyConstraints.FreezeRotationX | 
							RigidbodyConstraints.FreezeRotationZ;
		rigid.useGravity = false;

		rigid.drag = 1.35f;
		rigid.angularDrag = 0.1f;

		BoxCollider clickZone = newContainer.AddComponent<BoxCollider>();

		newContainer.layer = 12;
	}

	public override void UpdateView()
	{
		
	}
	
	public override void Update() 
	{
		IntegratedDevice.Update();
		IntegratedDevice.ExecuteLogic();
	}

	public override void LateUpdate()
	{

	}

	public override void OnDrawGizmos(){}
}
