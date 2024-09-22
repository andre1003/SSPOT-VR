using System.Reflection;
using JetBrains.Annotations;
using SSpot;
using UnityEditor;
using UnityEngine;

namespace SSPot.Editor
{
    [CustomPropertyDrawer(typeof(HashedString))]
    public class HashedStringEditor : PropertyDrawer
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
         
            var targetAttribute = fieldInfo.GetCustomAttribute<AnimatorParamNameAttribute>();
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
            
            DrawParameterDropdown(position, label, animator, valueProp);
        }

        private static void DrawParameterDropdown(Rect position, GUIContent label, Animator animator,
            SerializedProperty valueProp)
        {

            if (!animator.isInitialized)
            {
                animator.enabled = false;
                animator.enabled = true;
                return;
            }
            
            var parameters = animator.parameters;
            var values = new GUIContent[parameters.Length];
            int currentIndex = -1;
            for (int i = 0; i < parameters.Length; i++)
            {
                values[i] = new(parameters[i].name);
                if (parameters[i].name == valueProp.stringValue)
                    currentIndex = i;
            }
            
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