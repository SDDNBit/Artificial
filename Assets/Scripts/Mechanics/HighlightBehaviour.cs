using Autohand;
using SoftBit.ObjectPooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftBit.Mechanics
{
    public class HighlightBehaviour : MonoBehaviour
    {
        public DistanceGrabbable DistanceGrabbable;
        public AttractableObject AttractableObject;

        private InteractableHintPool interactableHintPool;
        private InteractableHint interactableHint;

        private void Awake()
        {
            TryGetComponent(out DistanceGrabbable);
            TryGetComponent(out AttractableObject);
        }

        private void Start()
        {
            interactableHintPool = InteractableHintPool.Instance;
        }

        private void OnEnable()
        {
            if (DistanceGrabbable != null)
            {
                DistanceGrabbable.StartTargeting.AddListener(OnStartTargeting);
                DistanceGrabbable.StopTargeting.AddListener(OnStopTargeting);
            }
            if(AttractableObject != null)
            {
                AttractableObject.ObjectTargeted.AddListener(OnObjectTargeted);
            }
        }

        private void OnDisable()
        {
            if (DistanceGrabbable != null)
            {
                DistanceGrabbable.StartTargeting.RemoveListener(OnStartTargeting);
                DistanceGrabbable.StopTargeting.RemoveListener(OnStopTargeting);
            }
            if (AttractableObject != null)
            {
                AttractableObject.ObjectTargeted.RemoveListener(OnObjectTargeted);
            }
        }

        private void OnObjectTargeted(Grabbable grabbable, bool isTargeted)
        {
            if (isTargeted)
            {
                ShowHint();
            }
            else
            {
                HideHint();
            }
        }

        private void ShowHint()
        {
            if (interactableHint == null)
            {
                interactableHint = interactableHintPool.UseInteractableHint(transform);
            }
        }

        private void HideHint()
        {
            if (interactableHint)
            {
                interactableHintPool.DiscardInteractableHint(interactableHint);
                interactableHint = null;
            }
        }

        private void OnStartTargeting(Hand hand, Grabbable grabbable)
        {
            ShowHint();
        }

        private void OnStopTargeting(Hand hand, Grabbable grabbable)
        {
            HideHint();
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