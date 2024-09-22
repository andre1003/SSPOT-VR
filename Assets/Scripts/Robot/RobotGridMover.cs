using System;
using System.Collections;
using Photon.Pun;
using SSpot.Grids;
using UnityEngine;

namespace SSpot.Robot
{
    [RequireComponent(typeof(RobotAnimator))]
    public class RobotGridMover : MonoBehaviourPun, ILevelGridObject
    {
        [SerializeField] private float walkTime = 1;
        
        [SerializeField] private float turnTime = 1;

        [SerializeField] private bool useRootRotation;
        
        public LevelGrid Grid { get; set; }
        
        public Vector2Int GridPosition { get; set; }
        
        private Vector2Int _facing;
        public Vector2Int Facing
        {
            get => _facing;
            set => SetFacing(value);
        }

        private RobotAnimator _animator;
        
        private void Awake()
        {
            Facing = new((int)transform.forward.x, (int)transform.forward.z);
            _animator = GetComponent<RobotAnimator>();
        }

        public IEnumerator MoveForwardCoroutine()
        {
            var fromCell = GridPosition;
            var toCell = fromCell + Facing;

            if (!Grid.InGrid(toCell))
            {
                Debug.Log("Can't walk out of grid!");
                yield break;
            }

            Vector3 from = Grid.GetCellCenterWorld(fromCell);
            Vector3 to = Grid.GetCellCenterWorld(toCell);

            _animator.BeginWalking();
            yield return SmoothCoroutine(walkTime, t => transform.position = Vector3.Lerp(from, to, t));
            _animator.StopWalking();
            yield return _animator.WaitForAnimationCoroutine();
            
            Grid.ChangeNode(this, toCell);
        }

        public IEnumerator TurnLeftCoroutine() => TurnCoroutine(left: true);
        public IEnumerator TurnRightCoroutine() => TurnCoroutine(left: false);
        
        private IEnumerator TurnCoroutine(bool left)
        {
            if (left) _animator.TurnLeft();
            else _animator.TurnRight();
            
            if (useRootRotation)
            {
                _animator.EnableRootMotion();
                yield return _animator.WaitForAnimationCoroutine();
                _animator.DisableRootMotion();
            }
            
            Vector3 originalForward = transform.forward;
            Vector2Int target = left ? RotateFacingLeft(Facing) : RotateFacingRight(Facing);
            Vector3 targetForward = new(target.x, 0, target.y);
            yield return SmoothCoroutine(turnTime,
                t => transform.forward = Vector3.Slerp(originalForward, targetForward, t));
            if (!useRootRotation) yield return _animator.WaitForAnimationCoroutine();
            
            Facing = target;
        }
        
        private void SetFacing(Vector2Int value)
        {
            if (value.x > 1) value.x = 1;
            if (value.y > 1) value.y = 1;
                
            if (value is { x: 0, y: 0 } or { x: 1, y: 1 })
            {
                Debug.LogWarning($"Invalid facing for robot. Correcting to {Vector2Int.right}.");
                value = Vector2Int.right;
            }

            _facing = value;
            transform.forward = new(_facing.x, 0, _facing.y);
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

        private static IEnumerator SmoothCoroutine(float duration, Action<float> action)
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