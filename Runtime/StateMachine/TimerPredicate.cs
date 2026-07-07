using UnityEngine;

namespace CatAnnaDev.StateMachine
{
    public sealed class TimerPredicate : IPredicate
    {
        private readonly float _duration;
        private readonly bool _autoReset;
        private readonly bool _unscaled;
        private float _startTime;

        public TimerPredicate(float duration, bool autoReset = true, bool unscaled = false)
        {
            _duration = duration < 0f ? 0f : duration;
            _autoReset = autoReset;
            _unscaled = unscaled;
            _startTime = Now;
        }

        private float Now
        {
            get { return _unscaled ? Time.unscaledTime : Time.time; }
        }

        public float Elapsed
        {
            get { return Now - _startTime; }
        }

        public float Remaining
        {
            get
            {
                float remaining = _duration - Elapsed;
                return remaining < 0f ? 0f : remaining;
            }
        }

        public void Reset()
        {
            _startTime = Now;
        }

        public bool Evaluate()
        {
            if (Elapsed >= _duration)
            {
                if (_autoReset)
                {
                    _startTime = Now;
                }
                return true;
            }
            return false;
        }
    }
}
