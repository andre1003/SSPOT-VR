using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace SSpot.Robot
{
    public class RobotGridMoverTester : MonoBehaviourPun
    {
        [SerializeField] private RobotGridMover mover;
        [SerializeField] private RobotAnimator animator;
        [SerializeField] private AnimationClip testClip;
        
        
        private bool _isActing;

        private IEnumerator ActCoroutine(IEnumerator action)
        {
            _isActing = true;
            yield return action;
            _isActing = false;
        }
        
        private void Update()
        {
            if (_isActing) return;
            
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                StartCoroutine(ActCoroutine(mover.MoveForwardCoroutine()));
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                StartCoroutine(ActCoroutine(mover.TurnLeftCoroutine()));
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                StartCoroutine(ActCoroutine(mover.TurnRightCoroutine()));
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(ActCoroutine(animator.PlayClipCoroutine(testClip)));
            }
        }
    }
}