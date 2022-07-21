using UnityEngine;

namespace SoftBit.Mechanics
{
    [RequireComponent(typeof(FlyToObject))]
    public class AttractableObject : MonoBehaviour
    {
        [HideInInspector] public bool IsAlreadyOrbiting = false;
        [HideInInspector] public FlyToObject FlyToObjectComponent;

        private void Awake()
        {
            FlyToObjectComponent = GetComponent<FlyToObject>();
        }
    }
}
