using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace GameOff2023.Infrastructure.Audio {
    public class AudioSourceHolder : MonoBehaviour {
        [Header("���β�����Ƶ")]
        [Tooltip("������һ��")]
        public List<Sound> combatSounds;

        [Header("������")]
        [Tooltip("��������Ĭ������»�ѭ�����ţ���������Ƶ���ݳ���")]
        public List<Sound> mainThemeSounds;

        [Header("������")]
        [Tooltip("��������Ĭ������»�ѭ�����ţ���������Ƶ���ݳ���")]
        public List<Sound> environmentSounds;
    }

    ///<summary>
    /// ��Ƶ������ �洢���е���Ƶ���ҿ��Բ��ź�ֹͣ
    ///</summary>
    [Serializable]
    public class Sound {
        [Header("��Ƶ����")]
        public AudioClip clip;

        [Header("��Ƶ����")]
        public AudioMixerGroup outputGroup = null;

        [Header("��Ƶ����")]
        [Range(0, 1)]
        public float volume;

        [Header("�Զ�����")]
        public bool PlayOnAwake;

        [Header("ѭ������")]
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