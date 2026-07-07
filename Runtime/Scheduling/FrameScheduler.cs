using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using CatAnnaDev;

namespace CatAnnaDev.Scheduling
{
    internal enum ScheduleKind : byte
    {
        Frame = 0,
        Time = 1,
        Repeat = 2
    }

    internal enum ScheduleTimeMode : byte
    {
        Scaled = 0,
        Unscaled = 1,
        Realtime = 2
    }

    internal sealed class ScheduledTask
    {
        internal int version;
        internal bool active;
        internal ScheduleKind kind;
        internal ScheduleTimeMode timeMode;
        internal Action action;
        internal Func<bool> predicate;
        internal double dueTime;
        internal double interval;
        internal int targetFrame;
        internal int remainingRepeats;

        internal void ResetForReuse()
        {
            action = null;
            predicate = null;
            active = false;
            dueTime = 0d;
            interval = 0d;
            targetFrame = 0;
            remainingRepeats = 0;
            kind = ScheduleKind.Frame;
            timeMode = ScheduleTimeMode.Scaled;
        }
    }

    internal static class PlayerLoopBinder
    {
        internal static bool Insert(Type parentPhase, Type marker, PlayerLoopSystem.UpdateFunction fn)
        {
            var root = PlayerLoop.GetCurrentPlayerLoop();
            if (!InsertInto(ref root, parentPhase, marker, fn))
            {
                return false;
            }
            PlayerLoop.SetPlayerLoop(root);
            return true;
        }

        static bool InsertInto(ref PlayerLoopSystem system, Type parentPhase, Type marker, PlayerLoopSystem.UpdateFunction fn)
        {
            var children = system.subSystemList;
            if (children == null)
            {
                return false;
            }

            for (int i = 0; i < children.Length; i++)
            {
                if (children[i].type == parentPhase)
                {
                    var parent = children[i];
                    var existing = parent.subSystemList;
                    int startCount = existing == null ? 0 : existing.Length;

                    for (int j = 0; j < startCount; j++)
                    {
                        if (existing[j].type == marker)
                        {
                            return true;
                        }
                    }

                    var expanded = new PlayerLoopSystem[startCount + 1];
                    for (int j = 0; j < startCount; j++)
                    {
                        expanded[j] = existing[j];
                    }
                    expanded[startCount] = new PlayerLoopSystem
                    {
                        type = marker,
                        updateDelegate = fn
                    };
                    parent.subSystemList = expanded;
                    children[i] = parent;
                    return true;
                }
            }

            for (int i = 0; i < children.Length; i++)
            {
                var child = children[i];
                if (InsertInto(ref child, parentPhase, marker, fn))
                {
                    children[i] = child;
                    return true;
                }
            }

            return false;
        }
    }

    public static class FrameScheduler
    {
        struct FrameSchedulerUpdate { }

        static readonly List<ScheduledTask> Tasks = new List<ScheduledTask>(64);
        static readonly Stack<ScheduledTask> Pool = new Stack<ScheduledTask>(64);
        static bool _installed;

