using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

namespace GameOff2023.Infrastructure.Audio {

    

    public class AudioManager : MonoBehaviour {
        public AudioSourceHolder audioSourceHolder;

        //ÿһ����Ƶ����������Ƶ�����ӳ��
        private Dictionary<string, AudioSource> audioDic;

        //
        private Dictionary<string, AudioSource> combatAudioDic;
        private Dictionary<string, AudioSource> mainThemeDic;
        private Dictionary<string, AudioSource> environmentDic;

        #region ��ʼ��
        private static AudioManager instance;
        public static AudioManager Instance { get => instance; set => instance = value; }

        ///<summary>
        /// ��ʼ����Ƶ������
        ///</summary>
        public void InitAudioManager() {
            audioDic = new Dictionary<string, AudioSource>();
            combatAudioDic = new Dictionary<string, AudioSource>();
            mainThemeDic = new Dictionary<string, AudioSource>();
            environmentDic = new Dictionary<string, AudioSource>();
            instance = this;

            foreach (Sound sound in audioSourceHolder.combatSounds) {
                CreateSourceObj(sound, SoundType.Combat);
            }

            foreach (Sound sound in audioSourceHolder.mainThemeSounds) {
                CreateSourceObj(sound, SoundType.MainTheme);
            }
            //environmentSounds
            foreach (Sound sound in audioSourceHolder.environmentSounds) {
                CreateSourceObj(sound, SoundType.Environment);
            }
        }

        private void CreateSourceObj(Sound sound, SoundType soundType) {
            GameObject obj = new GameObject(sound.clip.name);
            obj.transform.SetParent(audioSourceHolder.transform);

            //Ϊ��Ƶ����AudioSource��
            AudioSource source = obj.AddComponent<AudioSource>();
            source.clip = sound.clip;
            source.volume = sound.volume;
            source.playOnAwake = sound.PlayOnAwake;
            source.loop = sound.loop;
            source.outputAudioMixerGroup = sound.outputGroup;

            if (sound.PlayOnAwake) {
                source.Play();
            }

            //���뵽ӳ����
            audioDic.Add(sound.clip.name, source);

            switch (soundType) {
                case SoundType.Combat:
                    combatAudioDic.Add(sound.clip.name, source);
                    break;
                case SoundType.MainTheme:
                    mainThemeDic.Add(sound.clip.name, source);
                    break;
                case SoundType.Environment:
                    environmentDic.Add(sound.clip.name, source);
                    break;
            }
        }
        #endregion

        #region �ⲿ�ӿ�
        ///<summary>
        /// ����ĳ����Ƶ iswaitΪ�Ƿ�ȴ�
        ///</summary>
        public static void PlayAudio(string name, bool iswait = false) {
            if (!instance.audioDic.ContainsKey(name)) {
                //��������Ƶ
                Debug.Log("������" + name + "��Ƶ");
                return;
            }

            if (iswait) {
                if (!instance.audioDic[name].isPlaying) {
                    //�����Ҫ�ȴ� ���Ქ��
                    instance.audioDic[name].Play();
                }
            }
            else {
                instance.audioDic[name].Play();
            }
        }

        ///<summary>
        /// ֹͣ��Ƶ�Ĳ���
        ///</summary>
        public static void StopAudio(string name) {
            if (!instance.audioDic.ContainsKey(name)) {
                Debug.LogError("������" + name + "��Ƶ");
                return;
            }
            else {
                instance.audioDic[name].Stop();
            }
        }

        ///<summary>
        /// ֹͣ������Ƶ�Ĳ���
        ///</summary>
        public static void StopAllAudio() {
            foreach (KeyValuePair<string, AudioSource> keyValue in instance.audioDic)
            {
                keyValue.Value.Stop();
            }
        }
        
        #endregion

    }
}