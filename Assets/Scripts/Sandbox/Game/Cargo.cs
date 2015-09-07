using System.Collections.Generic;

using SpaceSandbox;

public class Cargo : Entity
{
	public class CargoSlot
	{
		public int maxItemCap = 5;
		public float maxResVol = 1f;

		public int curItemCount = 0;
		public float curVolume = 0f;

		public Entity resource;
	}

	private Ship m_containerAttachedTo = null;
	
	public int Capacity { get; private set; } 	// amount of slots
	public int SpaceTaken { get; private set; }

	public List<CargoSlot> m_items = new List<CargoSlot>();

	public Cargo( int cap, Ship owner )
	{
		Capacity = cap;
		m_containerAttachedTo = owner;
//		for( int i = 0; i < cap; i++ )
//			m_items.Add( new CargoSlot() );
	}

	public bool AddItem( Entity item, int num = 1 )
	{
		if( SpaceTaken + item.Volume > Capacity )
			return false;

		CargoSlot slot = GetSlot(item.EntityName);

		if( slot == null )
		{
			slot = new CargoSlot();
			SpaceTaken ++;
			m_items.Add(slot);
		}

		switch( item.Type )
		{
		case Entity.EntityType.Item:
			
			slot.curItemCount += num;
			
			break;
			
		case Entity.EntityType.Liquid:
		case Entity.EntityType.Crumby:
			
			slot.curVolume += item.Volume;
			slot.resource.Volume += item.Volume;
			
			break;
		}
		
		slot.resource = item;

		return true;
	}

	public bool RemoveItem( string name, int num = 1 )
	{
		CargoSlot slot = GetSlot(name);
		if( slot == null )
			return false;

		switch( slot.resource.Type )
		{
		case Entity.EntityType.Item:
			
			slot.curItemCount -= num;

			if( slot.curItemCount == 0 )
			{
//				m_items.Remove(slot);
				SpaceTaken --;
			}

			break;
			
		case Entity.EntityType.Liquid:
		case Entity.EntityType.Crumby:
			
//			m_items.Remove(slot);
			SpaceTaken --;
			
			break;
		}

		return true;
	}

	public void RemoveCargo()
	{
		m_items.Clear();
		SpaceTaken = 0;
	}

	public bool IsCargoFull( System.EventArgs args )
	{
		return SpaceTaken > Capacity * 0.80f;
	}

	public CargoSlot GetSlot( string name )
	{
		foreach( CargoSlot slot in m_items )
			if( slot.resource.EntityName.Equals( name ) )
				return slot;

		return null;
	}

	public Entity GetItem( string name )
	{
		CargoSlot slot = GetSlot(name);
		if( slot == null )
			return null;

		return slot.resource;
	}

	public bool HasItem( string name )
	{
		return !(GetSlot( name ) == null);
	}

	public bool HasAnyItems()
	{
		return SpaceTaken > 0;
	}

	public TradingArgs ComposeTradeOffer( string name )
	{
		CargoSlot slot = GetSlot(name);

		return new TradingArgs()
		{
			itemName = name,
			itemCount = slot.curItemCount,
			resourceVolume = slot.curVolume,
			credits = 0f,
			requestSender = m_containerAttachedTo
		};
	}
}

