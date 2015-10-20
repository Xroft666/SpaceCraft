using UnityEngine;
using System.Collections.Generic;

using UnityEngine.UI;
using SpaceSandbox;

using UnityEngine.EventSystems;

public class UIController : MonoBehaviour
{
	static public UIController Instance { get; private set; }

	public static Camera s_UICamera;
	public static Canvas s_Canvas;

	public GameObject m_selectionGroupPrefab;

	public RectTransform m_selectListTransform;
	public RectTransform m_commandsTransform;
	public RectTransform m_hpBarTransform;
	public RectTransform m_devInterface;
	

	private Dictionary<ContainerView, GameObject> selections = new Dictionary<ContainerView, GameObject>();

	private CommandsStack commands = new CommandsStack();
	private DeveloperInterface devUI;

	private void Awake()
	{
		Instance = this;
		devUI = m_devInterface.GetComponent<DeveloperInterface>();

		s_UICamera = transform.GetComponentInParent<Camera>();
		s_Canvas = transform.GetComponentInParent<Canvas>();
	}

	private void Start()
	{
		commands.InitializeStack(m_commandsTransform, 5);
	}

	private void Update()
	{
		UtilsKeys();
	}
	
	private void UtilsKeys()
	{
		if( Input.GetKeyUp(KeyCode.R) )
		{
			Application.LoadLevel( Application.loadedLevel );
		}

		if( Input.GetKeyUp(KeyCode.Escape) )
		{
			devUI.CleanAllContent();
			m_devInterface.gameObject.SetActive(false);
			EventSystem.current.SetSelectedGameObject( null );
		}
	}


	public void OnContainerSelected( ContainerView container )
	{
		m_hpBarTransform.gameObject.SetActive(true);
		RectTransform hpBar = m_hpBarTransform.GetChild(0) as RectTransform;

		hpBar.sizeDelta = new Vector2(100f, 7.5f);

		Ship ship = container.m_contain as Ship;
		if( ship != null )
			(m_hpBarTransform.GetChild(0) as RectTransform).sizeDelta = new Vector2( ship.m_health, 7.5f );


		commands.InitializeContainerView(container);

		if( selections.ContainsKey( container ) )
		{
			selections[container].SetActive(true);
			return;
		}

		GenerateNewSelection( container );
	}

	public void OnContainerDeselected( ContainerView container )
	{
		selections[container].SetActive(false);
		m_hpBarTransform.gameObject.SetActive(false);
		commands.CleanCommandsStack();
	}

	public void OnContainerUpdated( ContainerView container )
	{
		selections[container].transform.localPosition = Camera.main.WorldToScreenPoint( container.transform.position );
		
		Ship ship = container.m_contain as Ship;
		Asteroid aster = container.m_contain as Asteroid;
		
		if( ship != null )
			selections[container].transform.FindChild("Cargo").GetComponent<Text>().text = "Cargo: " +
				ship.m_cargo.SpaceTaken.ToString("0.0") + " / " + ship.m_cargo.Capacity.ToString("0.0");
		
		if( aster != null )
			selections[container].transform.FindChild("Cargo").GetComponent<Text>().text = "Cargo: " +
				aster.Containment.Amount.ToString("0.0");



		Vector3 position = Vector3.zero;
		position.x = container.transform.position.x;
		position.z = container.transform.position.z;
		position.y = Camera.main.transform.position.y;


		Camera.main.transform.position = position;
	}

	public void OnContainerDeveloperConsole( ContainerView container )
	{
		Ship ship = container.m_contain as Ship;
		if( ship != null )
		{
			selections[container].SetActive(false);
			m_hpBarTransform.gameObject.SetActive(false);
			commands.CleanCommandsStack();


			m_devInterface.gameObject.SetActive(true);
			devUI.InitializeInteface(ship);
		}
	}

	private void GenerateNewSelection( ContainerView container )
	{
		Vector2 selectionScreenPos = Camera.main.WorldToScreenPoint( container.transform.position );

		GameObject newSelection = Instantiate(m_selectionGroupPrefab);
		newSelection.transform.parent = m_selectListTransform;
		
		RectTransform newSelectTransform = newSelection.GetComponent<RectTransform>();
		newSelectTransform.localPosition = selectionScreenPos;
		newSelectTransform.localRotation = Quaternion.identity;

		newSelectTransform.localScale = Vector3.one;

		newSelection.transform.FindChild("Name").GetComponent<Text>().text = container.name;

		
		selections.Add( container, newSelection );
	}
	

	private void OnDestroy()
	{
		selections.Clear();
	}
}
