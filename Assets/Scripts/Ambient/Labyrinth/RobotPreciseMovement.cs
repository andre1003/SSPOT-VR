using System.Collections;
using UnityEngine;

namespace SSPot.Ambient.Labyrinth
{
    public class RobotPreciseMovement : MonoBehaviour
    {
        [SerializeField] private Transform robotTransform;
        [SerializeField] private float movementTime = 2f;
        [SerializeField] private float rotationTime = 1f;
        public float MovementTime { get { return movementTime; } }

        private Coroutine moveCoroutine;
        private Coroutine rotateCoroutine;

        IEnumerator MoveTo(Vector3 target, float delay)
        {
            float time = 0f;
            Vector3 initialPosition = robotTransform.position;
            while(time < delay)
            {
                float t = time / delay;
                robotTransform.position = Vector3.Lerp(initialPosition, target, t);
                time += Time.deltaTime;
                yield return null;
            }
            robotTransform.position = target;
        }

        IEnumerator RotateTo(float angle, float delay)
        {
            float time = 0f;
            Quaternion initialRotation = robotTransform.rotation;
            Quaternion finalRotation = Quaternion.Euler(robotTransform.eulerAngles + Vector3.up * angle);
            while(time < delay)
            {
                float t = time / delay;
                robotTransform.rotation = Quaternion.Slerp(initialRotation, finalRotation, t);
                time += Time.deltaTime;
                yield return null;
            }
            robotTransform.rotation = finalRotation;
        }

        private void MoveForward()
        {
            Vector3 target = robotTransform.position + (robotTransform.forward * 3f);
            moveCoroutine = StartCoroutine(MoveTo(target, movementTime));
        }

        private void TurnRight()
        {
            rotateCoroutine = StartCoroutine(RotateTo(90f, rotationTime));
        }

        private void TurnLeft()
        {
            rotateCoroutine = StartCoroutine(RotateTo(-90f, rotationTime));
        }

        public void Move(string movement)
        {
            if(movement == "Forward") MoveForward();
            else if(movement == "Right") TurnRight();
            else TurnLeft();
        }

        public void StopMovement()
        {
            if(moveCoroutine != null) StopCoroutine(moveCoroutine);
            if(rotateCoroutine != null) StopCoroutine(rotateCoroutine);
        }
    }
}
