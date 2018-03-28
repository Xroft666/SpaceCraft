using System.Collections.Generic;

using SpaceSandbox;

public class Cargo : Entity
{
	public class CargoSlot
	{
		public int curItemCount;
		public float curVolume;

		public List<Entity> resources = new List<Entity>();
	}

	private Ship m_containerAttachedTo = null;
	
	public float Capacity { get; private set; } 	// amount of slots
	public float SpaceTaken { get; private set; }

	public List<CargoSlot> m_items = new List<CargoSlot>();

	public Cargo( float cap, Ship owner )
	{
		Capacity = cap;
		m_containerAttachedTo = owner;
	}

	public bool AddItem( Entity item)
	{
		CargoSlot slot = GetSlot(item.EntityName);

		if( slot == null )
		{
			slot = new CargoSlot();
			m_items.Add(slot);
		}

		//switch( item.Type )
		//{
		//case Entity.EntityType.Item:
		//	
		//	slot.curItemCount ++;
		//	slot.resources.Add(item);
		//	
		//	break;
		//	
		//case Entity.EntityType.Liquid:
		//case Entity.EntityType.Crumby:
		//	
		//	slot.curVolume += item.Volume;
		//
		//	if( slot.resources.Count > 0 )
		//	{
		//		slot.resources[0].Volume += item.Volume;
		//	}
		//	else
		//	{
		//		slot.resources.Add(item);
		//	}
		//	
		//	break;
		//}
		//
		//SpaceTaken += item.Volume;

		return true;
	}

	public bool RemoveItem( string name)
	{
		CargoSlot slot = GetSlot(name);
		if( slot == null )
			return false;

		//SpaceTaken -= slot.resources[0].Volume;
		//
		//switch( slot.resources[0].Type )
		//{
		//case Entity.EntityType.Item:
		//
		//
		//	slot.curItemCount --;
		//	slot.resources.RemoveAt(0);
		//
		//
		//	if( slot.curItemCount == 0 )
		//	{
		//		m_items.Remove(slot);
		//	}
		//
		//	break;
		//	
		//case Entity.EntityType.Liquid:
		//case Entity.EntityType.Crumby:
		//
		//	slot.resources.RemoveAt(0);
		//	m_items.Remove(slot);
		//	
		//	break;
		//}

		return true;
	}

	public void RemoveCargo()
	{
		m_items.Clear();
		SpaceTaken = 0f;
	}

	public bool IsCargoFull( System.EventArgs args )
	{
		return SpaceTaken > Capacity * 0.80f;
	}

	public CargoSlot GetSlot( string name )
	{
		foreach( CargoSlot slot in m_items )
			if( slot.resources[0].EntityName.Equals( name ) )
				return slot;

		return null;
	}

	public Entity GetItem( string name )
	{
		CargoSlot slot = GetSlot(name);
		if( slot == null )
			return null;

		return slot.resources[0];
	}

	public bool HasItem( string name )
	{
		return !(GetSlot( name ) == null);
	}

	public bool HasAnyItems()
	{
		return SpaceTaken > 0;
	}

	public TradingArgs ComposeTradeOffer( string item, string sender, string receiver )
	{
		CargoSlot slot = GetSlot(item);

		return new TradingArgs()
		{
			itemName = item,
			itemCount = slot.curItemCount,
			resourceVolume = slot.curVolume,
			credits = 0f,
			commSender = sender,
			commReceiver = receiver
		};
	}
}

