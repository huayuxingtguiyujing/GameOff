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
            // ��ӡ��ʹ�õ�������
            Debug.Log("ʣ����Ԯ����" + enemyCount);
            enemyCountText.text = "ʣ����Ԯ����" + enemyCount;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemyCount--;
            // ��ӡ��ʹ�õ�������
            Debug.Log("ʣ����Ԯ����" + enemyCount);
            enemyCountText.text = "ʣ����Ԯ����" + enemyCount;
        }
    }


  

}
