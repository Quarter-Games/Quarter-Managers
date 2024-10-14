using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using System.Threading.Tasks;

namespace QG.Managers.SoundSystem
{
    using QG.Managers.SaveSystem.Basic;
    [AddComponentMenu("QG/Managers/Sound Manager")]

    /// <summary>
    /// Mixer Based Sound Manager
    /// Volume parameter of mixers should be named as "<MIXER_GROUP_NAME>_Volume"
    /// </summary>
    public class SoundManager : SingletonManager<SoundManager>
    {
        public const string VOLUME_PARAM_NAME = "_Volume";
        public List<AudioMixerGroup> MixerGroups;
        public List<AudioSource> Sources;
        private bool isReady = false;
        override public bool IsReady() => isReady;
        public override void Init()
        {
            base.Init();
            StartCoroutine(Initialization());
        }
        private IEnumerator Initialization()
        {
            Task loading = Task.Run(LoadSettings);
            yield return new WaitUntil(() => loading.IsCompleted);
            isReady = true;
        }

        async Task LoadSettings()
        {
            List<Task> loadingTasks = new();
            foreach (var mixer in MixerGroups)
            {
                loadingTasks.Add(LoadMixerSettings(mixer));
            }
            await Task.WhenAll(loadingTasks);
        }

        async Task LoadMixerSettings(AudioMixerGroup mixer)
        {
            float volume = await BasicSaveLoadManager.GetData(mixer.name + VOLUME_PARAM_NAME, 0f);
            mixer.audioMixer.SetFloat(mixer.name + VOLUME_PARAM_NAME, volume);
        }

        public static void PlayAudio(AudioSettings settings)
        {
            if (Instance != null) Instance.PlayTrack(settings);
            else LoadFallBackManager().PlayTrack(settings);
        }
        public static void TurnAudioOff(AudioSettings setting, float time)
        {
            if (Instance != null) Instance.TurnTrackOff(setting, time);
            else LoadFallBackManager().TurnTrackOff(setting, time);
        }
        public static void TurnMixerOff(AudioSettings settings, float time)
        {
            if (Instance != null) Instance.ChangeMixerTo(settings, time);
            else LoadFallBackManager().ChangeMixerTo(settings, time);
        }
        public static void ChangeVolumeMixer(AudioMixerGroup mixer, float endValue, float time, Func<bool> predicateToReturn = null)
        {
            if (Instance != null) Instance.ChangeMixerVolume(mixer, endValue, time, predicateToReturn);
            else LoadFallBackManager().ChangeMixerVolume(mixer, endValue, time, predicateToReturn);
        }

        private void PlayTrack(AudioSettings settings)
        {
            var source = Sources.Where(x => !x.isPlaying).FirstOrDefault();
            settings.ApplySettings(source);
            source.Play();
        }
        private void TurnTrackOff(AudioSettings settings, float time)
        {
            var source = Sources.Where(x => x.clip == settings.AudioClip).FirstOrDefault();
            if (source == null) return;
            StartCoroutine(LerpVolume(source, 0, time));
        }
        private IEnumerator LerpVolume(AudioSource source, float targetVolume, float time, bool turnOff = true)
        {
            float startVolume = source.volume;
            float elapsedTime = 0;

            while (elapsedTime < time)
            {
                source.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / time);
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            source.volume = targetVolume;
            if (turnOff) source.Stop();
        }
        private void ChangeMixerTo(AudioSettings settings, float time)
        {
            var sources = Sources.Where(x => x.outputAudioMixerGroup == settings.OutputAudioMixerGroup).ToList();
            foreach (var source in sources)
            {
                StartCoroutine(LerpVolume(source, 0, time));
            }
            PlayTrack(settings);
        }
        private void ChangeMixerVolume(AudioMixerGroup mixer, float endValue, float time, Func<bool> predicateToReturn = null)
        {
            mixer.audioMixer.GetFloat(mixer.name + VOLUME_PARAM_NAME, out float volume);
            StartCoroutine(LerpMixerVolume(mixer, endValue, time, predicateToReturn));
        }
        private IEnumerator LerpMixerVolume(AudioMixerGroup mixer, float targetVolume, float time, Func<bool> predicateToReturn = null)
        {
            mixer.audioMixer.GetFloat(mixer.name + VOLUME_PARAM_NAME, out float volume);
            float elapsedTime = 0;

            while (elapsedTime < time)
            {
                mixer.audioMixer.SetFloat(mixer.name + VOLUME_PARAM_NAME, Mathf.Lerp(volume, targetVolume, elapsedTime / time));
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            mixer.audioMixer.SetFloat(mixer.name + VOLUME_PARAM_NAME, targetVolume);
            if (predicateToReturn == null) yield break;
            yield return new WaitUntil(predicateToReturn);
            elapsedTime = 0;
            while (elapsedTime < time)
            {
                mixer.audioMixer.SetFloat(mixer.name + VOLUME_PARAM_NAME, Mathf.Lerp(targetVolume, volume, elapsedTime / time));
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            mixer.audioMixer.SetFloat(mixer.name + VOLUME_PARAM_NAME, volume);
        }
    }
}