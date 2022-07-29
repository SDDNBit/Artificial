using UnityEngine;

namespace SoftBit.Utils
{
    public class DestroyIfNotInUse : MonoBehaviour
    {
        [HideInInspector] public bool InUse;

        [SerializeField] private float SecondsUntilDestroy = 15f;

        private float lastTimeInUse;

        private void Start()
        {
            InUse = false;
            lastTimeInUse = Time.time;
        }

        private void Update()
        {
            if (!InUse && (Time.time > lastTimeInUse + SecondsUntilDestroy))
            {
                Destroy(gameObject);
            }
            else
            {
                if (InUse)
                {
                    lastTimeInUse = Time.time;
                }
            }
        }
    }
}
