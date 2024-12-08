using System.Linq;
using SSpot.Robot;
using UnityEngine;

namespace SSpot.AnimatorUtilities
{
    /// <summary>
    /// Place on a <see cref="HashedString"/> field and the editor will show animator state names in the dropdown.
    /// The script searches for <see cref="RobotData"/> from a prefab in Resources to get the owner animator.
    /// </summary>
    public class RobotAnimatorStateNameAttribute : AnimatorStateNameAttribute
    {
        private static RobotData GetRobot(Object target)
        {
            if (target is RobotData data) return data;
            if (target is GameObject go && go.TryGetComponent(out data)) return data;
            return Resources.FindObjectsOfTypeAll<RobotData>().FirstOrDefault();
        }
        
        public override bool TryGetAnimator(Object target, out Animator animator)
        {
            animator = null;

            var robot = GetRobot(target);
            if (!robot) return false;

            animator = robot.GetComponentInChildren<Animator>();
            return animator;
        }
    }
}