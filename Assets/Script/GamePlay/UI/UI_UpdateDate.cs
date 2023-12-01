using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_UpdateDate : MonoBehaviour
{
    private int enemyCount = 0;
    public Text enemyCountText;

    void Update()
    {
        

        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemyCount++;
            // 打印或使用敌人数量
            Debug.Log("剩余增援数：" + enemyCount);
            enemyCountText.text = "剩余增援数：" + enemyCount;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemyCount--;
            // 打印或使用敌人数量
            Debug.Log("剩余增援数：" + enemyCount);
            enemyCountText.text = "剩余增援数：" + enemyCount;
        }
    }


  

}
