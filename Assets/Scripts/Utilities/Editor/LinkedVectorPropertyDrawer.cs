using System.Collections.Generic;
using System.Linq;
using SSPot.Utilities.Attributes;
using UnityEditor;
using UnityEngine;

namespace SSPot.Utilities.Editor
{
    [CustomPropertyDrawer(typeof(LinkedVectorAttribute))]
    public class LinkedVectorPropertyDrawer : PropertyDrawer
    {
        private enum FieldType { None, Int, Float }
        
        private static readonly string[] PropNames = {"x", "y", "z", "w"};

        private static readonly GUIContent ToggleLabel = new("Link");
        
        private bool _constrained = true;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var props = GetVectorProps(property, out bool isInt);
            if (props == null)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }
            
            int changeIndex = -1;

            if (_constrained && !AreAllValuesEqual(props, isInt)) 
                _constrained = false;
            
            //Constrained toggle
            Rect toggleRect = DrawToggle(position, ref _constrained, ref changeIndex);
            
            //Label
            Rect labelRect = new(position) {xMax = toggleRect.x};
            GUI.Label(labelRect, label);

            //Fields
            float subRectX = toggleRect.xMax + EditorGUIUtility.standardVerticalSpacing; 
            DrawFields(position, subRectX, props, ref changeIndex);

            //Apply
            if (!_constrained || changeIndex == -1)
                return;

            for (int i = 0; i < props.Count; i++)
            {
                if (i == changeIndex) continue;
                
                if (isInt)
                    props[i].intValue = props[changeIndex].intValue;
                else
                    props[i].floatValue = props[changeIndex].floatValue;
            }
            
            property.serializedObject.ApplyModifiedProperties();
        }

        private static bool AreAllValuesEqual(List<SerializedProperty> props, bool isInt) => isInt
            ? props.All(p => p.intValue == props[0].intValue)
            : props.All(p => Mathf.Approximately(p.floatValue, props[0].floatValue));

        private static List<SerializedProperty> GetVectorProps(SerializedProperty property, out bool isInt)
        {
            var props = PropNames.Select(property.FindPropertyRelative)
                .Where(p => p != null)
                .ToList();
            
            if (props.Count == 0)
            {
                Debug.LogWarning($"{property.type} is not a vector. Must have x, y, z or w to use {nameof(LinkedVectorAttribute)}. {property.name} will not be drawn.");
                isInt = false;
                return null;
            }
            
            FieldType fieldType = GetVectorFieldType(props);
            if (fieldType == FieldType.None)
            {
                Debug.LogWarning($"{nameof(LinkedVectorAttribute)} can only be used with int or float vectors. {property.name} will not be drawn.");
                isInt = false;
                return null;
            }
            
            isInt = fieldType == FieldType.Int;
            return props;
        }
        
        private static FieldType GetVectorFieldType(List<SerializedProperty> props)
        {
            bool allTheSame = props.All(p => p.propertyType == props[0].propertyType);
            if (!allTheSame) return FieldType.None;

            return props[0].propertyType switch
            {
                SerializedPropertyType.Integer => FieldType.Int,
                SerializedPropertyType.Float => FieldType.Float,
                _ => FieldType.None
            };
        }
        
        private static Rect DrawToggle(Rect fullPosition, ref bool constrained, ref int changeIndex)
        {
            float toggleWidth = EditorStyles.toggle.CalcSize(ToggleLabel).x;
            var toggleRect = new Rect(fullPosition)
            {
                x = fullPosition.x + EditorGUIUtility.labelWidth - toggleWidth,
                width = toggleWidth
            };

            using (new LabelWidthBlock(ToggleLabel))
            {
                EditorGUI.BeginChangeCheck();
                constrained = EditorGUI.Toggle(toggleRect, ToggleLabel, constrained);
                if (EditorGUI.EndChangeCheck())
                    changeIndex = 0;
            }
            
            return toggleRect;
        }

        private static void DrawFields(Rect fullPosition, float startX, List<SerializedProperty> props, ref int changeIndex)
        {
            float subRectWidth = fullPosition.xMax - startX;
            if (props.Count == 4) subRectWidth /= 4;
            else subRectWidth /= 3;

            using (new LabelWidthBlock(new GUIContent("X")))
            {
                for (int i = 0; i < props.Count; i++)
                {
                    var subRect = new Rect(fullPosition)
                    {
                        x = startX + i * subRectWidth,
                        width = subRectWidth
                    };
                
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.PropertyField(subRect, props[i]);
                    if (EditorGUI.EndChangeCheck())
                        changeIndex = i;
                }
            }
        }
    }
}