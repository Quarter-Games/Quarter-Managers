using UnityEngine;
using UnityEngine.Audio;

namespace QG.Managers.SoundSystem
{
    [CreateAssetMenu(fileName = "Track", menuName = "Quarter Asset/Sound System/Track Settings")]
    public class AudioSettings : ScriptableObject
    {
        public AudioResource AudioClip = null;
        public AudioMixerGroup OutputAudioMixerGroup = null;
        public bool Mute = false;
        public bool BypassEffects = false;
        public bool BypassListenerEffects = false;
        public bool BypassReverbZones = false;
        public bool Loop = false;
        [Space]
        [Range(0, 256)] public int Priority = 128;
        [Range(0, 1)] public float Volume = 1;
        [Range(-3, 3)] public float Pitch = 1;
        [Range(-1, 1)] public float PanStereo = 0;
        [Range(0, 1)] public float SpatialBlend = 0;
        [Range(0, 1.1f)] public float ReverbZoneMix = 1;
        public void ApplySettings(AudioSource source)
        {
            source.resource = AudioClip;
            source.outputAudioMixerGroup = OutputAudioMixerGroup;
            source.mute = Mute;
            source.bypassEffects = BypassEffects;
            source.bypassListenerEffects = BypassListenerEffects;
            source.bypassReverbZones = BypassReverbZones;
            source.loop = Loop;
            source.priority = Priority;
            source.volume = Volume;
            source.pitch = Pitch;
            source.panStereo = PanStereo;
            source.spatialBlend = SpatialBlend;
            source.reverbZoneMix = ReverbZoneMix;
        }
    }
}