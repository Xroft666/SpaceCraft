using UnityEngine;
using System.Collections;

namespace MaterialSystem{
	public class ElementStats{
		public float temperature;	//temperature
		public float fragmentionRate;	//increases each impact, overall strength is (hardness*flexibility)*(1-fragmentationRate)
	}
}
