using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QG.Managers.StateSystem
{

    [AddComponentMenu("QG/Managers/GameState Manager")]
    public class GameStateManager : SingletonManager<GameStateManager>
    {
        private GameState _gameState;
        public static float GameTime { get; private set; }
        public static float DeltaTime { get; private set; }
        Dictionary<Timer, Coroutine> listOfTimers;
        public override bool IsReady() => true;

        public override void Init()
        {
            base.Init();
            _gameState = new GameState();
        }
        private void Update()
        {
            if (_gameState == null) return;
            DeltaTime = Time.unscaledDeltaTime * _gameState.GameTimeScale;
            GameTime += DeltaTime;
        }
        public static void SetState(GameState state)
        {
            if (Instance == null) LoadFallBackManager();
            Instance.ChangeState(state);
        }
        public static void StartTimer(Timer timer)
        {
            if (Instance == null) LoadFallBackManager();
            var coroutine = Instance.StartCoroutine(Instance.Timer(timer));
            if (Instance.listOfTimers == null) Instance.listOfTimers = new Dictionary<Timer, Coroutine>();
            Instance.listOfTimers.Add(timer, coroutine);
        }
        public static void StopTimer(Timer timer, bool raiseCallback = false)
        {
            if (Instance == null) LoadFallBackManager();
            if (Instance.listOfTimers == null) return;
            if (Instance.listOfTimers.ContainsKey(timer))
            {
                Instance.StopCoroutine(Instance.listOfTimers[timer]);
                Instance.listOfTimers.Remove(timer);
                if (raiseCallback) timer.Callback?.Invoke();
            }
        }
        private IEnumerator Timer(Timer timer)
        {
            float startTime = timer.TimerType switch
            {
                TimerType.InGame => GameTime,
                TimerType.Scaled => Time.time,
                TimerType.Fixed => Time.fixedTime,
                TimerType.Unscaled => Time.unscaledTime,
                _ => throw new NotImplementedException()
            };
            yield return new WaitUntil(() =>
            {
                float currentTime = timer.TimerType switch
                {
                    TimerType.InGame => GameTime,
                    TimerType.Scaled => Time.time,
                    TimerType.Fixed => Time.fixedTime,
                    TimerType.Unscaled => Time.unscaledTime,
                    _ => throw new NotImplementedException()
                };
                timer.Progress?.Invoke((currentTime - startTime) / timer.Duration);
                return currentTime - startTime >= timer.Duration;
            });
            timer.Callback?.Invoke();
            listOfTimers.Remove(timer);
        }
        public void ChangeState(GameState state)
        {
            _gameState = state;
            DeltaTime = Time.unscaledDeltaTime * _gameState.GameTimeScale;
            Time.timeScale = _gameState.UnityTimeScale;
        }
    }
    public class Timer
    {
        public float Duration { get; private set; }
        public Action Callback { get; private set; }
        public Action<float> Progress { get; private set; }
        public TimerType TimerType { get; private set; }
        public Timer(float duration, Action callback = null, Action<float> progress = null, TimerType timerType = TimerType.InGame)
        {
            Duration = duration;
            Callback = callback;
            Progress = progress;
            TimerType = timerType;
        }
    }
    public enum TimerType
    {
        InGame,
        Scaled,
        Fixed,
        Unscaled
    }
}