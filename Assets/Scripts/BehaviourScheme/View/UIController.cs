using UnityEngine;
using System.Collections.Generic;

using UnityEngine.UI;
using SpaceSandbox;

public class UIController : MonoBehaviour 
{
	public GameObject m_selectionGroupPrefab;

	public RectTransform m_selectListTransform;
	public RectTransform m_commandsTransform;
	public RectTransform m_hpBarTransform;

	

	private Dictionary<ContainerView, GameObject> selections = new Dictionary<ContainerView, GameObject>();
	private ContainerView selectedContainer = null;

	private CommandsStack commands = new CommandsStack();

	private void Start()
	{
		commands.InitializeStack(m_commandsTransform, 5);
	}

	private void Update()
	{
		MouseClickHandler();
		UtilsKeys();
		CameraControls();
		UpdateSelections();
	}

	private void MouseClickHandler()
	{
		if( Input.GetMouseButtonUp( 0 ) )
		{
			selectedContainer = null;
			HideAllSelections();
			commands.CleanCommandsStack();

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] hits = Physics.RaycastAll( ray );

//			RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction);
//			foreach( RaycastHit2D hit in hits )
			foreach( RaycastHit hit in hits )
			{
				if( hit.collider == null )
					continue;
			
				ContainerView view = hit.collider.GetComponent<ContainerView>();
				if( view != null )
				{
					selectedContainer = view;

					commands.InitializeContainerView(selectedContainer);
					OnContainerSelectedEvent(selectedContainer);


					break;
				}
			}
		}
	}
	
	private void UtilsKeys()
	{
		if( Input.GetKeyUp(KeyCode.R) )
		{
			Application.LoadLevel( Application.loadedLevel );
		}
	}
	
	private void CameraControls()
	{
		if( selectedContainer == null || !selectedContainer.gameObject.activeInHierarchy )
		{
			Vector3 input = Vector3.zero; 
			if( Input.GetKey(KeyCode.UpArrow) )
				input += Vector3.forward;
			
			if( Input.GetKey(KeyCode.DownArrow) )
				input += Vector3.back;
			
			if( Input.GetKey(KeyCode.LeftArrow) )
				input += Vector3.left;
			
			if( Input.GetKey(KeyCode.RightArrow) )
				input = Vector3.right;
			
			Camera.main.transform.position += input * Time.deltaTime;
		}
		else
		{
			Vector3 position = Vector3.zero;
			position.x = selectedContainer.transform.position.x;
			position.z = selectedContainer.transform.position.z;
			position.y = Camera.main.transform.position.y;
			
			Camera.main.transform.position = position;
		}
		
		Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + Input.GetAxis( "Mouse ScrollWheel"), 1f, 10f);
	}

	private void UpdateSelections()
	{
		foreach( KeyValuePair<ContainerView, GameObject> selection in selections )
		{
			if( selection.Value.activeInHierarchy )
			{
				// Should be changed for events instead if update checks
				if( !selection.Key.gameObject.activeInHierarchy )
				{
					selection.Value.SetActive(false);
					return;
				}

				selection.Value.transform.localPosition = Camera.main.WorldToScreenPoint( selection.Key.transform.position );

				Ship ship = selection.Key.m_contain as Ship;
				Asteroid aster = selection.Key.m_contain as Asteroid;
				
				if( ship != null )
					selection.Value.transform.FindChild("Cargo").GetComponent<Text>().text = "Cargo: " +
						ship.m_cargo.SpaceTaken.ToString("0.0") + " / " + ship.m_cargo.Capacity.ToString("0.0");
				
				if( aster != null )
					selection.Value.transform.FindChild("Cargo").GetComponent<Text>().text = "Cargo: " +
						aster.Containment.Amount.ToString("0.0");
			}
		}
	}

	private void OnContainerSelectedEvent( ContainerView container )
	{
		m_hpBarTransform.gameObject.SetActive(true);

		if( selections.ContainsKey( container ) )
		{
			selections[container].SetActive(true);
			return;
		}

		GenerateNewSelection( container );
	}

	private void HideAllSelections()
	{
		foreach( GameObject selectionGO in selections.Values )
			selectionGO.SetActive( false );

		m_hpBarTransform.gameObject.SetActive(false);
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
