using System;
using System.Collections.Generic;
using SSPot.Utilities;
using UnityEngine;

namespace SSPot.Menu
{
    public class CanvasStack : MonoBehaviour
    {
        [SerializeField] private GameObject[] canvases = Array.Empty<GameObject>();
        [SerializeField] private GameObject startingCanvas;

        private readonly Stack<GameObject> _stack = new();
        
        private void Awake()
        {
            canvases.ForEach(c => c.SetActive(false));
            Push(startingCanvas);
        }

        public void Push(GameObject canvas)
        {
            if (_stack.Count > 0) 
                _stack.Peek().SetActive(false);

            _stack.Push(canvas);
            canvas.SetActive(true);
        }

        public void Pop()
        {
            if (_stack.Count > 0) 
                _stack.Pop().SetActive(false);
            
            if (_stack.Count > 0) 
                _stack.Peek().SetActive(true);
        }
    }
}