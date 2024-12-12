using System.IO;
using System.Linq;
using SSPot.Utilities;
using UnityEditor;
using UnityEngine;

namespace SSpot.Utilities.Editor
{
    [CustomPropertyDrawer(typeof(SerializableScene))]
    public class SerializableSceneDrawer : PropertyDrawer
    {
        private const string GuidPropName = "guid";
        private const string IndexPropName = "buildIndex";
        private const string NamePropName = "name";
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var guidProp = property.FindPropertyRelative(GuidPropName);
            var indexProp = property.FindPropertyRelative(IndexPropName);
            var nameProp = property.FindPropertyRelative(NamePropName);
           
            string currentGuid = guidProp.stringValue;
            
            var scenes = EditorBuildSettings.scenes;

            int currentIndex = scenes.FindIndex(s => s.guid.ToString() == currentGuid);
            if (currentGuid != "" && currentIndex == -1)
            {
                Debug.LogWarning($"Scene {AssetDatabase.GUIDToAssetPath(currentGuid)} not found in build settings.", property.serializedObject.targetObject);
            }
            
            EditorGUI.BeginProperty(position, label, property);

            var labels = scenes.Select(s => new GUIContent(Path.GetFileNameWithoutExtension(s.path))).ToArray();
            int selectedIndex = EditorGUI.Popup(position, label, currentIndex, labels);
            if (selectedIndex != currentIndex)
            {
                guidProp.stringValue = scenes[selectedIndex].guid.ToString();
                indexProp.intValue = selectedIndex;
                nameProp.stringValue = labels[selectedIndex].text;
                property.serializedObject.ApplyModifiedProperties();
            }
            
            EditorGUI.EndProperty();
        }
    }
}