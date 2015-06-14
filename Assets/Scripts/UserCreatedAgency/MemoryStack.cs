using UnityEngine;
using System.Collections.Generic;

using SpaceSandbox;

public class MemoryStack 
{
	private Dictionary<string, Entity> m_objectsInMemory = new Dictionary<string, Entity>();

	public void AddObject( string name, Entity obj )
	{
		if( !m_objectsInMemory.ContainsKey( name ) )
			m_objectsInMemory.Add( name, obj );
	}

	public void RemoveObject( string name )
	{
		if( m_objectsInMemory.ContainsKey( name ) )
			m_objectsInMemory.Remove( name );
	}

	public Entity GetObject( string name )
	{
		if( m_objectsInMemory.ContainsKey( name ) )
			return m_objectsInMemory[name];
		else
			return null;
	}

	public Dictionary<string, Entity> GetAllObjects()
	{
		return m_objectsInMemory;
	}
}
