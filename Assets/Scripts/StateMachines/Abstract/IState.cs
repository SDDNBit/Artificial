namespace SoftBit.States.Abstract
{
    public interface IState
    {
        public void Enter(IStateMachine stateMachine);
        public void Update();
        public void Exit();
    }
}