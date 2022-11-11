using SoftBit.Utils;
using UnityEngine;

namespace SoftBit.AnimatorBehaviours
{
    public class IdleBored : StateMachineBehaviour
    {
        private const float NormalizeMargin = 0.98f;
        private const float DamperTime = 0.5f;

        [SerializeField] private float timeUntilGetBored;
        [SerializeField] private int numberOfBoredAnimations;

        private bool isBored;
        private float idleTimePassed;
        private int boredAnimationIndex;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            PlayIdleAnimation();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!isBored)
            {
                idleTimePassed += Time.deltaTime;

                if (idleTimePassed > timeUntilGetBored && stateInfo.normalizedTime % 1 < 0.02f)
                {
                    isBored = true;
                    boredAnimationIndex = Random.Range(1, numberOfBoredAnimations + 1);
                    boredAnimationIndex = boredAnimationIndex * 2 - 1;

                    animator.SetFloat(Constants.AnimatorIdleState, boredAnimationIndex - 1);
                }
            }
            else if (stateInfo.normalizedTime % 1 > NormalizeMargin)
            {
                PlayIdleAnimation();
            }
            animator.SetFloat(Constants.AnimatorIdleState, boredAnimationIndex, DamperTime, Time.deltaTime);
        }

        private void PlayIdleAnimation()
        {
            if (isBored)
            {
                boredAnimationIndex--;
            }
            isBored = false;
            idleTimePassed = 0;
        }
    }
}
