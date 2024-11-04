using System;
using System.Collections;
using UnityEngine;

namespace SSPot.Utilities
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

        public static IEnumerator WaitThen(float waitSeconds, Action action)
        {
            yield return new WaitForSeconds(waitSeconds);
            action();
        }

        public static IEnumerator WaitThenDeactivate(float waitSeconds, GameObject objectToDeactivate)
        {
            yield return new WaitForSeconds(waitSeconds);
            objectToDeactivate.SetActive(false);
        }
    }
}