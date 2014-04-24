using UnityEngine;
using System.Collections;

public class CoinController : MonoBehaviour
{
	public static int coins = 0;
		// Use this for initialization
		void OnGUI(){
			string coinText = "Score: " + coins;
			GUI.Box (new Rect (Screen.width - 150, 20, 130, 20), coinText);
		}
}

