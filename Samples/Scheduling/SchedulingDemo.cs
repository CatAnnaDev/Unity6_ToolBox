using System;
using System.Threading;
using UnityEngine;
using CatAnnaDev.Scheduling;

namespace CatAnnaDev.Samples
{
    public sealed class SchedulingDemo : MonoBehaviour
    {
        int _nextFrameFires;
        int _afterFires;
        int _everyFires;
        int _dispatcherFires;

        int _lastNextFrameNumber;
        float _lastAfterAt;
        float _lastEveryAt;

        ScheduledHandle _everyHandle;

        GUIStyle _boxStyle;
        GUIStyle _titleStyle;
        GUIStyle _bodyStyle;

        void OnDisable()
        {
            FrameScheduler.CancelAll();
            MainThreadDispatcher.Clear();
        }

        void Update()
        {
            if (DemoInput.GetKeyDown(KeyCode.Alpha1)) { ScheduleNextFrame(); }
            if (DemoInput.GetKeyDown(KeyCode.Alpha2)) { ScheduleAfter(); }
            if (DemoInput.GetKeyDown(KeyCode.Alpha3)) { StartEvery(); }
            if (DemoInput.GetKeyDown(KeyCode.Alpha4)) { CancelEvery(); }
            if (DemoInput.GetKeyDown(KeyCode.Alpha5)) { DispatchFromWorker(); }
        }

        void OnGUI()
        {
            EnsureStyles();

            var area = new Rect(12f, 12f, 560f, 470f);
            GUILayout.BeginArea(area, _boxStyle);

            GUILayout.Label("CatAnnaDev - FrameScheduler demo", _titleStyle);
            GUILayout.Space(4f);
            GUILayout.Label(
                "FrameScheduler runs plain Action callbacks off the Unity PlayerLoop:\n" +
                "one-shot next frame, one-shot after N seconds, and repeating on an\n" +
                "interval. Every() returns a ScheduledHandle you can Cancel(). The\n" +
                "MainThreadDispatcher hops work from a background thread back to Unity.",
                _bodyStyle);

            GUILayout.Space(6f);
            GUILayout.Label("Controls", _titleStyle);
            GUILayout.Label(
                "[1] / button  FrameScheduler.NextFrame  (fires once next frame)\n" +
                "[2] / button  FrameScheduler.After(1.5s) (fires once after delay)\n" +
                "[3] / button  FrameScheduler.Every(0.5s) (starts repeating task)\n" +
                "[4] / button  handle.Cancel()            (stops the repeating task)\n" +
                "[5] / button  MainThreadDispatcher       (queues from a worker thread)",
                _bodyStyle);

            GUILayout.Space(6f);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("1: NextFrame")) { ScheduleNextFrame(); }
            if (GUILayout.Button("2: After 1.5s")) { ScheduleAfter(); }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("3: Start Every 0.5s")) { StartEvery(); }
            if (GUILayout.Button("4: Cancel repeat")) { CancelEvery(); }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("5: Dispatch from worker thread")) { DispatchFromWorker(); }

            GUILayout.Space(8f);
            GUILayout.Label("Live status", _titleStyle);
            GUILayout.Label(
                "NextFrame fires  : " + _nextFrameFires + "  (last at frame " + _lastNextFrameNumber + ")\n" +
                "After fires      : " + _afterFires + "  (last at t=" + _lastAfterAt.ToString("0.00") + ")\n" +
                "Every fires      : " + _everyFires + "  (last at t=" + _lastEveryAt.ToString("0.00") + ")\n" +
                "Repeat active    : " + _everyHandle.IsActive + "\n" +
                "Dispatcher fires : " + _dispatcherFires + "  (pending " + MainThreadDispatcher.PendingCount + ")\n" +
                "Scheduler active tasks : " + FrameScheduler.ActiveCount,
                _bodyStyle);

            GUILayout.EndArea();
        }

        void ScheduleNextFrame()
        {
            FrameScheduler.NextFrame(() =>
            {
                _nextFrameFires++;
                _lastNextFrameNumber = Time.frameCount;
                Debug.Log("[SchedulingDemo] NextFrame fired on frame " + Time.frameCount);
            });
        }

        void ScheduleAfter()
        {
            FrameScheduler.After(1.5f, () =>
            {
                _afterFires++;
                _lastAfterAt = Time.time;
                Debug.Log("[SchedulingDemo] After(1.5s) fired at t=" + Time.time.ToString("0.00"));
            });
        }

        void StartEvery()
        {
            _everyHandle.Cancel();
            _everyHandle = FrameScheduler.Every(0.5f, () =>
            {
                _everyFires++;
                _lastEveryAt = Time.time;
                Debug.Log("[SchedulingDemo] Every(0.5s) tick #" + _everyFires + " at t=" + Time.time.ToString("0.00"));
            });
        }

        void CancelEvery()
        {
            if (_everyHandle.IsActive)
            {
                _everyHandle.Cancel();
                Debug.Log("[SchedulingDemo] Repeating task cancelled.");
            }
        }

        void DispatchFromWorker()
        {
            int mainThread = Thread.CurrentThread.ManagedThreadId;
            var worker = new Thread(() =>
            {
                int workerThread = Thread.CurrentThread.ManagedThreadId;
                MainThreadDispatcher.Enqueue(() =>
                {
                    _dispatcherFires++;
                    Debug.Log("[SchedulingDemo] Queued on worker thread " + workerThread +
                              ", ran on thread " + Thread.CurrentThread.ManagedThreadId +
                              " (main=" + mainThread + ", IsMainThread=" + MainThreadDispatcher.IsMainThread + ")");
                });
            });
            worker.IsBackground = true;
            worker.Start();
        }

        void EnsureStyles()
        {
            if (_boxStyle != null)
            {
                return;
            }

            _boxStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(12, 12, 12, 12),
                alignment = TextAnchor.UpperLeft
            };

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold
            };

            _bodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                richText = false,
                wordWrap = false
            };
        }
    }
}
