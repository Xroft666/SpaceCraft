using System.Collections.Generic;

using SpaceSandbox;

public class Cargo : Entity
{
	// free space
	public float Capacity { get; set; } 
	public float SpaceTaken { get; private set; }

	public List<Entity> m_items = new List<Entity>();

	public void AddItem(Entity item)
	{
		// check if such item already there, so combine it in one slot
		// check if enough storage space in cargo

		m_items.Add( item );
	}

	public void RemoveItem( Entity item )
	{
		// if it is combined slot, decreate the amount of items in one slot
		m_items.Remove( item );
	}

	public bool IsCargoFull()
	{
		return SpaceTaken == Capacity;
	}
}

