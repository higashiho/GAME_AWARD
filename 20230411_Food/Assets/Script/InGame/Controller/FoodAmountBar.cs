using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InGame
{
    public class FoodAmountBar 
    {

        private Slider amountSlider;

        public FoodAmountBar(Transform slider)
        {
            this.amountSlider = slider.GetComponent<Slider>();
        }
        // 指定するAmountの量
        private int bestAmount;   
        

        [SerializeField, Header("Amount最小値")]
        private int MIN;
        [SerializeField, Header("Amount最大値")]
        private int MAX;

        void Awake()
        {
            drawSlider(0);
        }

        /// <summary>
        /// スライダーに値をセットするメソッド
        /// </summary>
        public void setValue()
        {
            drawSlider(1);
            amountSlider.value = getRandomValues();
        }

        /// <summary>
        /// ランダムな値を返すメソッド
        /// </summary>
        /// <returns></returns>
        private int getRandomValues()
        {
            int num = UnityEngine.Random.Range(MIN, MAX);
            return num;
        }

        /// <summary>
        /// スライダーを消去するメソッド
        /// </summary>
        public void EraseSlider()
        {
            drawSlider(0);
        }

        private void drawSlider(float val)
        {
            for(int i = 0; i < 2; i++)
            {
                var color = amountSlider.transform.GetChild(0).GetComponent<Image>().color;
                color.a = val;
                amountSlider.transform.GetChild(0).GetComponent<Image>().color = color;
            }
            
        }
        

    }
}

