using UnityEngine;

namespace SSpot
{
    public interface IGameObjectProvider
    {
        // ReSharper disable once InconsistentNaming
        GameObject gameObject { get; }
    }
}