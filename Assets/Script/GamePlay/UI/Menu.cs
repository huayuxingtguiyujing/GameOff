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
        // ���ص�ָ���ĳ���
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // ���¿�ʼ��Ϸ
    public void RestartGame()
    {
        // ��ȡ��ǰ����������
        string currentSceneName = SceneManager.GetActiveScene().name;

        // ���¼��ص�ǰ����
        SceneManager.LoadScene(currentSceneName);
    }

    //��ͣ�˵�
    public void Pause()
    {
        Time.timeScale = 0f; // ��ͣ��Ϸʱ������
        pauseMenuUI.SetActive(true); // ��ʾ��ͣ�˵�
    }

    public void Resume()
    {
        Time.timeScale = 1f; // �ָ���Ϸʱ������
        pauseMenuUI.SetActive(false); // ������ͣ�˵�
    }

    //���ò˵�
    public void OpenSettings()
    {
        // ������Ӵ����ò˵��Ĵ���
        Debug.Log("Open Settings");
    }

    // ������������
    public void SetMusicVolume(float volume)
    {
        if (musicAudioSource != null)
        {
            musicAudioSource.volume = volume;
        }
    }

    // ����������������
    public void OnMusicVolumeChanged()
    {
        float volume = musicVolumeSlider.value;
        SetMusicVolume(volume);
    }

}