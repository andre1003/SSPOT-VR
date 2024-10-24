using System;
using UnityEngine;

namespace SSpot.AnimatorUtilities
{
    public abstract class AnimatorHashedStringAttribute : Attribute
    {
        public abstract GUIContent[] GetValues(Animator animator);
    }
}