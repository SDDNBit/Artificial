using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using Constants = SoftBit.Utils.Constants;

namespace SoftBit.Mechanics
{
    public class InteractableHint : MonoBehaviour
    {
        private readonly Vector3 MinScale = new Vector3(0.05f, 0.05f, 1f);
        private readonly Vector3 MaxScale = new Vector3(0.10f, 0.10f, 1f);
        private const float AnimationDuration = 0.3f;

        private ShareTheSameLocation shareTheSameLocation;
        private Transform selfTransform;
        private TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore;

        private void Awake()
        {
            shareTheSameLocation = GetComponent<ShareTheSameLocation>();
            selfTransform = transform;
        }

        public void SetState(bool isActive, Transform trackable = null)
        {
            if (isActive)
            {
                shareTheSameLocation.Target = trackable;
                gameObject.SetActive(true);
                selfTransform.localScale = MinScale;
                tweenerCore?.Kill();
                tweenerCore = selfTransform.DOScale(MaxScale, AnimationDuration).SetEase(Ease.InSine);
            }
            else
            {
                tweenerCore?.Kill();
                tweenerCore = selfTransform.DOScale(MinScale, AnimationDuration).SetEase(Ease.InSine).OnComplete(HideInteractable);
            }

            void HideInteractable()
            {
                shareTheSameLocation.Target = trackable;
                shareTheSameLocation.ForceUpdatePosition(Constants.DefaultVisibleLocation);
                gameObject.SetActive(false);
            }
        }
    }
}