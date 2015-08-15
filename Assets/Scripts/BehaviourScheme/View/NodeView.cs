using UnityEngine;
using System.Collections;

using BehaviourScheme;

// the visual representation of a node
// it s a monobehaviour, so it contains all the callbacks, visuals and references 
public class NodeView : MonoBehaviour 
{
	public BSNode Node { get; private set; }
	public BlueprintSchemeView SchemeView { get; private set; }

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
		else if( (node as BSFunction) != null )
		{
			// Initialize as Function
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

	#region callbacks

	// Attach unity's interaction components to provide those callbacks
	// provide data back to BlueprintSchemeView

	public void OnNodeClicked()
	{

	}

	public void OnNodeDragged()
	{

	}

	public void OnNodePressed()
	{

	}

	public void OnNodeReleased()
	{

	}

	#endregion
}
