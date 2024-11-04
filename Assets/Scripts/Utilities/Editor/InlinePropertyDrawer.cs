using SSPot.Utilities.Attributes;
using UnityEditor;
using UnityEngine;

namespace SSPot.Utilities.Editor
{
    [CustomPropertyDrawer(typeof(InlineAttribute))]
    public class InlinePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.Next(true)
                ? EditorGUI.GetPropertyHeight(property, label)
                : base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.Next(true))
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            Debug.LogError($"Property {property.name} must have at least one child to use {nameof(InlineAttribute)} attribute"); 
            EditorGUI.PropertyField(position, property, label);
        }
    }
}