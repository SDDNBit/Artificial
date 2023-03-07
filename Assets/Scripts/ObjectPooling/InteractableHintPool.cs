using SoftBit.Mechanics;
using SoftBit.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftBit.ObjectPooling
{
    public class InteractableHintPool : Singleton<InteractableHintPool>
    {
        [SerializeField] private InteractableHint interactableHintPrefab;
        private Queue<InteractableHint> interactableHints = new Queue<InteractableHint>();


        protected override void Awake()
        {
            base.Awake();
            for (var i = 0; i < Constants.InteractableHintPoolCount; ++i)
            {
                var instantiatedInteractableHint = Instantiate(interactableHintPrefab, transform);
                interactableHints.Enqueue(instantiatedInteractableHint);
                instantiatedInteractableHint.gameObject.SetActive(false);
            }
            //foreach (Transform child in transform)
            //{
            //    interactableHints.Enqueue(child.GetComponent<InteractableHint>());
            //    child.gameObject.SetActive(false);
            //}
        }

        public InteractableHint UseInteractableHint(Transform target)
        {
            if (interactableHints.Count > 0)
            {
                var interactableHint = interactableHints.Dequeue();
                interactableHint.SetState(true, target);
                return interactableHint;
            }
            return null;
        }

        public void DiscardInteractableHint(InteractableHint interactableHint)
        {
            interactableHint.SetState(false);
            interactableHints.Enqueue(interactableHint);
        }
    }
}
