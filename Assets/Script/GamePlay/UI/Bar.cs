using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    public Slider Slider;
    public int maxCount ;
    public int minCount=0 ;
    [SerializeField]
    private int currentCount;

    void Start()
    {
        // ���ó�ʼ����ֵ
        currentCount = minCount;

        // ���� UI
        UpdateHealthUI();
    }

    // ���� UI ��ʾ
    void UpdateHealthUI()
    {
        if (Slider != null)
        {
            Slider.value = currentCount;
        }
    }


    // ����ֵ
    public void TakeDamage()
    {
        currentCount--;

        // ����Ƿ���Ҫ��������ֵ
        if (currentCount < 0)
        {
            currentCount = maxCount;
        }

        // ���� UI
        UpdateHealthUI();
    }

    //�ƽ�ֵ
    public void PushCount()
    {
        currentCount++;

        if (currentCount > 6)
        {
            currentCount = minCount;
        }

        // ���� UI
        UpdateHealthUI();
    }

}
