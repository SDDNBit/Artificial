using Autohand;
using SoftBit.ObjectPooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftBit.Mechanics
{
    public class HighlightBehaviour : MonoBehaviour
    {
        [SerializeField] private DistanceGrabbable distanceGrabbable;

        private InteractableHintPool interactableHintPool;
        private InteractableHint interactableHint;

        private void Start()
        {
            interactableHintPool = InteractableHintPool.Instance;
        }

        private void OnEnable()
        {
            distanceGrabbable.StartTargeting.AddListener(OnStartTargeting);
            distanceGrabbable.StopTargeting.AddListener(OnStopTargeting);
        }

        private void OnDisable()
        {
            distanceGrabbable.StartTargeting.RemoveListener(OnStartTargeting);
            distanceGrabbable.StopTargeting.RemoveListener(OnStopTargeting);
        }

        private void OnStartTargeting(Hand hand, Grabbable grabbable)
        {
            if (interactableHint == null)
            {
                interactableHint = interactableHintPool.UseInteractableHint(transform);
            }
        }

        private void OnStopTargeting(Hand hand, Grabbable grabbable)
        {
            if (interactableHint)
            {
                interactableHintPool.DiscardInteractableHint(interactableHint);
                interactableHint = null;
            }
        }

        #region TestMethods
        [ContextMenu("StartTargeting")]
        private void StartTargeting()
        {
            OnStartTargeting(null, null);
        }

        [ContextMenu("StopTargeting")]
        private void StopTargeting()
        {
            OnStopTargeting(null, null);
        }
        #endregion
    }
}