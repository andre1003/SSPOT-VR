using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SSpot.AnimatorUtilities
{
    public abstract class AnimatorHashedStringAttribute : Attribute
    {
        public abstract bool TryGetAnimator(Object target, [NotNullWhen(true)] out Animator animator); 
        
        public abstract GUIContent[] GetValues(Animator animator);
    }
}