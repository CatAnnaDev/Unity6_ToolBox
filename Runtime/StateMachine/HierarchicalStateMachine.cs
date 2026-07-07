using System;

namespace CatAnnaDev.StateMachine
{
    public class HierarchicalStateMachine : IState
    {
        private readonly Action _onEnter;
        private readonly Action _onExit;

        public string Name { get; }
        public StateMachine Machine { get; }

        public IState InitialState { get; set; }

        public HierarchicalStateMachine(string name = null, StateMachine machine = null, Action onEnter = null, Action onExit = null)
        {
            Name = string.IsNullOrEmpty(name) ? GetType().Name : name;
            Machine = machine ?? new StateMachine();
            _onEnter = onEnter;
            _onExit = onExit;
        }

        public IState ActiveChild
        {
            get { return Machine.CurrentState; }
        }

        public virtual void Enter()
        {
            _onEnter?.Invoke();
            if (InitialState != null && Machine.CurrentState == null)
            {
                Machine.SetState(InitialState);
            }
        }

        public virtual void Tick()
        {
            Machine.Tick();
        }

        public virtual void FixedTick()
        {
            Machine.FixedTick();
        }

        public virtual void Exit()
        {
            Machine.Reset();
            _onExit?.Invoke();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
