﻿using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace SSpot.AnimatorUtilities
{
    /// <summary>
    /// Place on a <see cref="HashedString"/> field and the editor will show animator state names in the dropdown.
    /// The field must be a property of a component attached to an object with an Animator. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class AnimatorStateNameAttribute : AnimatorHashedStringAttribute
    {
        [CanBeNull]
        public override GUIContent[] GetValues(Animator animator)
        {
#if !UNITY_EDITOR
            return Array.Empty<GUIContent>();
#else
            if (animator.runtimeAnimatorController is not UnityEditor.Animations.AnimatorController controller)
            {
                return null;
            }
            
            return controller.layers
                .SelectMany(l => l.stateMachine.states)
                .Select(state => new GUIContent(state.state.name))
                .ToArray();
#endif
        }
    }
}