using UnityEngine;
using System.Collections;

using BehaviourScheme;
using SpaceSandbox;

public class DraggableItem : MonoBehaviour 
{
	public enum ControlType
	{
		Entry,
		Selection,
		Sequence, 
		Evaluation,
		Foreach
	}

	public Entity EntityContainment { get; set; }
	public Device DeviceContainment { get; set; }

	public bool IsControlNode { get; set; }
	public ControlType controlType { get; set; }
}
