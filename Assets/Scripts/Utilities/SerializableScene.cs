using System;
using UnityEngine;

namespace SSPot.Utilities
{
    [Serializable]
    public struct SerializableScene
    {
        [SerializeField] private string guid;

        [SerializeField] private int buildIndex;
        
        [SerializeField] private string name;

        public int BuildIndex => buildIndex;

        public string Name => name;
    }
}