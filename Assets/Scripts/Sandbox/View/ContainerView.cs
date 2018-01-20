using UnityEngine;

using SpaceSandbox;
using System.Collections.Generic;

using BehaviourScheme;

using UnityEngine.EventSystems;

/// <summary>
/// Container view representation mono class.
/// Passes through all the Unity-related events to the model Container class
/// </summary>

public class ContainerView : MonoBehaviour, IPointerClickHandler, ISelectHandler, IDeselectHandler, IUpdateSelectedHandler
{
	public Container m_contain;
	public int m_owner;

	private void Start()
	{
		(m_contain as IUpdatable).Initialize();

#if !UNITY_EDITOR
		GameObject marker = GameObject.CreatePrimitive( PrimitiveType.Cube );
		marker.transform.SetParent( transform, false );
		marker.transform.localScale *= 0.25f;
		marker.transform.localPosition = Vector3.up * 0.4f;
		
		Component.Destroy( marker.GetComponent<Collider>() );
#endif
	}

	private void Update()
	{
		(m_contain as IUpdatable).Update();
	}

	void OnDrawGizmos()
	{
	//	Gizmos.color = Color.red;
	//	Gizmos.DrawLine(transform.position, transform.position + transform.up);

		m_contain.OnDrawGizmos();
	}

	void IPointerClickHandler.OnPointerClick (PointerEventData eventData)
	{
		switch( eventData.clickCount )
		{
		case 1:
			EventSystem.current.SetSelectedGameObject( gameObject );
	//		eventData.selectedObject = gameObject;
			break;
		case 2:
			UIController.Instance.OnContainerDeveloperConsole( this );
			break;
		}
		

	}

	void ISelectHandler.OnSelect(BaseEventData eventData)
	{
		UIController.Instance.OnContainerSelected( this );
	}

	void IDeselectHandler.OnDeselect(BaseEventData eventData)
	{
		UIController.Instance.OnContainerDeselected( this );
	}

	void IUpdateSelectedHandler.OnUpdateSelected (BaseEventData eventData)
	{
		UIController.Instance.OnContainerUpdated( this );
	}
}
