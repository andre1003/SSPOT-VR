using System;
using System.Reflection;
using JetBrains.Annotations;
using SSpot.AnimatorUtilities;
using UnityEditor;
using UnityEngine;

namespace SSPot.Editor
{
    [CustomPropertyDrawer(typeof(HashedString))]
    public class HashedStringPropertyDrawer : PropertyDrawer
    {
        private const string ValueFieldName = "value";
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueProp = property.FindPropertyRelative(ValueFieldName);
            if (valueProp == null)
            {
                Debug.LogError($"Field '{ValueFieldName}' not found in {nameof(HashedString)}.");
                return;
            }
         
            var targetAttribute = fieldInfo.GetCustomAttribute<AnimatorHashedStringAttribute>();
            if (targetAttribute == null)
            {
                EditorGUI.PropertyField(position, valueProp, label);
                return;
            }

            var go = GetGameObjectFromSerializedProperty(property);
            if (go == null || !go.TryGetComponent(out Animator animator))
            {
                Debug.LogError($"{nameof(HashedString)} property {property.name} with custom attribute " +
                               $"{nameof(AnimatorParamNameAttribute)} must be in a GameObject with " +
                               "an attached Animator.");
                return;
            }
            
            if (!animator.isInitialized)
            {
                animator.enabled = false;
                animator.enabled = true;
                EditorGUI.PropertyField(position, valueProp, label);
                return;
            }

            var values = targetAttribute.GetValues(animator);
            if (values == null)
            {
                EditorGUI.PropertyField(position, valueProp, label);
                return;
            }
            
            DrawDropdown(position, label, values, valueProp);
        }

        private static void DrawDropdown(Rect position, GUIContent label, GUIContent[] values,
            SerializedProperty valueProp)
        {
            int currentIndex = Array.FindIndex(values, val => val.text == valueProp.stringValue);
            int newIndex = EditorGUI.Popup(position, label, currentIndex, values);

            if (currentIndex != newIndex)
            {
                valueProp.stringValue = values[newIndex].text;
                valueProp.serializedObject.ApplyModifiedProperties();
            }
        }

        [CanBeNull]
        private static GameObject GetGameObjectFromSerializedProperty(SerializedProperty prop) =>
            prop.serializedObject.targetObject switch
            {
                GameObject go => go,
                Component c => c.gameObject,
                _ => null
            };
    }
}