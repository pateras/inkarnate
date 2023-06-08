using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Inkarnate
{
    public class GameState : MonoBehaviour
    {
        public int Score { get; set; }

		private void Awake()
		{
            DontDestroyOnLoad(this.gameObject);
		}
		// Start is called before the first frame update
		void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
