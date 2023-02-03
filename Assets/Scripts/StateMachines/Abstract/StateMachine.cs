using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftBit.States.Abstract
{
    public abstract class StateMachine : MonoBehaviour
    {
        private State currentState;

        private void Update()
        {
            currentState?.Update();
        }

        protected void Init(State initialState, Dictionary<State, Dictionary<Transition, State>> states)
        {
            foreach(var state in states)
            {
                foreach(var transition in state.Value)
                {
                    transition.Key.Callback = () => SetState(transition.Value);
                    state.Key.AddTransition(transition.Key);
                }
            }

            SetState(initialState);
        }

        private void SetState(State state)
        {
            currentState?.Exit();
            currentState = state;
            currentState.Enter();
        }
    }
}