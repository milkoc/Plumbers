using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour
{

	void OnTriggerEnter2D(Collider collider ) {
//		if (collider.tag == "Coin") {
//			score += 5;
//			Destroy(collider.gameObject);
//		}
				if (collider.gameObject.tag == "Player") {
						Destroy (this.gameObject);	
						CoinController.coins += 5;
				}
		}
	void OnCollisionEnter2D(Collision2D coll){
		if (coll.gameObject.tag == "Player") {
			Destroy(this.gameObject);	
			CoinController.coins += 5;
		}
	}
}