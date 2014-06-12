using UnityEngine;
using System.Collections;

public class CoinController : MonoBehaviour
{
	const int scorePerCoin = 5;
	public static int coins = 0;
		// Use this for initialization
		void OnGUI() {
		coins = Player.Score;
			string coinText = "Score: " + coins * scorePerCoin;
			GUI.Box (new Rect (Screen.width - 150, 20, 130, 20), coinText);
		}
}

