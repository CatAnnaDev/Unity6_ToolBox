using System;

namespace CatAnnaDev.StateMachine
{
    public abstract class State : IState
    {
        public string Name { get; }

        protected State()
        {
            Name = GetType().Name;
        }

        protected State(string name)
        {
            Name = string.IsNullOrEmpty(name) ? GetType().Name : name;
        }

        public virtual void Enter() { }
        public virtual void Tick() { }
        public virtual void FixedTick() { }
        public virtual void Exit() { }

        public override string ToString()
        {
            return Name;
        }
    }

    public sealed class DelegateState : IState
    {
        private readonly Action _onEnter;
        private readonly Action _onTick;
        private readonly Action _onFixedTick;
        private readonly Action _onExit;

        public string Name { get; }

        public DelegateState(string name, Action onEnter = null, Action onTick = null, Action onFixedTick = null, Action onExit = null)
        {
            Name = string.IsNullOrEmpty(name) ? "State" : name;
            _onEnter = onEnter;
            _onTick = onTick;
            _onFixedTick = onFixedTick;
            _onExit = onExit;
        }

        public void Enter()
        {
            _onEnter?.Invoke();
        }

        public void Tick()
        {
            _onTick?.Invoke();
        }

        public void FixedTick()
        {
            _onFixedTick?.Invoke();
        }

        public void Exit()
        {
            _onExit?.Invoke();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
