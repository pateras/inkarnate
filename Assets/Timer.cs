using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Inkarnate
{
    public class Timer : MonoBehaviour
    {
        private TimeSpan remaining = TimeSpan.FromMinutes(3.0f);
        public TextMeshProUGUI Label;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            remaining -= TimeSpan.FromSeconds(Time.deltaTime);
            long ticks = remaining.Ticks;
            Label.text = new DateTime(ticks).ToString("mm:ss");

            if (ticks <= 0)
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            SceneManager.LoadScene("Gameover");
        }
    }
}
