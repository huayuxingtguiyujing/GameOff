using GameOff2023.Infrastructure.Audio;
using System.Transactions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject pauseMenuUI;

    public AudioSource musicAudioSource;
    public Slider musicVolumeSlider;

    public void NewGame()
    {
        // 加载到指定的场景
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // 重新开始游戏
    public void RestartGame()
    {
        // 获取当前场景的名称
        string currentSceneName = SceneManager.GetActiveScene().name;

        // 重新加载当前场景
        SceneManager.LoadScene(currentSceneName);
    }

    //暂停菜单
    public void Pause()
    {
        Time.timeScale = 0f; // 暂停游戏时间流逝
        pauseMenuUI.SetActive(true); // 显示暂停菜单
    }

    public void Resume()
    {
        Time.timeScale = 1f; // 恢复游戏时间流逝
        pauseMenuUI.SetActive(false); // 隐藏暂停菜单
    }

    //设置菜单
    public void OpenSettings()
    {
        // 这里添加打开设置菜单的代码
        Debug.Log("Open Settings");
    }

    // 设置音乐音量
    public void SetMusicVolume(float volume)
    {
        if (musicAudioSource != null)
        {
            musicAudioSource.volume = volume;
        }
    }

    // 处理音乐音量更改
    public void OnMusicVolumeChanged()
    {
        float volume = musicVolumeSlider.value;
        SetMusicVolume(volume);
    }

}