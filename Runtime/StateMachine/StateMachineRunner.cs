using UnityEngine;

namespace CatAnnaDev.StateMachine
{
    [AddComponentMenu("CatAnnaDev/StateMachine/State Machine Runner")]
    public class StateMachineRunner : MonoBehaviour
    {
        [SerializeField] private bool _tickInUpdate = true;
        [SerializeField] private bool _tickInFixedUpdate = true;
        [SerializeField] private bool _autoBuildOnAwake = true;

        public StateMachine Machine { get; private set; }

        public bool IsRunning
        {
            get { return Machine != null && Machine.CurrentState != null; }
        }

        protected virtual void Awake()
        {
            if (_autoBuildOnAwake && Machine == null)
            {
                Machine = Build();
            }
        }

        public void SetMachine(StateMachine machine)
        {
            Machine = machine;
        }

        protected virtual StateMachine Build()
        {
            return new StateMachine();
        }

        protected virtual void Update()
        {
            if (_tickInUpdate)
            {
                Machine?.Tick();
            }
        }

        protected virtual void FixedUpdate()
        {
            if (_tickInFixedUpdate)
            {
                Machine?.FixedTick();
            }
        }
    }
}
