using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameOff2023.GamePlay.UI {
    /// <summary>
    /// ¿ØÖÆ»¬¶¯Ìõui
    /// </summary>
    public class SliderBar : MonoBehaviour {

        [SerializeField] Image BGImage;
        [SerializeField] Image StatuImage;
        [SerializeField] TMP_Text barText;

        public void UpdateSliderBar(float maxStatu, float currentStatu) {
            float width = BGImage.GetComponent<RectTransform>().sizeDelta.x;
            float height = BGImage.GetComponent<RectTransform>().sizeDelta.y;

            float targetWidth = Mathf.Min(width * currentStatu / maxStatu, width) ;
            StatuImage.GetComponent<RectTransform>().sizeDelta = new Vector2(targetWidth, height);

            barText.text = $"{currentStatu} / {maxStatu}";
        }

    }
}