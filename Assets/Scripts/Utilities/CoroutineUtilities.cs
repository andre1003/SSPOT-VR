using System;
using System.Collections;
using UnityEngine;

namespace SSpot.Utilities
{
    public static class CoroutineUtilities
    {
        public static IEnumerator SmoothCoroutine(float duration, Action<float> action)
        {
            float time = 0;
            while (time < duration)
            {
                action(time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            action(1);
        }
    }
}