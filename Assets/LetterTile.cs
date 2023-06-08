using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LetterTile : MonoBehaviour
{
    public GameObject ModelObject;
    public SpriteRenderer SpriteRenderer;
    public TextMeshPro TextMesh;

    public string Letter { get; private set; }
    public int Row { get; private set; }
    public int Column { get; private set; }

	public void Init(string letter, int row, int column)
    {
        this.Letter = letter;
        TextMesh.text = letter;

        this.Row = row;
        this.Column = column;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Select(bool lastClicked)
    {
        SpriteRenderer.color = lastClicked ? Color.green : Color.yellow;
    }

    public void Deselect()
    {
		SpriteRenderer.color = Color.white;
	}
}
