using System;
using System.Collections;
using Photon.Pun;
using SSpot.Grids;
using UnityEngine;

namespace SSpot.Robot
{
    public class RobotGridMover : MonoBehaviourPun, ILevelGridObject
    {
        [SerializeField] private RobotAnimator animator;
        
        [SerializeField] private float walkTime = 1;
        
        [SerializeField] private float turnTime = 1;

        public LevelGrid Grid { get; set; }
        
        public Vector2Int GridPosition { get; set; }

        [field: SerializeField]
        public Vector2Int Facing { get; private set; } = Vector2Int.right;

        private void Awake()
        {
            Facing = new((int)transform.forward.x, (int)transform.forward.z);
            if (Facing is { x: 0, y: 0 } or { x: 1, y: 1 })
            {
                Facing = Vector2Int.right;
                transform.forward = Vector3.right;
            }
        }

        public IEnumerator MoveForwardCoroutine()
        {
            var fromCell = GridPosition;
            var toCell = fromCell + Facing;

            if (!Grid.InGrid(toCell))
            {
                Debug.LogError("Can't walk out of grid!");
                yield break;
            }

            var from = Grid.GetCellCenterWorld(fromCell);
            var to = Grid.GetCellCenterWorld(toCell);

            animator.BeginWalking();
            float time = 0;
            while (time < walkTime)
            {
                float t = time / walkTime;
                transform.position = Vector3.Lerp(from, to, t);
                time += Time.deltaTime;
                yield return null;
            }

            Grid.ChangeNode(this, toCell);
            animator.StopWalking(); 
        }

        public IEnumerator TurnLeftCoroutine() => TurnCoroutine(left: true);
        public IEnumerator TurnRightCoroutine() => TurnCoroutine(left: false);
        
        private IEnumerator TurnCoroutine(bool left)
        {
            if (left) animator.TurnLeft();
            else animator.TurnRight();
    
            //TODO make more precise and support root rotation
            Vector3 original = transform.forward;
            Vector3 target = transform.right;
            if (left) target = -target;
            
            float time = 0;
            while (time < turnTime)
            {
                float t = time / turnTime;
                transform.forward = Vector3.Slerp(original, target, t);
                time += Time.deltaTime;
                yield return null;
            }

            transform.forward = target;
            Facing = new(Mathf.RoundToInt(target.x), Mathf.RoundToInt(target.z));
        }

        GameObject ILevelGridObject.GameObject => gameObject;

        private static Vector2Int RotateFacingLeft(Vector2Int facing)
        {
            if (facing == Vector2Int.right) return Vector2Int.up;
            if (facing == Vector2Int.up) return Vector2Int.left;
            if (facing == Vector2Int.left) return Vector2Int.down;
            if (facing == Vector2Int.down) return Vector2Int.right;
            throw new ArgumentException();
        }
        
        private static Vector2Int RotateFacingRight(Vector2Int facing)
        {
            if (facing == Vector2Int.right) return Vector2Int.down;
            if (facing == Vector2Int.down) return Vector2Int.left;
            if (facing == Vector2Int.left) return Vector2Int.up;
            if (facing == Vector2Int.up) return Vector2Int.right;
            throw new ArgumentException();
        }
    }
}