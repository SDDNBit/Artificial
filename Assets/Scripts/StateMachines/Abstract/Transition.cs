using System;

namespace SoftBit.States.Abstract
{

    /// <summary>
    /// State machine inspired and made after Razdolbays tutorial, git repo: https://github.com/MaxMerkuryev/ASMR/blob/main/ASM%20Project/Assets/Scripts/StateMachineBase/Transition.cs
    /// </summary>
    public abstract class Transition
    {
        public Action Callback;

        public abstract bool CheckCondition();
        public virtual void Enter() { }
        public virtual void Exit() { }

        public void Update()
        {
            if (!CheckCondition())
            {
                return;
            }

            if(Callback != null)
            {
                Callback.Invoke();
            }
            else
            {
                Enter();
            }
        }
    }
}
