using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using SpaceSandbox;

/// <summary>
/// Timer Device. Counts to the specified second and triggers the event
/// </summary>
public class DTradeComputer : Device 
{

	private IEnumerator LoadItemsFrom ( DeviceQuery qry )//EventArgs args )
	{
		TradingArgs tArgs = qry.Invoke() as TradingArgs;
		Cargo clientCargo = tArgs.requestSender.m_cargo;

		yield return new WaitForSeconds(2f);

		Cargo.CargoSlot slot = clientCargo.GetSlot(tArgs.itemName);

		for( int i = 0; i < slot.resources.Count; i++ )
		{
			m_container.m_cargo.AddItem( slot.resources[i] );
			clientCargo.RemoveItem(tArgs.itemName);
		}
	} 
	
	private IEnumerator UnloadItemsTo( DeviceQuery qry )//EventArgs args )
	{
		TradingArgs tArgs = qry.Invoke() as TradingArgs;
		Cargo clientCargo = tArgs.requestSender.m_cargo;

		yield return new WaitForSeconds(2f);

		Cargo.CargoSlot slot = m_container.m_cargo.GetSlot(tArgs.itemName);

		for( int i = 0; i < slot.resources.Count; i++ )
		{
			clientCargo.AddItem( slot.resources[i] );
			m_container.m_cargo.RemoveItem(tArgs.itemName);
		}
	}
	

	public override void OnDeviceInstalled()
	{
		m_blueprint.AddAction("LoadItemsFrom", LoadItemsFrom);
		m_blueprint.AddAction("UnloadItemsTo", UnloadItemsTo);
	}

	public override void OnDeviceUninstalled()
	{
		m_blueprint.RemoveAction("LoadItemsFrom");
		m_blueprint.RemoveAction("UnloadItemsTo");
	}

	public override void Initialize()
	{

	}

	public override void Update()
	{

	}

	public override void Destroy ()
	{

	}

}
