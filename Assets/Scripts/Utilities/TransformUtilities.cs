using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SSpot.Utilities
{
    public static class TransformUtilities
    {
        public static IEnumerable<Transform> Children(this Transform transform)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                yield return transform.GetChild(i);
            }
        }
        
        public static IEnumerable<Transform> ActiveChildren(this Transform transform) => 
            Children(transform).Where(child => child.gameObject.activeSelf);
    }
}