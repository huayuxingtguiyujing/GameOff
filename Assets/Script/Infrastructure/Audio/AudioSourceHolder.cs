using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace GameOff2023.Infrastructure.Audio {
    public class AudioSourceHolder : MonoBehaviour {
        [Header("单次播放音频")]
        [Tooltip("仅播放一次")]
        public List<Sound> combatSounds;

        [Header("主题曲")]
        [Tooltip("主题曲，默认情况下会循环播放，与其他音频兼容出现")]
        public List<Sound> mainThemeSounds;

        [Header("环境音")]
        [Tooltip("环境音，默认情况下会循环播放，与其他音频兼容出现")]
        public List<Sound> environmentSounds;
    }

    ///<summary>
    /// 音频管理器 存储所有的音频并且可以播放和停止
    ///</summary>
    [Serializable]
    public class Sound {
        [Header("音频剪辑")]
        public AudioClip clip;

        [Header("音频分组")]
        public AudioMixerGroup outputGroup = null;

        [Header("音频音量")]
        [Range(0, 1)]
        public float volume;

        [Header("自动启动")]
        public bool PlayOnAwake;

        [Header("循环播放")]
        public bool loop;

        public Sound(AudioClip clip) {
            this.clip = clip;
            volume = 0.5f;
            PlayOnAwake = false;
            loop = false;
        }
    }

    public enum SoundType {
        MainTheme,
        Combat,
        Environment
    }

}