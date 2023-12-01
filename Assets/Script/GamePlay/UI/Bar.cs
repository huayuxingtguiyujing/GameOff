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
        // 设置初始受伤值
        currentCount = minCount;

        // 更新 UI
        UpdateHealthUI();
    }

    // 更新 UI 表示
    void UpdateHealthUI()
    {
        if (Slider != null)
        {
            Slider.value = currentCount;
        }
    }


    // 受伤值
    public void TakeDamage()
    {
        currentCount--;

        // 检查是否需要重置受伤值
        if (currentCount < 0)
        {
            currentCount = maxCount;
        }

        // 更新 UI
        UpdateHealthUI();
    }

    //推进值
    public void PushCount()
    {
        currentCount++;

        if (currentCount > 6)
        {
            currentCount = minCount;
        }

        // 更新 UI
        UpdateHealthUI();
    }

}
