using UnityEngine;
using System.Collections;

using BehaviourScheme;

using UnityEngine.EventSystems;

// the visual representation of a node
// it s a monobehaviour, so it contains all the callbacks, visuals and references 
public class NodeView : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	public BSNode Node { get; private set; }
	public BlueprintSchemeView SchemeView { get; private set; }

	private Camera m_uiCamera;

	private void Awake()
	{
		m_uiCamera = transform.root.GetComponent<Camera>();
	}

	public void InitializeNode( BSNode node, BlueprintSchemeView schemeView )
	{
		Node = node;
		SchemeView = schemeView;

		// initialize visuals that depends on the type

		if( (node as BSEntry) != null )
		{
			// Initialize as Entry
		}
		else if( (node as BSExit) != null )
		{
			// Initialize as Exit
		}
		else if( (node as BSAction) != null )
		{
			// Initialize as Action
		}
		else if( (node as BSEvaluate) != null )
		{
			// Initialize as Evaluate
		}
		else if( (node as BSBranch) != null )
		{
			// Initialize as Select
		}
	}
	
	#region IDragHandler implementation
	public void OnDrag (PointerEventData eventData)
	{
		Vector2 localPos;

		RectTransformUtility.ScreenPointToLocalPointInRectangle( 
			transform.parent as RectTransform, eventData.position, m_uiCamera, out localPos );

		transform.localPosition = localPos;
	}
	#endregion

	#region IPointerClickHandler implementation

	public void OnPointerClick (PointerEventData eventData)
	{

	}

	#endregion

	#region IBeginDragHandler implementation

	public void OnBeginDrag (PointerEventData eventData)
	{

	}

	#endregion

	#region IEndDragHandler implementation

	public void OnEndDrag (PointerEventData eventData)
	{

	}

	#endregion
}
