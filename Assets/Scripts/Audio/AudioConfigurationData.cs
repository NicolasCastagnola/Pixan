using UnityEngine;

namespace Audio
{
    [CreateAssetMenu(fileName = "AudioConfiguration_SO", menuName = "Create Audio Configuration")]
    public class AudioConfigurationData : ScriptableObject
    {
        public object ID { get; private set; }
        
        [SerializeField, Tooltip("lock this sound from playing")] private bool Locked;
        
        [Tooltip("target audio clip")]public AudioClip clip;
        [Tooltip("target audio channel")] public AudioMixerChannels channel;
        [Tooltip("should loop this forever")] public bool loop;
        [Range(0, 1), Tooltip("volume to play this sound")] public float volume;
        [Tooltip("delay to play this sound"), Min(0)] public float delay;
        
        public void Play2D() => Play2D(name);
        public void Play2D(object id) => Play3D(id, AudioManager.SoundParameters.Default2D);
        public void Play3D() => Play3D(name);
        public void Play3D(object id) => Play3D(id,AudioManager.SoundParameters.Default3D);
        public void Play3D(object id, AudioManager.SoundParameters parameters)
        {
            if (Locked) return;

            if (AudioManager.Instance == null) return;
            
            ID = id;
        
            AudioManager.Instance.Play(id, this, parameters);
        }

        public void Pause(object id)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.Pause(id);
        }
    
        public void Stop(object id)
        {
            if (AudioManager.Instance == null) return;
            
            AudioManager.Stop(id);
            ID = null;
        }
        public void Stop() => Stop(name);
    }
}