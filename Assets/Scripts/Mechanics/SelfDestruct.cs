using System.Collections;
using UnityEngine;

namespace SoftBit.Mechanics
{
    public class SelfDestruct : MonoBehaviour
    {
        [SerializeField] private float SecondsUntilDestroy = 15f;

        private void Start()
        {
            Destroy(gameObject, SecondsUntilDestroy);
        }
    }
}
