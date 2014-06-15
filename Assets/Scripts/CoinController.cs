using UnityEngine;
using System.Collections;

public class CoinController : MonoBehaviour
{
	const int scorePerCoin = 5;
	public static int coins = 0;
		// Use this for initialization
		void OnGUI() {
	

			string coinText = "Score: " + coins * scorePerCoin;
			GUI.Box (new Rect (Screen.width - 150, 20, 130, 20), coinText);
		if (NetworkManager.gracze [0] == Network.player) {
				GUI.Box (new Rect (10, 20, 130, 20), "Gracz 1");
				}
//		if(NetworkManager.gracze[0] == Network.player){
//			
//			coins = Player.punkty[0];
//
//		}
//		else if (NetworkManager.gracze [1] == Network.player){
//			
//			coins = Player.punkty [1];
//			GUI.Box (new Rect (10, 20, 130, 20), "Gracz 1");
//		}
//		
//		else if (NetworkManager.gracze [2] == Network.player){
//			coins = Player.punkty [2];
//			GUI.Box (new Rect (10, 20, 130, 20), "Gracz 1");
//		}
//		else if (NetworkManager.gracze [3] == Network.player){
//			coins = Player.punkty [3];
//			GUI.Box (new Rect (10, 20, 130, 20), "Gracz 1");
//		}
		if(Player.punkty[0] == 100)
		{
			for(int i = 0 ; i < Player.punkty.Length ; i++){
				Player.punkty[i]=0;
			}

			GUI.Box (new Rect (Screen.width/2, 20, 130, 20), "Wygrał gracz 1");
		}
		else if(Player.punkty[1] == 100)
		{
			for(int i = 0 ; i < Player.punkty.Length ; i++){
				Player.punkty[i]=0;
			}
			GUI.Box (new Rect (Screen.width/2, 20, 130, 20), "Wygrał gracz 2");
		}
		else if(Player.punkty[2] == 100)
		{
			for(int i = 0 ; i < Player.punkty.Length ; i++){
				Player.punkty[i]=0;
			}
			GUI.Box (new Rect (Screen.width/2, 20, 130, 20), "Wygrał gracz 3");
		}
		else if(Player.punkty[3] == 100)
		{
			for(int i = 0 ; i < Player.punkty.Length ; i++){
				Player.punkty[i]=0;
			}
			GUI.Box (new Rect (Screen.width/2, 20, 130, 20), "Wygrał gracz 4");
		}
	}
}