using UnityEngine;
using System;
using System.Collections.Generic;

using SpaceSandbox;
using BehaviourScheme;

using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DeveloperInterface : MonoBehaviour
{
	public DraggableItemsScrollRect	m_actionsScrollView,
										m_eventsScrollView,
										m_controlsScrollView,
										m_cargoScrollView,
										m_installedScrollView,
										m_queriesScrollView;

	public BlueprintSchemeView m_blueprintView;

	private float m_buttonsDistance = 20f;

	public Text m_pathText;

	private Ship selectedShip;
	private Device selectedDevice;

	private void Awake()
	{
		//m_pathText = transform.Find("Names/path").GetComponent<Text>();
		m_pathText.raycastTarget = true;
		Button pathBtn = m_pathText.gameObject.AddComponent<Button>();
		pathBtn.onClick.AddListener(() => { OnPathButtonHandler(); });


		//m_actionsScrollView = transform.Find("Actions").GetComponent<DraggableItemsScrollRect>();
		//m_eventsScrollView = transform.Find("Events").GetComponent<DraggableItemsScrollRect>();
		//m_controlsScrollView = transform.Find("Controls").GetComponent<DraggableItemsScrollRect>();
		//m_cargoScrollView = transform.Find("Cargo").GetComponent<DraggableItemsScrollRect>();
		//m_installedScrollView = transform.Find("Installed").GetComponent<DraggableItemsScrollRect>();
		//m_queriesScrollView = transform.Find("Queries").GetComponent<DraggableItemsScrollRect>();



		m_cargoScrollView.m_onDropHandlers[m_installedScrollView] = OnDevicesToCargoDropped;
		m_installedScrollView.m_onDropHandlers[m_cargoScrollView] = OnCargoToInstalledDropped;



		//m_blueprintView = transform.Find("Blueprint").GetComponent<BlueprintSchemeView>();


		m_blueprintView.m_onDropHandlers[m_actionsScrollView] = OnActionsToBlueprintDropped;
		m_blueprintView.m_onDropHandlers[m_eventsScrollView] = OnEventsToBlueprintDropped;
		m_blueprintView.m_onDropHandlers[m_queriesScrollView] = OnQueriesToBlueprintDropped;
		m_blueprintView.m_onDropHandlers[m_controlsScrollView] = OnControlsToBlueprintDropped;
	}

	public void RunButtonHandler()
	{
		foreach( KeyValuePair<string, Ship> ship in WorldManager.s_containersCache )
		{
			ship.Value.IntegratedDevice.m_isActive = true;
			ship.Value.IntegratedDevice.Initialize();
		}
	}
	
	public void StopButtonHandler()
	{
		foreach( KeyValuePair<string, Ship> ship in WorldManager.s_containersCache )
		{
			ship.Value.IntegratedDevice.m_isActive = false;
			ship.Value.IntegratedDevice.TasksRunner.KillTasks();

			ship.Value.IntegratedDevice.Destroy();
		}
	}

	public void RestartButtonHandler()
	{
		Application.LoadLevel( Application.loadedLevel );
	}

	public void CloseButtonHandler()
	{
		CleanAllContent();
		gameObject.SetActive(false);
		EventSystem.current.SetSelectedGameObject( null );
	}

	public void InitializeInteface( Ship selectedContainer )
	{
		selectedShip = selectedContainer;
		selectedDevice = selectedContainer.IntegratedDevice;


		InitializeActions(selectedDevice);
		InitializeEvents(selectedDevice);
		InitializeQueries(selectedDevice);
		InitializeControls(selectedDevice);

		InitializeCargo(selectedContainer);
		InitializeInstalledDevices(selectedContainer);

		m_blueprintView.InitializeView(selectedDevice, this);

		m_pathText.text = selectedContainer.EntityName + ": " + selectedDevice.EntityName;
	}

	private void InitializeActions(Device device)
	{
		CleanContent(m_actionsScrollView.content);

		Dictionary<string, DeviceAction> actions = new Dictionary<string, DeviceAction>();
		device.GetCompleteActionsList("", ref actions);

		Dictionary<string, BSEntry> exits = new Dictionary<string, BSEntry>();
		device.GetCompleteExitsList("", ref exits);

//		FillUpContent( new List<string>(actions.Keys), m_actionsScrollView.content, device);

		int actionsNum = actions.Keys.Count + exits.Keys.Count;
		int count = -actionsNum / 2;
		
		m_actionsScrollView.content.sizeDelta = new Vector2(m_actionsScrollView.content.sizeDelta.x, actionsNum * 20f);
		
		foreach( string item in actions.Keys )
		{
			string[] descriptor = item.Split('.');
			
			string name = descriptor[1];
			string path = descriptor[0];
			string description = "";
			
			DraggableItem draggableItem = CreateButton(m_actionsScrollView.content, name, Vector3.up * count * m_buttonsDistance);
			
			draggableItem.DeviceContainment = device.GetInternalDevice(path);
			draggableItem.MethodName = name;
			draggableItem.eventType = DraggableItem.DeviceActionType.Action;
			
			count++;
		}
		
		foreach( string item in exits.Keys )
		{
			string[] descriptor = item.Split('.');
			
			string name = descriptor[1];
			string path = descriptor[0];
			string description = "";
			
			DraggableItem draggableItem = CreateButton(m_actionsScrollView.content, name, Vector3.up * count * m_buttonsDistance);
			
			draggableItem.DeviceContainment = device.GetInternalDevice(path);
			draggableItem.MethodName = name;
			draggableItem.eventType = DraggableItem.DeviceActionType.Entry;
			
			count++;
		}
	}

	private void InitializeEvents(Device device)
	{
		CleanContent(m_eventsScrollView.content);

		Dictionary<string, DeviceTrigger> events = new Dictionary<string, DeviceTrigger>();
		device.GetCompleteTriggersList("", ref events);

		FillUpContent( new List<string>(events.Keys), m_eventsScrollView.content, device );
	}

	private void InitializeQueries( Device device )
	{
		CleanContent( m_queriesScrollView.content );
		
		Dictionary<string, DeviceQuery> queries = new Dictionary<string, DeviceQuery>();
		device.GetCompleteQueriesList("", ref queries);
		
		Dictionary<string, DeviceCheck> checks = new Dictionary<string, DeviceCheck>();
		device.GetCompleteChecksList("", ref checks);


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

		for( int i = 0; i < 5; i++ )
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
		backImg.sprite = UIController.Instance.m_spriteTexture;

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
		
		NodeView node = null;

		if( draggableItem.eventType == DraggableItem.DeviceActionType.Action )
			node = m_blueprintView.CreateAction( draggableItem.DeviceContainment, draggableItem.MethodName );
		else
			node = m_blueprintView.CreateExit( draggableItem.DeviceContainment, draggableItem.MethodName );
		
		Vector2 locPos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle( m_blueprintView.transform as RectTransform, eventData.position, UIController.s_UICamera, out locPos);
		
		node.transform.localPosition = locPos;
	}

	public void OnEventsToBlueprintDropped( PointerEventData eventData)
	{
		DraggableItem draggableItem = eventData.selectedObject.GetComponent<DraggableItem>();

		NodeView node = m_blueprintView.CreateTrigger( draggableItem.DeviceContainment, draggableItem.MethodName );

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
		case DraggableItem.ControlType.Entry:
			node = m_blueprintView.CreateEntry(m_blueprintView.m_device, "UserEntry");
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
			m_blueprintView.InitializeView(selectedShip.IntegratedDevice, this);
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
			m_blueprintView.InitializeView(selectedShip.IntegratedDevice, this);
		}
	}

	public void NodeViewOpenInternal( NodeView view )
	{
		BSExit exit = view.Node as BSExit;
		if( exit == null )
			return;


		selectedDevice = exit.m_device;

		m_blueprintView.CleanBlueprint();
		//	m_blueprintView.InitializeView(selectedDevice, this);

		BSEntry entry = selectedDevice.GetEntry( exit.m_entryName );

		m_blueprintView.GenerateTree( entry );
		m_blueprintView.PositionNode( entry );
		m_blueprintView.GenerateConnections( entry );
	}

	public void OnPathButtonHandler()
	{
		selectedDevice = selectedShip.IntegratedDevice;

		//m_blueprintView.CleanBlueprint();
		//
		//BSEntry entry = selectedDevice.GetEntry( "RootEntry" );
		//
		//m_blueprintView.GenerateTree( entry );
		//m_blueprintView.PositionNode( entry );
		//m_blueprintView.GenerateConnections( entry );
	}
}
