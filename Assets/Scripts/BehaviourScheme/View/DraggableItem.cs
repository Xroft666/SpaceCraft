using UnityEngine;
using System.Collections;

using BehaviourScheme;
using SpaceSandbox;

public class DraggableItem : MonoBehaviour 
{
	public enum ControlType
	{
		Selection,
		Sequence, 
		Evaluation,
		Foreach
	}

	public enum QueryType
	{
		Query,
		Check
	}

	public Entity EntityContainment { get; set; }
	public Device DeviceContainment { get; set; }
	public string MethodName { get; set; }
	
	public ControlType controlType { get; set; }
	public QueryType queryType { get; set; }
}
