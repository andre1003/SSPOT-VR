using System;
using UnityEditor;
using UnityEngine;

namespace SSPot.Utilities.Editor
{
    /// <summary>
    /// A utility struct to temporarily set the label width in the Unity editor GUI.
    /// When disposed, it restores the original label width.
    /// </summary>
    public readonly struct LabelWidthBlock : IDisposable
    {
        private readonly float _ogWidth;
        
        public LabelWidthBlock(float width)
        {
            _ogWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = width;
        }

        public LabelWidthBlock(GUIContent labelContent) : this(EditorStyles.label.CalcSize(labelContent).x)
        {
        }
        
        public void Dispose()
        {
            EditorGUIUtility.labelWidth = _ogWidth;
        }
    }
}