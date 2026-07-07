using System;
using System.Collections.Generic;

namespace CatAnnaDev.StateMachine
{
    public sealed class StateMachineBuilder
    {
        internal sealed class PendingTransition
        {
            public string FromName;
            public string ToName;
            public IPredicate Condition;
        }

        internal sealed class StateDefinition
        {
            public string Name;
            public Action OnEnter;
            public Action OnTick;
            public Action OnFixedTick;
            public Action OnExit;
        }

        private readonly Dictionary<string, StateDefinition> _states = new Dictionary<string, StateDefinition>(StringComparer.Ordinal);
        private readonly List<PendingTransition> _transitions = new List<PendingTransition>();
        private readonly List<PendingTransition> _anyTransitions = new List<PendingTransition>();

        private string _initialName;

        public StateBuilder State(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("State name cannot be null or empty.", nameof(name));
            }

            if (!_states.TryGetValue(name, out StateDefinition definition))
            {
                definition = new StateDefinition { Name = name };
                _states.Add(name, definition);
                if (_initialName == null)
                {
                    _initialName = name;
                }
            }

            return new StateBuilder(this, definition);
        }

        public StateMachineBuilder Initial(string name)
        {
            _initialName = name;
            return this;
        }

        public TransitionBuilder AnyTransitionTo(string toName)
        {
            EnsureState(toName);
            PendingTransition pending = new PendingTransition { FromName = null, ToName = toName };
            _anyTransitions.Add(pending);
            return new TransitionBuilder(this, pending, null);
        }

        public StateMachine Build(bool enterInitial = true)
        {
            StateMachine machine = new StateMachine();
            Dictionary<string, IState> resolved = new Dictionary<string, IState>(StringComparer.Ordinal);

            foreach (KeyValuePair<string, StateDefinition> entry in _states)
            {
                StateDefinition d = entry.Value;
                resolved[d.Name] = new DelegateState(d.Name, d.OnEnter, d.OnTick, d.OnFixedTick, d.OnExit);
            }

            for (int i = 0; i < _transitions.Count; i++)
            {
                PendingTransition t = _transitions[i];
                machine.AddTransition(Resolve(resolved, t.FromName), Resolve(resolved, t.ToName), RequireCondition(t));
            }

            for (int i = 0; i < _anyTransitions.Count; i++)
            {
                PendingTransition t = _anyTransitions[i];
                machine.AddAnyTransition(Resolve(resolved, t.ToName), RequireCondition(t));
            }

            if (enterInitial && _initialName != null && resolved.TryGetValue(_initialName, out IState initial))
            {
                machine.SetState(initial);
            }

            return machine;
        }

        private static IPredicate RequireCondition(PendingTransition t)
        {
            if (t.Condition == null)
            {
                throw new InvalidOperationException("Transition to '" + t.ToName + "' is missing a When(...) condition.");
            }

            return t.Condition;
        }

        private static IState Resolve(Dictionary<string, IState> resolved, string name)
        {
            if (name != null && resolved.TryGetValue(name, out IState state))
            {
                return state;
            }

            throw new InvalidOperationException("Unknown state referenced by builder: '" + name + "'.");
        }

        private void EnsureState(string name)
        {
            if (!_states.ContainsKey(name))
            {
                _states.Add(name, new StateDefinition { Name = name });
                if (_initialName == null)
                {
                    _initialName = name;
                }
            }
        }

        public sealed class StateBuilder
        {
            private readonly StateMachineBuilder _owner;
            private readonly StateDefinition _definition;

            internal StateBuilder(StateMachineBuilder owner, StateDefinition definition)
            {
                _owner = owner;
                _definition = definition;
            }

            public StateBuilder OnEnter(Action action)
            {
                _definition.OnEnter += action;
                return this;
            }

            public StateBuilder OnTick(Action action)
            {
                _definition.OnTick += action;
                return this;
            }

            public StateBuilder OnFixedTick(Action action)
            {
                _definition.OnFixedTick += action;
                return this;
            }

            public StateBuilder OnExit(Action action)
            {
                _definition.OnExit += action;
                return this;
            }

            public StateBuilder AsInitial()
            {
                _owner._initialName = _definition.Name;
                return this;
            }

            public StateBuilder State(string name)
            {
                return _owner.State(name);
            }

            public TransitionBuilder TransitionTo(string toName)
            {
                _owner.EnsureState(toName);
                PendingTransition pending = new PendingTransition { FromName = _definition.Name, ToName = toName };
                _owner._transitions.Add(pending);
                return new TransitionBuilder(_owner, pending, this);
            }

            public StateMachine Build(bool enterInitial = true)
            {
                return _owner.Build(enterInitial);
            }
        }

        public sealed class TransitionBuilder
        {
            private readonly StateMachineBuilder _owner;
            private readonly PendingTransition _pending;
            private readonly StateBuilder _source;

            internal TransitionBuilder(StateMachineBuilder owner, PendingTransition pending, StateBuilder source)
            {
                _owner = owner;
                _pending = pending;
                _source = source;
            }

            public StateBuilder When(Func<bool> condition)
            {
                return When(new FuncPredicate(condition));
            }

            public StateBuilder When(IPredicate condition)
            {
                _pending.Condition = condition ?? throw new ArgumentNullException(nameof(condition));
                return _source ?? _owner.State(_pending.ToName);
            }

            public StateBuilder After(float seconds)
            {
                return When(new TimerPredicate(seconds));
            }
        }
    }
}
