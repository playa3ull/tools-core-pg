﻿namespace CocodriloDog.Core {

	using UnityEngine;
	using UnityEditor;
	using System.Collections;
	using System.Collections.Generic;

	[CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
	public class MinMaxRangePropertyDrawer : PropertyDrawerBase {


		#region Public Methods

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			// I left this condition for consistency with the other property drawers.
			base.GetPropertyHeight(property, label);
			if (Property.type == typeof(MinMaxRange).Name) {
				return FieldHeight * 2;
			} else {
				return FieldHeight * 2;
			}
		}

		#endregion


		#region Unity Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			base.OnGUI(position, property, label);

			if (Property.type == typeof(MinMaxRange).Name) {
				EditorGUI.BeginProperty(position, label, property);
				EditorGUI.LabelField(GetNextPosition(), label);
				DrawMinMaxControls();
				ClampValuesToLimits();
				EditorGUI.EndProperty();
			} else {
				EditorGUI.HelpBox(
					position,
					string.Format(
						"{0} only supports {1}",
						typeof(MinMaxRangeAttribute).Name,
						typeof(MinMaxRange).Name
					),
					MessageType.Error
				);
			}

		}

		#endregion


		#region Private Properties

		private SerializedProperty MinValueProperty {
			get { return Property.FindPropertyRelative("m_MinValue"); }
		}

		private SerializedProperty MaxValueProperty {
			get { return Property.FindPropertyRelative("m_MaxValue"); }
		}

		private MinMaxRangeAttribute MinMaxRangeAttribute {
			get { return attribute as MinMaxRangeAttribute; }
		}

		#endregion


		#region Private Methods

		private Rect GetMinMaxControlsRect() {
			Rect minMaxControlsRect = GetNextPosition();
			EditorGUI.indentLevel++;
			minMaxControlsRect = EditorGUI.IndentedRect(minMaxControlsRect);
			EditorGUI.indentLevel--;
			return minMaxControlsRect;
		}

		private void DrawMinMaxControls() {

			Rect position = GetMinMaxControlsRect();

			float floatFieldWidth = position.width * 0.2f;

			Rect minFloatFieldPosition = position;
			minFloatFieldPosition.width = floatFieldWidth - 4;
			DrawMinFloat(minFloatFieldPosition, MinValueProperty, MaxValueProperty);

			Rect maxFloatFieldPosition = position;
			maxFloatFieldPosition.x = position.xMax - floatFieldWidth + 4;
			maxFloatFieldPosition.width = floatFieldWidth - 4;
			DrawMaxFloat(maxFloatFieldPosition, MinValueProperty, MaxValueProperty);

			Rect sliderPosition = position;
			sliderPosition.x += floatFieldWidth;
			sliderPosition.width -= floatFieldWidth * 2;
			DrawMinMaxSlider(sliderPosition, MinMaxRangeAttribute, MinValueProperty, MaxValueProperty);

		}

		private void DrawMinFloat(
			Rect position, SerializedProperty minValueProperty, SerializedProperty maxValueProperty
		) {
			EditorGUI.BeginChangeCheck();
			EditorGUI.PropertyField(position, minValueProperty, GUIContent.none);
			if (EditorGUI.EndChangeCheck()) {
				if (minValueProperty.floatValue > maxValueProperty.floatValue) {
					minValueProperty.floatValue = maxValueProperty.floatValue;
				}
			}
		}

		private void DrawMaxFloat(
			Rect position, SerializedProperty minValueProperty, SerializedProperty maxValueProperty
		) {
			EditorGUI.BeginChangeCheck();
			EditorGUI.PropertyField(position, maxValueProperty, GUIContent.none);
			if (EditorGUI.EndChangeCheck()) {
				if (maxValueProperty.floatValue < minValueProperty.floatValue) {
					maxValueProperty.floatValue = minValueProperty.floatValue;
				}
			}
		}

		private void DrawMinMaxSlider(
			Rect position, 
			MinMaxRangeAttribute minMaxRangeAttribute,
			SerializedProperty minValueProperty,
			SerializedProperty maxValueProperty
		) {

			float newMinValue = minValueProperty.floatValue;
			float newMaxValue = maxValueProperty.floatValue;

			EditorGUI.BeginChangeCheck();
			EditorGUI.MinMaxSlider(
				position,
				ref newMinValue, ref newMaxValue,
				minMaxRangeAttribute.MinLimit, minMaxRangeAttribute.MaxLimit
			);

			if (EditorGUI.EndChangeCheck()) {
				minValueProperty.floatValue = newMinValue;
				maxValueProperty.floatValue = newMaxValue;
			}

		}

		private void ClampValuesToLimits() {
			if (MinValueProperty.floatValue < MinMaxRangeAttribute.MinLimit) {
				MinValueProperty.floatValue = MinMaxRangeAttribute.MinLimit;
			}
			if (MinValueProperty.floatValue > MinMaxRangeAttribute.MaxLimit) {
				MinValueProperty.floatValue = MinMaxRangeAttribute.MaxLimit;
			}
			if (MaxValueProperty.floatValue < MinMaxRangeAttribute.MinLimit) {
				MaxValueProperty.floatValue = MinMaxRangeAttribute.MinLimit;
			}
			if (MaxValueProperty.floatValue > MinMaxRangeAttribute.MaxLimit) {
				MaxValueProperty.floatValue = MinMaxRangeAttribute.MaxLimit;
			}
		}

		#endregion


	}
}