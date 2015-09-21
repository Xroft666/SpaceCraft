using UnityEngine;
using System;
using System.Collections.Generic;

using SpaceSandbox;

using UnityEngine.UI;
using UnityEngine.Events;

public class DeveloperInterface : MonoBehaviour 
{
	private RectTransform 	m_actionsContent, 
							m_eventsContent, 
							m_controlsContent, 
							m_cargoContent,
							m_installedDevices;

	private BlueprintSchemeView m_blueprintView;

	private float m_buttonsDistance = 20f;

	private Ship selectedShip;

	private void Awake()
	{
		m_actionsContent = transform.FindChild("Actions/Content") as RectTransform;
		m_eventsContent = transform.FindChild("Events/Content") as RectTransform;
		m_controlsContent = transform.FindChild("Controls/Content") as RectTransform;
		m_cargoContent = transform.FindChild("Cargo/Content") as RectTransform;
		m_installedDevices = transform.FindChild("Installed/Content") as RectTransform;
		m_blueprintView = transform.FindChild("Blueprint").GetComponent<BlueprintSchemeView>();
	}

	public void RunButtonHandler()
	{
		Job.make( selectedShip.IntegratedDevice.ActivateDevice(null), true );
	}
	
	public void StopButtonHandler()
	{
		Job.make( selectedShip.IntegratedDevice.DeactivateDevice(null), true );
	}

	public void InitializeInteface( Ship selectedContainer )
	{
		Device upMostDevice = selectedContainer.IntegratedDevice;

		InitializeActions(upMostDevice);
		InitializeEvents(upMostDevice);
		InitializeControls(upMostDevice);

		InitializeCargo(selectedContainer);
		InitializeInstalledDevices(selectedContainer);

		selectedShip = selectedContainer;


		m_blueprintView.InitializeView(upMostDevice);
	}

	private void InitializeActions(Device device)
	{
		CleanContent(m_actionsContent);

		Dictionary<string, DeviceAction> actions = new Dictionary<string, DeviceAction>();
		device.GetCompleteActionsList("", ref actions);

		FillUpContent( new List<string>(actions.Keys), m_actionsContent, "f" );
	}

	private void InitializeEvents(Device device)
	{
		CleanContent(m_eventsContent);

		Dictionary<string, DeviceEvent> events = new Dictionary<string, DeviceEvent>();
		device.GetCompleteEventsList("", ref events);

		FillUpContent( new List<string>(events.Keys), m_eventsContent, "e" );
	}

	private void InitializeControls(Device device)
	{
		CleanContent(m_controlsContent);

		m_controlsContent.sizeDelta = new Vector2(m_controlsContent.sizeDelta.x, 5 * m_buttonsDistance);

		CreateButton(m_controlsContent, "Entry", Vector3.up * -40f);
		CreateButton(m_controlsContent, "Selection", Vector3.up * -20f);
		CreateButton(m_controlsContent, "Sequence", Vector3.up * 0f);
		CreateButton(m_controlsContent, "Evaluation", Vector3.up * 20f);
		CreateButton(m_controlsContent, "Foreach", Vector3.up * 40f);
	}

	private void InitializeCargo( Ship ship )
	{
		int actionsNum = ship.m_cargo.m_items.Count;
		int count = -actionsNum / 2;
		
		m_cargoContent.sizeDelta = new Vector2(m_cargoContent.sizeDelta.x, actionsNum * m_buttonsDistance);

		foreach( Cargo.CargoSlot slot in ship.m_cargo.m_items )
		{
			string name = slot.resources[0].EntityName + " x" + slot.curItemCount;
			DeviceItem item = CreateButton(m_cargoContent, name, Vector3.up * count * m_buttonsDistance);

			item.InitializeItem( slot.resources[0] );

			count++;
		}
	}

	private void InitializeInstalledDevices( Ship ship )
	{
		int actionsNum = ship.IntegratedDevice.m_integratedDevices.Count;
		int count = -actionsNum / 2;
		
		m_cargoContent.sizeDelta = new Vector2(m_cargoContent.sizeDelta.x, actionsNum * m_buttonsDistance);
		
		foreach( Device device in ship.IntegratedDevice.m_integratedDevices )
		{
			string name = device.EntityName;
			CreateButton(m_installedDevices, name, Vector3.up * count * m_buttonsDistance);
			
			count++;
		}
	}

	private void FillUpContent( List<string> content, RectTransform contentTransform, string iconName )
	{
		int actionsNum = content.Count;
		int count = -actionsNum / 2;

		contentTransform.sizeDelta = new Vector2(contentTransform.sizeDelta.x, actionsNum * 20f);

		foreach( string item in content )
		{
			string name = iconName;
			string path = item;
			string description = "";

			CreateButton(contentTransform, path, Vector3.up * count * m_buttonsDistance);
			count++;
		}
	}

	private DeviceItem CreateButton( RectTransform parent, string itemName, Vector3 position )
	{
		Vector2 iconSize = new Vector2(100f, 20f);

		GameObject newAction = new GameObject(itemName, typeof(RectTransform));
		newAction.transform.SetParent( parent, false );
		
		RectTransform transf = newAction.GetComponent<RectTransform>();
		transf.sizeDelta = iconSize;
		
//		Button button = newAction.AddComponent<Button>();
//		button.onClick.AddListener(onClick);



		GameObject background = new GameObject("background", typeof(RectTransform));
		RectTransform backTransf = background.GetComponent<RectTransform>();
		backTransf.sizeDelta = iconSize;
		backTransf.SetParent(transf, false);

		Image backImg = background.AddComponent<Image>();


		GameObject textGO = new GameObject("text", typeof(RectTransform));
		RectTransform textTransf = textGO.GetComponent<RectTransform>();
		textTransf.sizeDelta = iconSize;
		textTransf.SetParent(transf, false);


		Text text = textGO.AddComponent<Text>();
		text.text = itemName;
		text.font = Font.CreateDynamicFontFromOSFont("Arial", 14);
		text.alignment = TextAnchor.MiddleCenter;
		text.color = Color.black;



		Selectable selectable = newAction.AddComponent<Selectable>();
		CanvasGroup canvas = newAction.AddComponent<CanvasGroup>();
		DeviceItem item = newAction.AddComponent<DeviceItem>();


		newAction.transform.localPosition = position;

		return item;
	}

	public void CleanAllContent()
	{
		selectedShip = null;

		CleanContent(m_actionsContent);
		CleanContent(m_eventsContent);
		CleanContent(m_controlsContent);
		CleanContent(m_cargoContent);
		CleanContent(m_installedDevices);
	}

	private void CleanContent( Transform content )
	{
		foreach( Transform child in content )
			GameObject.Destroy( child.gameObject );
	}
}
