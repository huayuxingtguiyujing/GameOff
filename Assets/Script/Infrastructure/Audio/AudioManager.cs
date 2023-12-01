using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

namespace GameOff2023.Infrastructure.Audio {

    

    public class AudioManager : MonoBehaviour {
        public AudioSourceHolder audioSourceHolder;

        //每一个音频的名称与音频组件的映射
        private Dictionary<string, AudioSource> audioDic;

        //
        private Dictionary<string, AudioSource> combatAudioDic;
        private Dictionary<string, AudioSource> mainThemeDic;
        private Dictionary<string, AudioSource> environmentDic;

        #region 初始化
        private static AudioManager instance;
        public static AudioManager Instance { get => instance; set => instance = value; }

        ///<summary>
        /// 初始化音频管理器
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

            //为音频创建AudioSource类
            AudioSource source = obj.AddComponent<AudioSource>();
            source.clip = sound.clip;
            source.volume = sound.volume;
            source.playOnAwake = sound.PlayOnAwake;
            source.loop = sound.loop;
            source.outputAudioMixerGroup = sound.outputGroup;

            if (sound.PlayOnAwake) {
                source.Play();
            }

            //加入到映射中
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

        #region 外部接口
        ///<summary>
        /// 播放某个音频 iswait为是否等待
        ///</summary>
        public static void PlayAudio(string name, bool iswait = false) {
            if (!instance.audioDic.ContainsKey(name)) {
                //不存在音频
                Debug.Log("不存在" + name + "音频");
                return;
            }

            if (iswait) {
                if (!instance.audioDic[name].isPlaying) {
                    //如果需要等待 不会播放
                    instance.audioDic[name].Play();
                }
            }
            else {
                instance.audioDic[name].Play();
            }
        }

        ///<summary>
        /// 停止音频的播放
        ///</summary>
        public static void StopAudio(string name) {
            if (!instance.audioDic.ContainsKey(name)) {
                Debug.LogError("不存在" + name + "音频");
                return;
            }
            else {
                instance.audioDic[name].Stop();
            }
        }

        ///<summary>
        /// 停止所有音频的播放
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