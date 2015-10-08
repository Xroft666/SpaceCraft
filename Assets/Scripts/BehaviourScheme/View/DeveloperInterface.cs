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



		m_cargoScrollView.m_onDropHandlers[m_installedScrollView] = OnDevicesToCargoDropped;
		m_installedScrollView.m_onDropHandlers[m_cargoScrollView] = OnCargoToInstalledDropped;



		m_blueprintView = transform.FindChild("Blueprint").GetComponent<BlueprintSchemeView>();


		m_blueprintView.m_onDropHandlers[m_actionsScrollView] = OnActionsToBlueprintDropped;
		m_blueprintView.m_onDropHandlers[m_eventsScrollView] = OnEventsToBlueprintDropped;
		m_blueprintView.m_onDropHandlers[m_queriesScrollView] = OnQueriesToBlueprintDropped;
		m_blueprintView.m_onDropHandlers[m_controlsScrollView] = OnControlsToBlueprintDropped;
	}

	public void RunButtonHandler()
	{
		//Job.make( selectedShip.IntegratedDevice.ActivateDevice(null), true );

		selectedShip.IntegratedDevice.m_isActive = true;
		selectedShip.IntegratedDevice.Initialize();
	}
	
	public void StopButtonHandler()
	{
		//Job.make( selectedShip.IntegratedDevice.DeactivateDevice(null), true );

		selectedShip.IntegratedDevice.m_isActive = false;
		selectedShip.IntegratedDevice.Blueprint.tasksRunner.KillTasks();

		selectedShip.IntegratedDevice.Destroy();
	}

	public void InitializeInteface( Ship selectedContainer )
	{
		selectedShip = selectedContainer;

		Device upMostDevice = selectedContainer.IntegratedDevice;

		InitializeActions(upMostDevice);
		InitializeEvents(upMostDevice);
		InitializeQueries(upMostDevice);

		InitializeControls(upMostDevice);

		InitializeCargo(selectedContainer);
		InitializeInstalledDevices(selectedContainer);





		m_blueprintView.InitializeView(upMostDevice);
	}

	private void InitializeActions(Device device)
	{
		CleanContent(m_actionsScrollView.content);

		Dictionary<string, DeviceAction> actions = new Dictionary<string, DeviceAction>();
		device.GetCompleteActionsList("", ref actions);

		FillUpContent( new List<string>(actions.Keys), m_actionsScrollView.content, device);

	}

	private void InitializeEvents(Device device)
	{
		CleanContent(m_eventsScrollView.content);

		Dictionary<string, DeviceEvent> events = new Dictionary<string, DeviceEvent>();
		device.GetCompleteEventsList("", ref events);

		FillUpContent( new List<string>(events.Keys), m_eventsScrollView.content, device );
	}

	private void InitializeQueries( Device device )
	{
		CleanContent( m_queriesScrollView.content );
		
		Dictionary<string, DeviceQuery> queries = new Dictionary<string, DeviceQuery>();
		device.GetCompleteQueriesList("", ref queries);
		
		Dictionary<string, DeviceCheck> checks = new Dictionary<string, DeviceCheck>();
		device.GetCompleteChecksList("", ref checks);
		
	//	List<string> queryNames = new List<string>();
	//	queryNames.AddRange(new List<string>(queries.Keys));
	//	queryNames.AddRange(new List<string>(checks.Keys));
		
//		FillUpContent( queryNames, m_queriesScrollView.content, device);

		int actionsNum = queries.Keys.Count + checks.Keys.Count;
		int count = -actionsNum / 2;
		
		m_queriesScrollView.content.sizeDelta = new Vector2(m_queriesScrollView.content.sizeDelta.x, actionsNum * 20f);
		
		foreach( string item in queries.Keys )
		{
			string[] descriptor = item.Split('.');
			
			string name = descriptor[1];
			string path = descriptor[0];
			string description = "";
			
			DraggableItem draggableItem = CreateButton(m_queriesScrollView.content, name, Vector3.up * count * m_buttonsDistance);
			
			draggableItem.DeviceContainment = device.GetInternalDevice(path);
			draggableItem.MethodName = name;
			draggableItem.queryType = DraggableItem.QueryType.Query;
			
			count++;
		}

		foreach( string item in checks.Keys )
		{
			string[] descriptor = item.Split('.');
			
			string name = descriptor[1];
			string path = descriptor[0];
			string description = "";
			
			DraggableItem draggableItem = CreateButton(m_queriesScrollView.content, name, Vector3.up * count * m_buttonsDistance);
			
			draggableItem.DeviceContainment = device.GetInternalDevice(path);
			draggableItem.MethodName = name;
			draggableItem.queryType = DraggableItem.QueryType.Check;

			count++;
		}
	}

	private void InitializeControls(Device device)
	{
		CleanContent(m_controlsScrollView.content);

		m_controlsScrollView.content.sizeDelta = new Vector2(m_controlsScrollView.content.sizeDelta.x, 5 * m_buttonsDistance);

		for( int i = 0; i < 4; i++ )
		{
			DraggableItem item = CreateButton(m_controlsScrollView.content, ((DraggableItem.ControlType) i).ToString(), Vector3.up * ( -40f + 20f * i ));

			item.controlType = (DraggableItem.ControlType) i;
		}
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

			Container container = slot.resources[0] as Container;
			Device device = slot.resources[0] as Device;
			
			if( container != null )
			{
				item.EntityContainment = container;
			}

			if( device != null )
			{
				item.DeviceContainment = device;
			}

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
			DraggableItem item = CreateButton(m_installedScrollView.content, name, Vector3.up * count * m_buttonsDistance);
			item.DeviceContainment = device;

			count++;
		}
	}

	private void FillUpContent( List<string> content, RectTransform contentTransform, Device device )
	{
		int actionsNum = content.Count;
		int count = -actionsNum / 2;
		
		contentTransform.sizeDelta = new Vector2(contentTransform.sizeDelta.x, actionsNum * 20f);
		
		foreach( string item in content )
		{
			string[] descriptor = item.Split('.');
			
			string name = descriptor[1];
			string path = descriptor[0];
			string description = "";
			
			DraggableItem draggableItem = CreateButton(contentTransform, name, Vector3.up * count * m_buttonsDistance);
			
			draggableItem.DeviceContainment = device.GetInternalDevice(path);
			draggableItem.MethodName = name;
			
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

		m_blueprintView.CleanBlueprint();
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
		DraggableItem draggableItem = eventData.selectedObject.GetComponent<DraggableItem>();
		
		NodeView node = m_blueprintView.CreateAction( draggableItem.DeviceContainment, draggableItem.MethodName );
		
		Vector2 locPos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle( m_blueprintView.transform as RectTransform, eventData.position, UIController.s_UICamera, out locPos);
		
		node.transform.localPosition = locPos;
	}

	public void OnEventsToBlueprintDropped( PointerEventData eventData)
	{
		DraggableItem draggableItem = eventData.selectedObject.GetComponent<DraggableItem>();

		NodeView node = m_blueprintView.CreateEntry( draggableItem.DeviceContainment, draggableItem.MethodName );

		Vector2 locPos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle( m_blueprintView.transform as RectTransform, eventData.position, UIController.s_UICamera, out locPos);
		
		node.transform.localPosition = locPos;
	}

	public void OnQueriesToBlueprintDropped(PointerEventData eventData)
	{
		DraggableItem draggableItem = eventData.selectedObject.GetComponent<DraggableItem>();
		
		NodeView node = null;

		if( draggableItem.queryType == DraggableItem.QueryType.Query )
			node = m_blueprintView.CreateQuery( draggableItem.DeviceContainment, draggableItem.MethodName );
		else
			node = m_blueprintView.CreateCheck( draggableItem.DeviceContainment, draggableItem.MethodName );

		Vector2 locPos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle( m_blueprintView.transform as RectTransform, eventData.position, UIController.s_UICamera, out locPos);
		
		node.transform.localPosition = locPos;
		
	}

	public void OnControlsToBlueprintDropped(PointerEventData eventData)
	{
		DraggableItem draggableItem = eventData.selectedObject.GetComponent<DraggableItem>();

		NodeView node = null;

		switch(draggableItem.controlType)
		{
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


	public void OnCargoToInstalledDropped(PointerEventData eventData, DraggableItemsScrollRect scrollView)
	{
		DraggableItem draggableItem = eventData.selectedObject.GetComponent<DraggableItem>();

		if( draggableItem.DeviceContainment != null )
		{
			selectedShip.m_cargo.RemoveItem(draggableItem.DeviceContainment.EntityName);
			selectedShip.IntegratedDevice.InstallDevice( draggableItem.DeviceContainment );

			CleanContent(m_actionsScrollView.content);
			CleanContent(m_eventsScrollView.content);
			CleanContent(m_queriesScrollView.content);

			InitializeActions(selectedShip.IntegratedDevice);
			InitializeEvents(selectedShip.IntegratedDevice);
			InitializeQueries(selectedShip.IntegratedDevice);

			scrollView.AssignObject(eventData.selectedObject);
			
			m_blueprintView.CleanBlueprint();
			m_blueprintView.InitializeView(selectedShip.IntegratedDevice);
		}
	}

	public void OnDevicesToCargoDropped(PointerEventData eventData, DraggableItemsScrollRect scrollView)
	{
		DraggableItem draggableItem = eventData.selectedObject.GetComponent<DraggableItem>();
		if( draggableItem.DeviceContainment != null )
		{
			selectedShip.IntegratedDevice.UninstallDevice( draggableItem.DeviceContainment );
			selectedShip.m_cargo.AddItem( draggableItem.DeviceContainment );

			CleanContent(m_actionsScrollView.content);
			CleanContent(m_eventsScrollView.content);
			CleanContent(m_queriesScrollView.content);
			
			InitializeActions(selectedShip.IntegratedDevice);
			InitializeEvents(selectedShip.IntegratedDevice);
			InitializeQueries(selectedShip.IntegratedDevice);

			scrollView.AssignObject(eventData.selectedObject);

			m_blueprintView.CleanBlueprint();
			m_blueprintView.InitializeView(selectedShip.IntegratedDevice);
		}
	}


}
