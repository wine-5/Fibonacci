using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Fibonacci.Audio
{
    public class AudioManager : Singleton<AudioManager>
    {
        protected override bool UseDontDestroyOnLoad => false;

        [Header("Audio Data")]
        [SerializeField] private AudioDataSO[] audioDataArray;
        [SerializeField] private int maxAudioSources = 10;
        
        private Dictionary<string, AudioData> audioDictionary;
        private List<AudioSource> audioSourcePool;
        private Queue<AudioSource> availableAudioSources;

        protected override void Awake()
        {
            base.Awake();
            InitializeAudioDictionary();
            SetupAudioSourcePool();
        }

        private void InitializeAudioDictionary()
        {
            audioDictionary = new Dictionary<string, AudioData>();
            
            if (audioDataArray != null)
            {
                foreach (var audioDataSO in audioDataArray)
                {
                    if (audioDataSO != null && audioDataSO.AudioDataList != null)
                    {
                        foreach (var audioData in audioDataSO.AudioDataList)
                        {
                            if (audioData != null && !string.IsNullOrEmpty(audioData.AudioName))
                            {
                                if (audioDictionary.ContainsKey(audioData.AudioName))
                                {
                                    Debug.LogWarning($"Duplicate audio name found: {audioData.AudioName}. Skipping...");
                                }
                                else
                                {
                                    audioDictionary[audioData.AudioName] = audioData;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SetupAudioSourcePool()
        {
            audioSourcePool = new List<AudioSource>();
            availableAudioSources = new Queue<AudioSource>();

            for (int i = 0; i < maxAudioSources; i++)
            {
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                audioSourcePool.Add(audioSource);
                availableAudioSources.Enqueue(audioSource);
            }
        }

        /// <summary>
        /// 指定した名前の音声を再生します
        /// </summary>
        /// <param name="audioName">音声データの名前</param>
        public void Play(string audioName)
        {
            if (audioDictionary.TryGetValue(audioName, out AudioData audioData))
            {
                if (audioData.AudioClip != null)
                {
                    AudioSource audioSource = GetAvailableAudioSource();
                    if (audioSource != null)
                    {
                        audioSource.clip = audioData.AudioClip;
                        audioSource.volume = audioData.VolumeMultiplier;
                        audioSource.Play();
                        Debug.Log($"Playing audio: {audioName} with volume: {audioData.VolumeMultiplier}");
                        
                        StartCoroutine(ReturnAudioSourceWhenFinished(audioSource));
                    }
                    else
                    {
                        Debug.LogWarning("No available AudioSource to play the audio");
                    }
                }
                else
                {
                    Debug.LogWarning($"AudioClip is null for audio: {audioName}");
                }
            }
            else
            {
                Debug.LogWarning($"Audio not found: {audioName}");
            }
        }

        private AudioSource GetAvailableAudioSource()
        {
            // 利用可能なAudioSourceがあればそれを返す
            if (availableAudioSources.Count > 0)
            {
                return availableAudioSources.Dequeue();
            }

            // なければ再生していないAudioSourceを探す
            foreach (var audioSource in audioSourcePool)
            {
                if (!audioSource.isPlaying)
                {
                    return audioSource;
                }
            }

            return null;
        }

        private System.Collections.IEnumerator ReturnAudioSourceWhenFinished(AudioSource audioSource)
        {
            while (audioSource.isPlaying)
            {
                yield return null;
            }

            availableAudioSources.Enqueue(audioSource);
        }

        /// <summary>
        /// すべての音声を停止します
        /// </summary>
        public void StopAll()
        {
            foreach (var audioSource in audioSourcePool)
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                    availableAudioSources.Enqueue(audioSource);
                }
            }
        }

        /// <summary>
        /// 指定した名前の音声をすべて停止します
        /// </summary>
        /// <param name="audioName">停止する音声の名前</param>
        public void Stop(string audioName)
        {
            if (audioDictionary.TryGetValue(audioName, out AudioData audioData))
            {
                foreach (var audioSource in audioSourcePool)
                {
                    if (audioSource.isPlaying && audioSource.clip == audioData.AudioClip)
                    {
                        audioSource.Stop();
                        availableAudioSources.Enqueue(audioSource);
                    }
                }
            }
        }
    }
}
