﻿namespace CocodriloDog.Core {

	using System;
    using System.Collections;
    using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Base class for property drawers used in subclasses of <see cref="MonoCompositeField{T}"/>.
	/// </summary>
	/// 
	/// <remarks>
	/// This drawer allows to create, select (edit) and remove <see cref="MonoCompositeObject"/>s
	/// that will be assigned to MonoBehaviours fields.
	/// </remarks>
	public abstract class MonoCompositeFieldPropertyDrawer : PropertyDrawerBase {


		#region Unity Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			base.OnGUI(position, property, label);

			// Vars
			var rect = GetNextPosition();
			var buttonsWidth = 110f;
			var objectProperty = Property.FindPropertyRelative("m_Object");

			var fullPath = $"{Property.serializedObject.targetObject.GetInstanceID()}.{Property.propertyPath}";

			// Enable field for copy/paste
			var e = Event.current;
			if (e.type == EventType.MouseDown && e.button == 1 && Position.Contains(e.mousePosition)) {
				MonoCompositeContextMenuHandler.EnabledFieldPath = fullPath;
			}

			// The field and label
			var fieldLabelRect = rect;
			fieldLabelRect.xMax -= buttonsWidth;

			// The field
			var fieldRect = fieldLabelRect;
			if (ObjectProperty.objectReferenceValue != null) {
				fieldRect.width *= 0.7f;
			}

			// Help readability
			var enableField = fullPath == MonoCompositeContextMenuHandler.EnabledFieldPath;
			var disableField = !enableField;

			// The field is enabled while the paths are equal
			EditorGUI.BeginDisabledGroup(disableField);
			EditorGUIUtility.labelWidth -= buttonsWidth * 0.4f;
			EditorGUI.PropertyField(fieldRect, ObjectProperty, new GUIContent(Property.displayName));
			EditorGUI.EndDisabledGroup();
			EditorGUIUtility.labelWidth = 0;

			if (ObjectProperty.objectReferenceValue != null) {
				// The label
				var labelRect = fieldLabelRect;
				labelRect.width *= 0.3f;
				labelRect.x = fieldRect.xMax;
				labelRect.xMin += 5;
				labelRect.xMax -= 5;
				EditorGUI.LabelField(labelRect, (ObjectProperty.objectReferenceValue as MonoCompositeObject).ObjectName);
			}

			// Create/Edit button
			Rect createEditRect = rect;
			createEditRect.xMin += rect.width - buttonsWidth;
			createEditRect.xMax -= buttonsWidth * 0.53f;

			if (objectProperty.objectReferenceValue == null) {
				if (GUI.Button(createEditRect, "Create")) {

					// Save the path because when the object is an array element, it will need the path with the index
					// in order to assign the value to the correct slot in the deferred assignment below. Otherwise it 
					// will always assign the value to the first slot.
					var pendingPropertyPath = Property.propertyPath;

					// Show the menu only when there are more than one types
					if (MonoCompositeTypes.Count > 1) {
						var menu = new GenericMenu();
						foreach (var type in MonoCompositeTypes) {
							menu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(type.Name)), false, () => {
								CreateObject(type);
							});
						}
						menu.ShowAsContext();
					} else {
						CreateObject(MonoCompositeTypes[0]);
					}

					void CreateObject(Type t) {
						CDEditorUtility.DelayedAction(() => {

							Property.serializedObject.Update();

							var monoCompositeObject =(MonoCompositeObject)Undo.AddComponent(GameObject, t);
							monoCompositeObject.ObjectName = t.Name;
							monoCompositeObject.hideFlags = HideFlags.HideInInspector;

							Property.serializedObject.FindProperty($"{pendingPropertyPath}.m_Object").objectReferenceValue = monoCompositeObject;
							monoCompositeObject.SetParent(Property.serializedObject.targetObject as MonoBehaviour);
							pendingPropertyPath = null;

							Property.serializedObject.ApplyModifiedProperties();

						}, 0);
					}

				}
			} else {
				if (GUI.Button(createEditRect, "Edit ▸")) {					
					MonoCompositeRoot.SelectedMonoCompositeObject = (MonoCompositeObject)objectProperty.objectReferenceValue;
				}
			}

			// Remove button
			Rect removeRect = rect;
			removeRect.xMin += rect.width - buttonsWidth;
			removeRect.xMin += buttonsWidth * 0.47f; 
			if(GUI.Button(removeRect, "Remove")) {
				if (objectProperty.objectReferenceValue != null) {
					CDEditorUtility.DelayedAction(() => {
						Property.serializedObject.Update();
						objectProperty.objectReferenceValue = null;
						// Destruction is handled by MonoCompositeGOChangeHandler on ChangeGameObjectOrComponentProperties
						Property.serializedObject.ApplyModifiedProperties();
					}, 0);
				}
			}

		}

		#endregion


		#region Protected Properties

		protected abstract List<Type> MonoCompositeTypes { get; }

		#endregion


		#region Protected Methods

		protected override void InitializePropertiesForOnGUI() {
			ObjectProperty = Property.FindPropertyRelative("m_Object");
		}

		#endregion


		#region Private Fields

		private GameObject m_GameObject;

		private MonoCompositeRoot m_MonoCompositeRoot;

		#endregion


		#region Private Properties

		private GameObject GameObject {
			get {
				if (m_GameObject == null) {
					if (Property.serializedObject.targetObject is Component) {
						m_GameObject = ((Component)Property.serializedObject.targetObject).gameObject;
					}
				}
				return m_GameObject;
			}
		}

		private MonoCompositeRoot MonoCompositeRoot {
			get {
				if (m_MonoCompositeRoot == null) {
					
					var objectProperty = Property.FindPropertyRelative("m_Object");
					var monoCompositeObject = objectProperty.objectReferenceValue as MonoCompositeObject;

					MonoBehaviour parent = monoCompositeObject.Parent;
					while (!(parent is MonoCompositeRoot) && parent != null) {
						monoCompositeObject = (parent as MonoCompositeObject);
						parent = monoCompositeObject != null ? monoCompositeObject.Parent : null;
					}

					m_MonoCompositeRoot = parent as MonoCompositeRoot;

				}
				return m_MonoCompositeRoot;
			}
		}

		private SerializedProperty ObjectProperty { get; set; }

		#endregion


	}

}