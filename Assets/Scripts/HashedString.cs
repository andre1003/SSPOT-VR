using System;
using UnityEngine;

namespace SSpot
{
    /// <summary>
    /// Place on a <see cref="HashedString"/> field and the editor will show animator parameter names in the dropdown.
    /// The field must be a property of a component attached to an object with an Animator. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class AnimatorParamNameAttribute : Attribute
    {
    }
    
    /// <summary>
    /// Represents a string and its cached int hash value from <see cref="Animator.StringToHash"/>.
    /// Used for faster access of animator parameters and animation states.
    /// </summary>
    [Serializable]
    public struct HashedString
    {
        [SerializeField] private string value;
        
        public string Value
        {
            get => value;
            set
            {
                _hash = null;
                this.value = value;
            }
        }

        private int? _hash;
        public int Hash => _hash ??= Animator.StringToHash(Value);

        public static implicit operator int(HashedString obj) => obj.Hash;
    }
}