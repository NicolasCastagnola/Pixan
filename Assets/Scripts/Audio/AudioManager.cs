using System;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Audio
{
    public class AudioManager : BaseMonoSingleton<AudioManager>
    {
        private readonly List<AudioPlayer> _activeAudioSources = new List<AudioPlayer>();
        private readonly Dictionary<AudioPlayer, object> _audioSourceToId = new Dictionary<AudioPlayer, object>();
        private readonly Dictionary<object, AudioPlayer> _idToAudioSource = new Dictionary<object, AudioPlayer>();
        
        private AudioSource _mainMusicSource;
        
        [SerializeField] private AudioConfigurationData _mainTheme;
        [SerializeField] private AudioSource prototype;
        [SerializeField] private float smooth3DSound;
        [SerializeField] private AudioMixer _audioMixer;

        public Dictionary<AudioMixerChannels, bool> activeChannels = new Dictionary<AudioMixerChannels, bool>();
        public AudioMixer AudioMixer => _audioMixer;
        public List<string> debugSoundList;

        [Serializable]
        public struct SoundParameters
        {
            private static Vector2 DefaultMinMaxDistance { get; } = new Vector2(1, 10);
            public static SoundParameters Default2D { get; } = new SoundParameters()
            {
                SourceTransform = null,
                MinMaxDistance = Vector2.zero
            };

            public static SoundParameters Default3D { get; } = new SoundParameters()
            {
                SourceTransform = null,
                MinMaxDistance = DefaultMinMaxDistance
            };

            public Transform SourceTransform;
            /// <summary>
            /// Debajo de la distancia Min, el sonido se escucha al 100%, sobre la distancia maxima el sonido no se escucha.
            /// </summary>
            [MinMaxSlider(0, 30)] public Vector2 MinMaxDistance;

            public SoundParameters(Transform transform)
            {
                SourceTransform = transform;
                MinMaxDistance = DefaultMinMaxDistance;
            }
            public SoundParameters(Transform transform, Vector2 distance)
            {
                SourceTransform = transform;
                MinMaxDistance = distance;
            }
        }

        protected override void Start()
        {
            base.Start();
            
            activeChannels.Add(AudioMixerChannels.MUSIC, true);
            activeChannels.Add(AudioMixerChannels.SFX, true);
            activeChannels.Add(AudioMixerChannels.FX2, true);

            foreach (var item in activeChannels)
            {
                LoadChannel(item.Key);
                Instance._audioMixer.GetFloat(item.Key.ToString() + "_VOLUME", out float debugValue);
            }
            if (_mainTheme)
            {
                _mainMusicSource = gameObject.AddComponent<AudioSource>();
                _mainMusicSource.clip = _mainTheme.clip;
                _mainMusicSource.outputAudioMixerGroup = _audioMixer.FindMatchingGroups(_mainTheme.channel.ToString())[0];
                _mainMusicSource.volume = _mainTheme.volume;
                _mainMusicSource.loop = _mainTheme.loop;
                _mainMusicSource.Play();
            }
            
            CreateFreeSourcesForEachChannel();

            SceneManager.activeSceneChanged += (x, y) => { PauseResumeAllSounds(true);};
        }

        private void CreateFreeSourcesForEachChannel()
        {
            for (int i = 0; i < 8; i++)
            {
                foreach ((var key, bool value) in activeChannels)
                {
                    
                    var audioSource = Instantiate(Instance.prototype.gameObject).GetComponent<AudioSource>();
                    audioSource.transform.SetParent(gameObject.transform);
                    audioSource.outputAudioMixerGroup = _audioMixer.FindMatchingGroups(key.ToString())[0];
                    audioSource.playOnAwake = false;
                    
                    var audioSourceConfig = new AudioPlayer(audioSource);

                    _activeAudioSources.Add(audioSourceConfig);
                }
            }
        }

        private void LoadChannel(AudioMixerChannels channel)
        {
            var key = channel.ToString();
            var value = true;
            
            if (PlayerPrefs.HasKey(key))
            {
                value = PlayerPrefs.GetInt(key) == 1;
            }
            
            SetChannelActive(value,channel);
        }
        protected override void OnDestroy()
        {
            if(!GameManager.disableLogs) Debug.Log("AudioManager > OnDestroy");
            base.OnDestroy();
        }

        public static bool IsPlaying(object id)
        {
            if (Instance._idToAudioSource.TryGetValue(id, out AudioPlayer audioPlayer))
                return audioPlayer.IsPlaying;
            return false;
        }

        public void Play(object id, AudioConfigurationData audioConfigurationData, SoundParameters parameters)
        {
            //if(!GameManager.disableLogs) Debug.Log($"||| PLAY ID:{id}");
            if (audioConfigurationData.delay > 0)
                StartCoroutine(Wait(audioConfigurationData.delay, PlaySound));
            else
                PlaySound();

            void PlaySound()
            {
                Stop(id);

                var targetGroup = _audioMixer.FindMatchingGroups(audioConfigurationData.channel.ToString())[0];
                var audioPlayer = GetNextAudioSourceConfig();
                
                audioPlayer.SetAudioMixerGroup(targetGroup);
                audioPlayer.SetTransform(parameters.SourceTransform);
                audioPlayer.SetMinMaxDistance(parameters.MinMaxDistance);
                audioPlayer.SetVolume(audioConfigurationData.volume);
                audioPlayer.SetClip(audioConfigurationData.clip);
                audioPlayer.SetLoop(audioConfigurationData.loop);
                audioPlayer.SetId(audioConfigurationData.ID);
                audioPlayer.Play();
                
                _idToAudioSource[id] = audioPlayer;
            }
        }

        public static void Stop(object id)
        {
            //if(!GameManager.disableLogs) Debug.Log($"||| STOP ID:{id}");
            if (Instance._idToAudioSource.TryGetValue(id, out AudioPlayer oldAudioSourceConfig))
            {
                oldAudioSourceConfig.Stop();
                Instance._idToAudioSource.Remove(id);
            }
        }
        public void Pause(object id)
        {
            if (_idToAudioSource.TryGetValue(id, out AudioPlayer oldAudioSourceConfig))
                oldAudioSourceConfig.Pause();
        }

        private AudioPlayer GetNextAudioSourceConfig()
        {
            int index = _activeAudioSources.FindIndex(x => x.IsAvailable);
            
            AudioPlayer audioPlayer = null;

            if (index != -1) //Si estamos reutilizando, lo sacamos de la lista.
            {
                if (_activeAudioSources[index].IsPlaying)
                {
                    audioPlayer = CreateNewAudioSourceConfig();
                    _activeAudioSources.Add(audioPlayer); 
                    return audioPlayer;
                }
                
                audioPlayer = _activeAudioSources[index];
                _activeAudioSources.RemoveAt(index);
            }

            audioPlayer ??= CreateNewAudioSourceConfig();
            _activeAudioSources.Add(audioPlayer); //Colocamos el audio source al final de la lista
            return audioPlayer;
        }

        private static AudioPlayer CreateNewAudioSourceConfig()
        {
            var audioSource = Instantiate(Instance.prototype.gameObject).GetComponent<AudioSource>();
            var audioSourceConfig = new AudioPlayer(audioSource);
            audioSource.playOnAwake = false;
            return audioSourceConfig;
        }

        public static void SetChannelActive(bool state, AudioMixerChannels channel)
        {
            int volume = (state ? 0 : -80);
            string property = channel + "_VOLUME";
            Instance._audioMixer.SetFloat(property, volume);
            PlayerPrefs.SetInt(channel.ToString(), state ? 1 : 0);
        }

        public static bool GetChannelState(AudioMixerChannels channel)
        {
            string property = channel.ToString() + "_VOLUME";
            Instance._audioMixer.GetFloat(property, out float volume);
            return volume == 0;
        }

        public void PauseResumeAllSounds(bool pause)
        {
            foreach (AudioPlayer activeAudioSource in _activeAudioSources)
            {
                if (pause)
                {
                    if (!activeAudioSource.InPause && activeAudioSource.IsPlaying)
                        activeAudioSource.Pause();
                }
                else
                {
                    if (activeAudioSource.InPause)
                        activeAudioSource.UnPause();
                }
            }
        }


        private IEnumerator Wait(float time, Action endAction)
        {
            yield return new WaitForSeconds(time);
            endAction();
        }
        
    }

    public class AudioPlayer
    {
        private Transform _soundRequestTransform;
        private readonly AudioSource _source;
        public float OriginalVolume { get; private set; }
        private float _currentVolume;

        public bool InPause { get; private set; }

        public object Id { get; private set; }

        public AudioPlayer(AudioSource source)
        {
            _source = source;
        }

        public void SetTransform(Transform requestsTransform)
        {
            _soundRequestTransform = requestsTransform;
            _source.transform.SetParent(requestsTransform != null ? requestsTransform : AudioManager.Instance.transform);
            _source.transform.localPosition = Vector3.zero;
        }

        public void SetVolume(float volume)
        {
            OriginalVolume = volume;
            _source.volume = volume;
        }

        public void SetLerpVolume(float targetVolume, float smooth)
        {
            _currentVolume = Mathf.Lerp(_currentVolume, targetVolume, smooth);
            _source.volume = _currentVolume;
        }

        public void SetClip(AudioClip clip) => _source.clip = clip;
        public void SetAudioMixerGroup(AudioMixerGroup audioMixer) => _source.outputAudioMixerGroup = audioMixer;
        public void SetLoop(bool loop) => _source.loop = loop;
        public AudioMixerGroup GetAudioMixerGroup() => _source.outputAudioMixerGroup;
        public void Play()
        {
            var exposeID = _source.outputAudioMixerGroup.name + "_VOLUME";
            _source.outputAudioMixerGroup.audioMixer.GetFloat(exposeID, out float volume);
            _source.Play();
            _source.name = $"AudioSource ID:{Id} Clip:{_source.clip.name}";

        }

        public void Stop()
        {
            _source.Stop();
            Id = null;
            _source.clip = null;
            _source.transform.SetParent(AudioManager.Instance.transform);
            _source.name = $"AudioSource Free";
        }

        public void Pause()
        {
            InPause = true;
            _source.Pause();
        }

        public void UnPause()
        {
            InPause = false;
            _source.UnPause();
        }

        public bool IsAvailable
        {
            get
            {
                if (_source.isPlaying) return false;
                if (InPause) return false;
                return true;
            }
        }

        public bool IsPlaying => _source.isPlaying;

        public void SetId(object id) => Id = id;

        public void SetMinMaxDistance(Vector2 minMaxDistance)
        {
            _source.spatialBlend = minMaxDistance == Vector2.zero ? 0 : 1;

            _source.minDistance = minMaxDistance.x;
            _source.maxDistance = minMaxDistance.y;
        }
    }

    public enum AudioMixerChannels
    {
        None,
        SFX,
        FX2,
        MUSIC,
        FOREVER
    }
}