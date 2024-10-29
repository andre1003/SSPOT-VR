using System;
using System.Collections;
using System.Collections.Generic;
using SSpot.Robot;
using UnityEngine;

namespace SSpot.ComputerCode
{
    [Serializable]
    public class CubeRunner
    {
        [SerializeField] private float timeBetweenCubes = .5f;

        public int CurrentIndex { get; private set; }

        public IEnumerator RunCubesCoroutine(List<Cube> cubes, RobotData robot, Action endCallback)
        {
            while (CurrentIndex < cubes.Count - 1)
            {
                var cube = cubes[CurrentIndex];
                yield return cube.ExecuteCoroutine(robot);
                yield return new WaitForSeconds(timeBetweenCubes);
                CurrentIndex++;
            }
            
            Reset();
            endCallback?.Invoke();
        }

        public void Reset()
        {
            CurrentIndex = 0;
        }
    }
}