﻿namespace CocodriloDog.Core {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// A base class to create fields in MonoBehaviours with references to 
    /// <see cref="MonoScriptableObject"/> that are saved in the MonoBehaviour as opposed 
    /// to being saved as an asset.
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// The type of the <see cref="MonoScriptableObject"/> to be referenced
    /// </typeparam>
    public abstract class MonoScriptableField<T> where T : MonoScriptableObject {


		#region Public Properties

		public T Object => m_Object;

		#endregion


		#region Public Methods

		public void SetObject(T value) => m_Object = value;

		#endregion


		#region Private Fields

		[SerializeField]
        private T m_Object;

		#endregion


	}

}