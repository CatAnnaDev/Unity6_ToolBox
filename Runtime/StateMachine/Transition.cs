using System;

namespace CatAnnaDev.StateMachine
{
    public sealed class Transition : ITransition
    {
        public IState To { get; }
        public IPredicate Condition { get; }

        public Transition(IState to, IPredicate condition)
        {
            To = to ?? throw new ArgumentNullException(nameof(to));
            Condition = condition ?? throw new ArgumentNullException(nameof(condition));
        }

        public Transition(IState to, Func<bool> condition)
            : this(to, new FuncPredicate(condition))
        {
        }
    }
}
