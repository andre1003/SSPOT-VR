using System.Collections;
using System.Linq;
using Photon.Pun;
using SSpot.AnimatorUtilities;
using SSpot.Grids;
using UnityEngine;

namespace SSpot.Robot
{
    public class RobotGridMoverTester : MonoBehaviourPun
    {
        [SerializeField] private RobotGridMover mover;
        [SerializeField] private RobotAnimator animator;
        [SerializeField, RobotAnimatorStateName] private HashedString testClip;
        
        private bool _isActing;

        private IEnumerator ActCoroutine(IEnumerator action)
        {
            _isActing = true;
            yield return action;
            _isActing = false;
        }
        
        private void Act(IEnumerator action) => StartCoroutine(ActCoroutine(action));
        
        private void Update()
        {
            if (_isActing) return;
            
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Act(mover.MoveForwardCoroutine());
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Act(mover.TurnLeftCoroutine());
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Act(mover.TurnRightCoroutine());
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                Act(animator.PlayOneShotCoroutine(testClip));
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Act(InteractCoroutine());
            }
        }

        private IEnumerator InteractCoroutine()
        {
            var interactTile = mover.GridPosition + mover.Facing;
            if (!mover.Grid.TryGetNode(interactTile, out var node))
                yield break;

            var interactable = node.Objects
                .Select(o => o.gameObject.GetComponent<ExampleInteractable>())
                .FirstOrDefault(o => o );
            if (!interactable) 
                yield break;

            if (interactable.InteractionAnimation.IsValid)
                yield return animator.PlayOneShotCoroutine(interactable.InteractionAnimation);
            Debug.Log("Interacted with " + interactable.Value);
        }
    }
}