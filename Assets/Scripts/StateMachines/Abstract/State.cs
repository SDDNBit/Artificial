using System.Collections.Generic;

namespace SoftBit.States.Abstract
{
    public abstract class State
    {
        private List<Transition> transitions = new List<Transition>();

        public void AddTransition(Transition transition)
        {
            transitions.Add(transition);
        }

        public virtual void Enter()
        {
            foreach(var transition in transitions)
            {
                transition.Enter();
            }
        }

        public virtual void Exit()
        {
            foreach (var transition in transitions)
            {
                transition.Exit();
            }
        }

        public virtual void Update()
        {
            foreach (var transition in transitions)
            {
                transition.Update();
            }
        }
    }
}