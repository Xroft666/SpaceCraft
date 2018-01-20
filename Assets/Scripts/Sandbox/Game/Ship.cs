using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using SpaceSandbox;

public class Ship : Container, IUpdatable, IDamagable
{
	public float m_health = 100f;

	public Cargo m_cargo { get; private set; }	
	public Device m_device { get; private set; }
	public TasksRunner m_tasksRunner { get; private set; }


	public Ship( float cargoCapacity )
	{
		m_cargo = new Cargo(cargoCapacity, this);
		
		m_device = new Device();
		m_device.AssignContainer( this );
		
		m_device.m_blueprint.AddCheck( "IsCargoFull", m_cargo.IsCargoFull );
		m_device.m_blueprint.m_entryPoint = m_device.m_blueprint.CreateEntry("RootEntry", m_device);
		m_device.m_isActive = false;

		m_tasksRunner = new TasksRunner ();
	}
	
	public Ship( Ship otherShip )
	{
		m_cargo = new Cargo( otherShip.m_cargo.Capacity, this );

		m_device = new Device(otherShip.m_device);
		m_device.AssignContainer( this );

		m_device.m_blueprint.AddCheck( "IsCargoFull", m_cargo.IsCargoFull );
		m_device.m_blueprint.m_entryPoint = m_device.m_blueprint.CreateEntry("RootEntry", m_device);
		m_device.m_isActive = false;

		m_tasksRunner = new TasksRunner ();
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
	void IDamagable.TakeDamage( float damage, float radius, UnityEngine.Vector2 point )
	{
		m_health = Mathf.Clamp(m_health - damage, 0f, 100f);
	}
	
	public override void Destroy()
	{
		m_health = 0f;

		m_device.Destroy();

		base.Destroy();
	}
	
	/// <summary>
	/// ContainerRepresentation -> Container -> Device calls execution flow
	/// </summary>
	
	void IUpdatable.Initialize() 
	{
		m_device.Initialize();
	}

	public void InitializeView()
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
	
	public virtual void Update() 
	{
		(m_device as IUpdatable).Update();

		if (m_device.m_isActive && !m_tasksRunner.IsRunning) 
		{
			// go through decision tree and collect all the tasks
			m_device.m_blueprint.m_entryPoint.Traverse ();	

			// execute collected tasks
			m_tasksRunner.ExecuteTasksQeue ();
		}
	}

	public override void OnDrawGizmos()
	{
		
	}
}
