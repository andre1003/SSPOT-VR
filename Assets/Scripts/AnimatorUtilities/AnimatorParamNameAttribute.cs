using System;
using UnityEngine;

namespace SSpot.AnimatorUtilities
{
    /// <summary>
    /// Place on a <see cref="HashedString"/> field and the editor will show animator parameter names in the dropdown.
    /// The field must be a property of a component attached to an object with an Animator. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class AnimatorParamNameAttribute : AnimatorHashedStringAttribute
    {
        public override GUIContent[] GetValues(Animator animator)
        {
            var parameters = animator.parameters;
            var values = new GUIContent[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                values[i] = new(parameters[i].name);
            }

            return values;
        }
    }
}