        public static int ActiveCount
        {
            get
            {
                int n = 0;
                for (int i = 0; i < Tasks.Count; i++)
                {
                    if (Tasks[i].active)
                    {
                        n++;
                    }
                }
                return n;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetState()
        {
            Tasks.Clear();
            Pool.Clear();
            _installed = false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Install()
        {
            if (_installed)
            {
                return;
            }
            _installed = PlayerLoopBinder.Insert(typeof(Update), typeof(FrameSchedulerUpdate), Tick);
            if (!_installed)
            {
                CatLog.Warn("FrameScheduler failed to install into the PlayerLoop.");
            }
        }

        public static ScheduledHandle NextFrame(Action action)
        {
            return ScheduleFrame(1, action);
        }

        public static ScheduledHandle AfterFrames(int frames, Action action)
        {
            return ScheduleFrame(frames < 0 ? 0 : frames, action);
        }

        public static ScheduledHandle After(float seconds, Action action)
        {
            return After(seconds, action, false);
        }

        public static ScheduledHandle After(float seconds, Action action, bool unscaled)
        {
            return ScheduleTime(seconds, action, unscaled ? ScheduleTimeMode.Unscaled : ScheduleTimeMode.Scaled);
        }

        public static ScheduledHandle AfterRealtime(float seconds, Action action)
        {
            return ScheduleTime(seconds, action, ScheduleTimeMode.Realtime);
        }

        public static ScheduledHandle Every(float interval, Action action)
        {
            return Every(interval, action, -1, null, false);
        }

        public static ScheduledHandle Every(float interval, Action action, int count)
        {
            return Every(interval, action, count, null, false);
        }

        public static ScheduledHandle Every(float interval, Action action, Func<bool> predicate)
        {
            return Every(interval, action, -1, predicate, false);
        }

        public static ScheduledHandle Every(float interval, Action action, int count, Func<bool> predicate, bool unscaled)
        {
            if (action == null)
            {
                return ScheduledHandle.None;
            }

            var task = Rent();
            task.kind = ScheduleKind.Repeat;
            task.timeMode = unscaled ? ScheduleTimeMode.Unscaled : ScheduleTimeMode.Scaled;
            task.action = action;
            task.predicate = predicate;
            task.interval = interval < 0f ? 0d : interval;
            task.remainingRepeats = count;
            task.dueTime = CurrentTime(task.timeMode) + task.interval;
            task.active = true;
            Tasks.Add(task);
            return new ScheduledHandle(task, task.version);
        }

        public static void CancelAll()
        {
            for (int i = 0; i < Tasks.Count; i++)
            {
                Tasks[i].active = false;
            }
        }

        static ScheduledHandle ScheduleFrame(int frames, Action action)
        {
            if (action == null)
            {
                return ScheduledHandle.None;
            }

            var task = Rent();
            task.kind = ScheduleKind.Frame;
            task.action = action;
            task.targetFrame = Time.frameCount + frames;
            task.active = true;
            Tasks.Add(task);
            return new ScheduledHandle(task, task.version);
        }

        static ScheduledHandle ScheduleTime(float seconds, Action action, ScheduleTimeMode mode)
        {
            if (action == null)
            {
                return ScheduledHandle.None;
            }

            var task = Rent();
            task.kind = ScheduleKind.Time;
            task.timeMode = mode;
            task.action = action;
            task.dueTime = CurrentTime(mode) + (seconds < 0f ? 0d : seconds);
            task.active = true;
            Tasks.Add(task);
            return new ScheduledHandle(task, task.version);
        }

        static void Tick()
        {
            int frame = Time.frameCount;
            double scaled = Time.timeAsDouble;
            double unscaled = Time.unscaledTimeAsDouble;
            double real = Time.realtimeSinceStartupAsDouble;

            int count = Tasks.Count;
            for (int i = 0; i < count; i++)
            {
                var task = Tasks[i];
                if (!task.active)
                {
                    continue;
                }

                switch (task.kind)
                {
                    case ScheduleKind.Frame:
                        if (frame >= task.targetFrame)
                        {
                            Invoke(task.action);
                            task.active = false;
                        }
                        break;

                    case ScheduleKind.Time:
                        if (ModeTime(task.timeMode, scaled, unscaled, real) >= task.dueTime)
                        {
                            Invoke(task.action);
                            task.active = false;
                        }
                        break;

                    case ScheduleKind.Repeat:
                        double now = ModeTime(task.timeMode, scaled, unscaled, real);
                        if (now >= task.dueTime)
                        {
                            if (task.predicate != null && !SafePredicate(task.predicate))
                            {
                                task.active = false;
                                break;
                            }

                            Invoke(task.action);

                            if (task.remainingRepeats > 0)
                            {
                                task.remainingRepeats--;
                                if (task.remainingRepeats == 0)
                                {
                                    task.active = false;
                                    break;
                                }
                            }

                            task.dueTime += task.interval;
                        }
                        break;
                }
            }

            Compact();
        }

        static void Compact()
        {
            int write = 0;
            int total = Tasks.Count;
            for (int i = 0; i < total; i++)
            {
                var task = Tasks[i];
                if (task.active)
                {
                    Tasks[write++] = task;
                }
                else
                {
                    Release(task);
                }
            }

            if (write < total)
            {
                Tasks.RemoveRange(write, total - write);
            }
        }

        static double CurrentTime(ScheduleTimeMode mode)
        {
            return ModeTime(mode, Time.timeAsDouble, Time.unscaledTimeAsDouble, Time.realtimeSinceStartupAsDouble);
        }

        static double ModeTime(ScheduleTimeMode mode, double scaled, double unscaled, double real)
        {
            switch (mode)
            {
                case ScheduleTimeMode.Unscaled: return unscaled;
                case ScheduleTimeMode.Realtime: return real;
                default: return scaled;
            }
        }

        static void Invoke(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                CatLog.Error(e);
            }
        }

        static bool SafePredicate(Func<bool> predicate)
        {
            try
            {
                return predicate();
            }
            catch (Exception e)
            {
                CatLog.Error(e);
                return false;
            }
        }

        static ScheduledTask Rent()
        {
            var task = Pool.Count > 0 ? Pool.Pop() : new ScheduledTask();
            task.ResetForReuse();
            return task;
        }

        static void Release(ScheduledTask task)
        {
            task.version++;
            task.ResetForReuse();
            Pool.Push(task);
        }
    }
}
