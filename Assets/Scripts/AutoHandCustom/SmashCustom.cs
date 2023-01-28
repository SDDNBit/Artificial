using Autohand;
using NaughtyAttributes;
using SoftBit.Mechanics;
using UnityEngine;
using UnityEngine.Events;

namespace SoftBit.Autohand.Custom
{
    public class SmashCustom : MonoBehaviour
    {
        [Header("Smash Options")]
        [Tooltip("Required velocity magnitude from Smasher to smash")]
        public float smashForce = 1;

        private void Awake()
        {
            enabled = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            SmasherCustom smasher;
            EnemyCollider enemyCollider;
            if (collision.transform.CanGetComponent(out smasher))
            {
                if (smasher.LastCollisionGameObject != null && smasher.LastCollisionGameObject.CanGetComponent(out enemyCollider))
                {
                    if (!enemyCollider.IsDestroyed && smasher.GetMagnitude() >= smashForce)
                    {
                        enemyCollider.DestroyPart(collision);
                    }
                }
            }
        }
    }
}
