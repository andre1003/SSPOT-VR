using System;
using System.Collections.Generic;
using System.Linq;
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
        public override GUIContent[] GetValues(Animator animator)
        {
#if !UNITY_EDITOR
            return Array.Empty<GUIContent>();
#else
            var controller = (UnityEditor.Animations.AnimatorController)animator.runtimeAnimatorController;
            return controller.layers
                .SelectMany(l => l.stateMachine.states)
                .Select(state => new GUIContent(state.state.name))
                .ToArray();
#endif
        }
    }
}