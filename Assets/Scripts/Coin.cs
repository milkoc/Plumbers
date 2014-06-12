using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour
{

	void OnTriggerEnter2D(Collider2D coll) {
//		if (collider.tag == "Coin") {
//			score += 5;
//			Destroy(collider.gameObject);
//		}
		if (spawnedFlag && coll.gameObject.tag == "Player") {
			despawn();
			CoinController.coins += 1;
		}
	}
	const double respawnSec = 5;
	double timeToSpawn;
	bool spawnedFlag = true;
	Color normalColor;
	void OnCollisionEnter2D(Collision2D coll){
		if (spawnedFlag && coll.gameObject.tag == "Player") {
			despawn();
			CoinController.coins += 1;
		}
	}

	void despawn()
	{
		spawnedFlag = false;
		timeToSpawn = respawnSec;
		normalColor = GetComponent<SpriteRenderer> ().color;
		GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 0f); //invisible
	}

	void spawn()
	{
		spawnedFlag = true;
		GetComponent<SpriteRenderer>().color = normalColor;
	}

	void Update()
	{
		if(!spawnedFlag) {
			timeToSpawn -= Time.deltaTime;
			if (timeToSpawn <= 0){
				spawn();
			}
		}
	}
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if (stream.isWriting) {
			bool spawn = spawnedFlag;
			stream.Serialize(ref spawn);
		} else {
			bool spawned = false;
			stream.Serialize(ref spawned);
			spawnedFlag = spawned;
		}
	}
}