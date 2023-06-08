using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Inkarnate
{
    public class HighScores : MonoBehaviour
    {
        public ScrollView ScoreList;
        public TMP_InputField NameInput;

        private GameState gameState;

		// Start is called before the first frame update
		void Start()
        {
            gameState = FindObjectOfType<GameState>();
		}

        // Update is called once per frame
        void Update()
        {
        
        }

        public void Submit()
        {
            ScoreList.Add(new Label($"{NameInput.text}: {gameState.Score}"));
        }
    }
}
