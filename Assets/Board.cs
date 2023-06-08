using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Inkarnate
{
    public class Board : MonoBehaviour
    {
        private float xGap = 1.2f;
        private float yGap = 1.2f;
		public GameObject LetterTilePrefab;
        private int seed = 126;
		private List<GameObject> rows = new List<GameObject>();

        private List<LetterTile> chain = new List<LetterTile>();
        private LetterTile lastClicked;

		private List<string> validWords;

		public TextMeshProUGUI CurrentWord;
		public TextMeshProUGUI Timer;
		public TextMeshProUGUI Total;
		public TMP_InputField ScrollView;

		public GameState GameState;

		private List<string> submittedWords = new List<string>();
		private string currentWord = string.Empty;

		// Start is called before the first frame update
		void Start()
        {
			StartCoroutine(GetWords());
            GenerateRows();
			GenerateTiles();
            PositionBoard();
            UnityEngine.Random.InitState(Mathf.FloorToInt(System.DateTime.Now.TimeOfDay.TotalMilliseconds.ConvertTo<float>()));
        }

		public IEnumerator GetWords()
		{
			using (WWW request = new WWW("https://raw.githubusercontent.com/raun/Scrabble/master/words.txt"))
			{
				yield return request;
				validWords = request.text.Split("\n").ToList();
			}
		}

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(point, Vector2.zero);

                if (hit.collider != null)
                {
                    LetterTile tile = hit.transform.GetComponent<LetterTile>();
                    Click(tile);
                }
                else
                {
                }
            }
        }

        private void GenerateRows()
        {
			for (int i = 0; i < 5; i++)
			{
				GameObject row = new GameObject($"Row ${i}");
				rows.Add(row);
				row.transform.Translate(0.0f, i * yGap, 0.0f);
                row.transform.parent = this.transform;
			}
		}

        private void GenerateTiles()
        {
            List<string> lines = GetAllLines();

			int position = 0;
            while (position < 25)
            {
                int count = 25;
                int index = UnityEngine.Random.Range(0, count - position);
                string letter = lines[index];
                lines.RemoveAt(index);

				int rowNumber = Mathf.FloorToInt(position == 0 ? 0 : position / 5);
                int columnNumber = rowNumber == 0 ? position : position % (rowNumber * 5);

                GameObject row = rows[rowNumber];

				LetterTile tile = CreateTile(letter, rowNumber, columnNumber);
                tile.transform.parent = row.transform;
				tile.transform.localPosition = Vector3.zero;
                tile.transform.Translate(xGap * columnNumber, 0.0f, 0.0f);

				position++;
            }
		}

        private List<string> GetAllLines()
        {
			using StreamReader reader = new StreamReader("Assets/Resources/letters.txt");
			string line = reader.ReadLine();
			List<string> lines = new List<string>();

			while (line != null)
			{
				lines.Add(line.ToUpper());
				line = reader.ReadLine();
			}
            return lines;
		}

        private LetterTile CreateTile(string letters, int row, int column)
        {
            LetterTile tile = Instantiate(LetterTilePrefab).GetComponent<LetterTile>();

            int position = UnityEngine.Random.Range(0, 6);

			string letter = letters.Substring(0, 1);
			letter = letter == "Q" ? "QU" : letter;

			tile.Init(letter, row, column);

            return tile;
        }

        private void PositionBoard()
        {
            this.transform.Translate(-(4 * xGap)/2.0f, -(4 * yGap)/2.0f, 0.0f);
        }

		private void Click(LetterTile tile)
		{
			if (ValidClick(tile))
			{
				if (tile == lastClicked)
				{
					Deselect(tile);
				}
				else
				{
					Select(tile);
				}

				UpdateCurrentWord();
			}
		}

		private void UpdateCurrentWord()
		{
			currentWord = GetCurrentWord();
			CurrentWord.text = currentWord;
		}

		private bool ValidClick(LetterTile tile)
		{
			if (lastClicked == null || tile == lastClicked)
			{
				return true;
			}

			if (chain.Contains(tile))
			{
				return false;
			}
			
			if (Adjacent(lastClicked, tile) == false)
			{
				return false;
			}

			return true;
		}

		private bool Adjacent(LetterTile a, LetterTile b)
		{
			int rowDiff = Math.Abs(a.Row - b.Row);
			int colDiff = Math.Abs(a.Column - b.Column);

			if ((rowDiff == 0 || rowDiff == 1) && (colDiff == 0 || colDiff == 1))
			{
				return true;
			}

			return false;
		}

		private void Select(LetterTile tile)
		{
			if (lastClicked != null)
			{
				lastClicked.Select(false);
			}

			lastClicked = tile;
			chain.Add(tile);
			lastClicked.Select(true);
		}

		private void Deselect(LetterTile tile)
		{
			tile.Deselect();
			chain.Remove(tile);
			lastClicked = chain.LastOrDefault();

			if (lastClicked != null)
			{
				lastClicked.Select(true);
			}
		}

		private string GetCurrentWord()
		{
			string word = string.Empty;
			foreach (string letter in chain.Select(l => l.Letter))
			{
				word += letter;
			}

			return word;
		}

		public void Submit()
		{
			if (currentWord.Length > 2 && submittedWords.Contains(currentWord) == false && validWords.Contains(currentWord))
			{
				Score(currentWord);
				//ScrollView.text.Add(new Label(currentWord));
			}
			else
			{
				Penalty();
			}

			foreach (LetterTile tile in chain.ToList())
			{
				tile.Deselect();
			}
			chain.Clear();
			UpdateCurrentWord();
		}

		private void Score(string word)
		{
			submittedWords.Add(word);
			GameState.Score += GetPoints(word);
			Total.text = GameState.Score.ToString();
		}

		private void Penalty()
		{
			GameState.Score -= 2;
			Total.text = GameState.Score.ToString();
		}

		private int GetPoints(string word)
		{
			int points = 0;
			switch (word.Length)
			{
				case < 5:
					points = 1;
					break;
				case 5:
					points = 2;
					break;
				case < 7:
					points = 3;
					break;
				default:
					points = 8;
					break;
			}
			return points;
		}
	}
}
