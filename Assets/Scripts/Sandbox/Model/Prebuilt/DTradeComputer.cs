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

	private IEnumerator LoadItemsFrom ( EventArgs args )
	{
		TradingArgs tArgs = args as TradingArgs;
		Cargo clientCargo = tArgs.requestSender.m_cargo;

		yield return new WaitForSeconds(2f);

		Cargo.CargoSlot slot = clientCargo.GetSlot(tArgs.itemName);

		m_containerAttachedTo.m_cargo.AddItem( slot.resource, slot.curItemCount );
		clientCargo.RemoveItem(tArgs.itemName, tArgs.itemCount);
	} 
	
	private IEnumerator UnloadItemsTo( EventArgs args )
	{
		TradingArgs tArgs = args as TradingArgs;
		Cargo clientCargo = tArgs.requestSender.m_cargo;

		yield return new WaitForSeconds(2f);

		Cargo.CargoSlot slot = m_containerAttachedTo.m_cargo.GetSlot(tArgs.itemName);

		clientCargo.AddItem( slot.resource, slot.curItemCount );
		m_containerAttachedTo.m_cargo.RemoveItem(tArgs.itemName, tArgs.itemCount);
	}
	

	public override void OnDeviceInstalled()
	{
		AddAction("LoadItemsFrom", LoadItemsFrom);
		AddAction("UnloadItemsTo", UnloadItemsTo);
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
