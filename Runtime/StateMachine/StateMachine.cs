using System;
using System.Collections.Generic;

namespace CatAnnaDev.StateMachine
{
    public class StateMachine
    {
        private sealed class StateNode
        {
            public readonly IState State;
            public readonly List<ITransition> Transitions;

            public StateNode(IState state)
            {
                State = state;
                Transitions = new List<ITransition>();
            }
        }

        private readonly Dictionary<IState, StateNode> _nodes = new Dictionary<IState, StateNode>();
        private readonly List<ITransition> _anyTransitions = new List<ITransition>();

        private StateNode _current;

        public IState CurrentState
        {
            get { return _current?.State; }
        }

        public event Action<IState, IState> OnStateChanged;

        public void AddTransition(IState from, IState to, IPredicate condition)
        {
            GetOrCreateNode(to);
            GetOrCreateNode(from).Transitions.Add(new Transition(to, condition));
        }

        public void AddTransition(IState from, IState to, Func<bool> condition)
        {
            AddTransition(from, to, new FuncPredicate(condition));
        }

        public void AddAnyTransition(IState to, IPredicate condition)
        {
            GetOrCreateNode(to);
            _anyTransitions.Add(new Transition(to, condition));
        }

        public void AddAnyTransition(IState to, Func<bool> condition)
        {
            AddAnyTransition(to, new FuncPredicate(condition));
        }

        public void SetState(IState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            StateNode node = GetOrCreateNode(state);
            IState previous = _current?.State;
            _current = node;
            node.State.Enter();
            OnStateChanged?.Invoke(previous, node.State);
        }

        public void ChangeState(IState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            if (_current != null && ReferenceEquals(_current.State, state))
            {
                return;
            }

            StateNode node = GetOrCreateNode(state);
            IState previous = _current?.State;
            _current?.State.Exit();
            _current = node;
            node.State.Enter();
            OnStateChanged?.Invoke(previous, node.State);
        }

        public void Tick()
        {
            if (_current == null)
            {
                return;
            }

            ITransition transition = GetTransition();
            if (transition != null && !ReferenceEquals(transition.To, _current.State))
            {
                ChangeState(transition.To);
            }

            _current.State.Tick();
        }

        public void FixedTick()
        {
            _current?.State.FixedTick();
        }

        public bool Contains(IState state)
        {
            return state != null && _nodes.ContainsKey(state);
        }

        public void Reset()
        {
            _current = null;
        }

        private ITransition GetTransition()
        {
            for (int i = 0; i < _anyTransitions.Count; i++)
            {
                ITransition any = _anyTransitions[i];
                if (any.Condition.Evaluate())
                {
                    return any;
                }
            }

            List<ITransition> transitions = _current.Transitions;
            for (int i = 0; i < transitions.Count; i++)
            {
                ITransition transition = transitions[i];
                if (transition.Condition.Evaluate())
                {
                    return transition;
                }
            }

            return null;
        }

        private StateNode GetOrCreateNode(IState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            if (!_nodes.TryGetValue(state, out StateNode node))
            {
                node = new StateNode(state);
                _nodes.Add(state, node);
            }

            return node;
        }
    }
}
