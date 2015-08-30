using UnityEngine;
using System.Collections.Generic;

using SpaceSandbox;

public class Ship : Container {

	/// <summary>
	/// The m_cargo. The inventory of this specific container
	/// </summary>
//	private List<Entity> m_cargo = new List<Entity>();
	private Cargo m_cargo = new Cargo();
	
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
		m_cargo.AddItem( entity );
	}
	
	public List<Entity> GetCargoList()
	{
		return m_cargo.m_items;
	}
	
	public void RemoveFromCargo( Entity entity )
	{
		m_cargo.RemoveItem( entity );
	}
	
	/// <summary>
	/// Takes the damage. Just an example of the interface usage.
	/// </summary>
	public override void TakeDamage( float damage, float radius, UnityEngine.Vector2 point )
	{
		// calculate what happens on taking damage
		// if too high, destroy, drop items etc
	}
	
	public override void Destroy()
	{
		foreach( Entity entity in m_cargo.m_items )
			entity.Destroy();
		
		m_integratedDevice.Destroy();

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
		
		GameObject body = new GameObject("body");
		
		SpriteRenderer sRenderer = body.AddComponent<SpriteRenderer>();
		sRenderer.sprite = WorldManager.World.m_visuals;
		
		body.transform.SetParent( view.transform, false );
		
		Rigidbody2D rigid = newContainer.gameObject.AddComponent<Rigidbody2D>();
		rigid.gravityScale = 0f;
		rigid.drag = 1.35f;
		rigid.angularDrag = 0.1f;
		
		BoxCollider2D clickZone = newContainer.AddComponent<BoxCollider2D>();
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
		IntegratedDevice.CleanScheduledEvents();
	}

	public override void OnDrawGizmos(){}
}
