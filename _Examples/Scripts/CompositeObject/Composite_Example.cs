namespace CocodriloDog.Core.Examples {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class Composite_Example : MonoBehaviour {

		[Header("Composite Example (Without Root)")]

		[SerializeField]
		public string OtherProperty = "Other Property";

		[SerializeField]
		public string OtherProperty2 = "Other Property2";

		[SerializeReference]
		public Dog SingleDog;

	}

}