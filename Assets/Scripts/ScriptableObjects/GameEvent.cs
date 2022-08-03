using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Game event", fileName = "Create game event")]
    public class GameEvent : ScriptableObject
    {
        private HashSet<GameEventListener> listeners = new HashSet<GameEventListener>();

        public void Invoke()
        {
            foreach (var listener in listeners)
            {
                listener.RaiseEvent();
            }
        }

        public void Register(GameEventListener gameEventListener) => listeners.Add(gameEventListener);

        public void Deregister(GameEventListener gameEventListener) => listeners.Remove(gameEventListener);
    }
}
