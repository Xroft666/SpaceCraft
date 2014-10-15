using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MaterialSystem{
	public class ElementList : MonoBehaviour {

		public List<ElementSpecs> elements = new List<ElementSpecs>();

		public static ElementList Instance;
		// Use this for initialization
		void Awake () {
			if(Instance == null){
				Instance = this;
			}else{
				Destroy(gameObject);
			}
		}
		
		// Update is called once per frame
		void Update () {
			
		}
	}
}
