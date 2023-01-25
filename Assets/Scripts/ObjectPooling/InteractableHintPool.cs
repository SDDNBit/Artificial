using SoftBit.Mechanics;
using SoftBit.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftBit.ObjectPooling
{
    public class InteractableHintPool : Singleton<InteractableHintPool>
    {
        Queue<InteractableHint> interactableHints = new();

        protected override void Awake()
        {
            base.Awake();
            foreach (Transform child in transform)
            {
                interactableHints.Enqueue(child.GetComponent<InteractableHint>());
                child.gameObject.SetActive(false);
            }
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
