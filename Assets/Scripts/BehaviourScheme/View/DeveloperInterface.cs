using UnityEngine;
using System;
using System.Collections.Generic;

using SpaceSandbox;

using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DeveloperInterface : MonoBehaviour
{
	private DraggableItemsScrollRect	m_actionsScrollView,
										m_eventsScrollView,
										m_controlsScrollView,
										m_cargoScrollView,
										m_installedScrollView,
										m_queriesScrollView;

	private BlueprintSchemeView m_blueprintView;

	private float m_buttonsDistance = 20f;

	private Ship selectedShip;

	private void Awake()
	{
		m_actionsScrollView = transform.FindChild("Actions").GetComponent<DraggableItemsScrollRect>();
		m_eventsScrollView = transform.FindChild("Events").GetComponent<DraggableItemsScrollRect>();
		m_controlsScrollView = transform.FindChild("Controls").GetComponent<DraggableItemsScrollRect>();
		m_cargoScrollView = transform.FindChild("Cargo").GetComponent<DraggableItemsScrollRect>();
		m_installedScrollView = transform.FindChild("Installed").GetComponent<DraggableItemsScrollRect>();
		m_queriesScrollView = transform.FindChild("Queries").GetComponent<DraggableItemsScrollRect>();



		m_cargoScrollView.m_onDropHandlers[m_installedScrollView] = OnCargoToInstalledDropped;
		m_installedScrollView.m_onDropHandlers[m_cargoScrollView] = OnDevicesToCargoDropped;



		m_blueprintView = transform.FindChild("Blueprint").GetComponent<BlueprintSchemeView>();


		m_blueprintView.m_onDropHandlers[m_actionsScrollView] = OnActionsToBlueprintDropped;
		m_blueprintView.m_onDropHandlers[m_eventsScrollView] = OnEventsToBlueprintDropped;
		m_blueprintView.m_onDropHandlers[m_queriesScrollView] = OnQueriesToBlueprintDropped;
		m_blueprintView.m_onDropHandlers[m_controlsScrollView] = OnControlsToBlueprintDropped;
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
		InitializeQueries(selectedContainer);

		selectedShip = selectedContainer;


		m_blueprintView.InitializeView(upMostDevice);
	}

	private void InitializeActions(Device device)
	{
		CleanContent(m_actionsScrollView.content);

		Dictionary<string, DeviceAction> actions = new Dictionary<string, DeviceAction>();
		device.GetCompleteActionsList("", ref actions);

		FillUpContent( new List<string>(actions.Keys), m_actionsScrollView.content, "f" );
	}

	private void InitializeEvents(Device device)
	{
		CleanContent(m_eventsScrollView.content);

		Dictionary<string, DeviceEvent> events = new Dictionary<string, DeviceEvent>();
		device.GetCompleteEventsList("", ref events);

		FillUpContent( new List<string>(events.Keys), m_eventsScrollView.content, "e" );
	}

	private void InitializeControls(Device device)
	{
		CleanContent(m_controlsScrollView.content);

		m_controlsScrollView.content.sizeDelta = new Vector2(m_controlsScrollView.content.sizeDelta.x, 5 * m_buttonsDistance);

		DraggableItem item;

		item = CreateButton(m_controlsScrollView.content, "Entry", Vector3.up * -40f);
		item.IsControlNode = true;
		item.controlType = DraggableItem.ControlType.Entry;

		item = CreateButton(m_controlsScrollView.content, "Selection", Vector3.up * -20f);
		item.IsControlNode = true;
		item.controlType = DraggableItem.ControlType.Selection;

		item = CreateButton(m_controlsScrollView.content, "Sequence", Vector3.up * 0f);
		item.IsControlNode = true;
		item.controlType = DraggableItem.ControlType.Sequence;

		item = CreateButton(m_controlsScrollView.content, "Evaluation", Vector3.up * 20f);
		item.IsControlNode = true;
		item.controlType = DraggableItem.ControlType.Evaluation;

		item = CreateButton(m_controlsScrollView.content, "Foreach", Vector3.up * 40f);
		item.IsControlNode = true;
		item.controlType = DraggableItem.ControlType.Foreach;
	}

	private void InitializeCargo( Ship ship )
	{
		int actionsNum = ship.m_cargo.m_items.Count;
		int count = -actionsNum / 2;
		
		m_cargoScrollView.content.sizeDelta = new Vector2(m_cargoScrollView.content.sizeDelta.x, actionsNum * m_buttonsDistance);

		foreach( Cargo.CargoSlot slot in ship.m_cargo.m_items )
		{
			string name = slot.resources[0].EntityName + " x" + slot.curItemCount;

			DraggableItem item = CreateButton(m_cargoScrollView.content, name, Vector3.up * count * m_buttonsDistance);

			count++;
		}
	}

	private void InitializeInstalledDevices( Ship ship )
	{
		int actionsNum = ship.IntegratedDevice.m_integratedDevices.Count;
		int count = -actionsNum / 2;
		
		m_installedScrollView.content.sizeDelta = new Vector2(m_installedScrollView.content.sizeDelta.x, actionsNum * m_buttonsDistance);
		
		foreach( Device device in ship.IntegratedDevice.m_integratedDevices )
		{
			string name = device.EntityName;
			CreateButton(m_installedScrollView.content, name, Vector3.up * count * m_buttonsDistance);
			
			count++;
		}
	}

	private void InitializeQueries( Ship ship )
	{
		CleanContent( m_queriesScrollView.content );

		Dictionary<string, DeviceQuery> queries = new Dictionary<string, DeviceQuery>();
		ship.IntegratedDevice.GetCompleteQueriesList("", ref queries);

		Dictionary<string, DeviceCheck> checks = new Dictionary<string, DeviceCheck>();
		ship.IntegratedDevice.GetCompleteChecksList("", ref checks);

		List<string> queryNames = new List<string>();
		queryNames.AddRange(new List<string>(queries.Keys));
		queryNames.AddRange(new List<string>(checks.Keys));

		FillUpContent( queryNames, m_queriesScrollView.content, "q" );
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

	private DraggableItem CreateButton( RectTransform parent, string itemName, Vector3 position )
	{
		Vector2 iconSize = new Vector2(100f, 20f);

		GameObject newAction = new GameObject(itemName, typeof(RectTransform));
		newAction.transform.SetParent( parent, false );
		
		RectTransform transf = newAction.GetComponent<RectTransform>();
		transf.sizeDelta = iconSize;

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


		newAction.transform.localPosition = position;

		return newAction.AddComponent<DraggableItem>();
	}

	public void CleanAllContent()
	{
		selectedShip = null;

		CleanContent(m_actionsScrollView.content);
		CleanContent(m_eventsScrollView.content);
		CleanContent(m_controlsScrollView.content);
		CleanContent(m_cargoScrollView.content);
		CleanContent(m_installedScrollView.content);
	}

	private void CleanContent( Transform content )
	{
		foreach( Transform child in content )
			GameObject.Destroy( child.gameObject );
	}

	


	public void OnActionsToBlueprintDropped( PointerEventData eventData)
	{

	}

	public void OnEventsToBlueprintDropped( PointerEventData eventData)
	{
	

	}

	public void OnControlsToBlueprintDropped(PointerEventData eventData)
	{
		DraggableItem draggableItem = eventData.selectedObject.GetComponent<DraggableItem>();

		NodeView node = null;

		switch(draggableItem.controlType)
		{
		case DraggableItem.ControlType.Entry:
			node = m_blueprintView.CreateEntry();
			break;
		case DraggableItem.ControlType.Evaluation:
			node = m_blueprintView.CreateEvaluate();
			break;
		case DraggableItem.ControlType.Foreach:
			node = m_blueprintView.CreateForeach();
			break;
		case DraggableItem.ControlType.Selection:
			node = m_blueprintView.CreateSelect();
			break;
		case DraggableItem.ControlType.Sequence:
			node = m_blueprintView.CreateSequence();
			break;
		}

		Vector2 locPos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle( m_blueprintView.transform as RectTransform, eventData.position, UIController.s_UICamera, out locPos);

		node.transform.localPosition = locPos;
	}

	public void OnQueriesToBlueprintDropped(PointerEventData eventData)
	{
	

	}

	public void OnCargoToInstalledDropped(PointerEventData eventData, DraggableItemsScrollRect scrollView)
	{
		scrollView.AssignObject(eventData.selectedObject);
	}

	public void OnDevicesToCargoDropped(PointerEventData eventData, DraggableItemsScrollRect scrollView)
	{
		scrollView.AssignObject(eventData.selectedObject);
	}


}
