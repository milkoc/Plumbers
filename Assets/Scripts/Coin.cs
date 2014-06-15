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
			if(NetworkManager.gracze[0] == Network.player)
				Player.punkty[0] += 1;
			if(NetworkManager.gracze[1] == Network.player)
				Player.punkty[1] += 1;

			if(NetworkManager.gracze[2] == Network.player)
				Player.punkty[2] += 1;
			if(NetworkManager.gracze[3] == Network.player)
				Player.punkty[3] += 1;
		}
	}
	const double respawnSec = 5;
	double timeToSpawn;
	bool spawnedFlag = true;
	Color normalColor;
	void OnCollisionEnter2D(Collision2D coll){
		if (spawnedFlag && coll.gameObject.tag == "Player") {
			despawn();
			if(NetworkManager.gracze[0] == Network.player)
				Player.punkty[0] += 1;
			if(NetworkManager.gracze[1] == Network.player)
				Player.punkty[1] += 1;
			if(NetworkManager.gracze[2] == Network.player)
				Player.punkty[2] += 1;
			if(NetworkManager.gracze[3] == Network.player)
				Player.punkty[3] += 1;
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
			//double toSpawn = timeToSpawn;
			stream.Serialize(ref spawn);
			//stream.Serialize(ref toSpawn);
		} else {
			bool spawned = false;
			//double toSpawn = 0 ;
			stream.Serialize(ref spawned);
			//stream.Serialize(ref toSpawn);

			spawnedFlag = spawned;
			//timeToSpawn = toSpawn;
		}
	}
}