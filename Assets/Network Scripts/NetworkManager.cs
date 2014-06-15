using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{

		public GameObject playerPrefab;
		private const string typeName = "SuperPlumbingSquadSE";
		private const string gameName = "Room1";
		private const int port = 11000;
		private const int maxPlayers = 4;
		//public static NetworkPlayer gracze = new NetworkPlayer[4];
		public static NetworkPlayer[] gracze;
		private int liczba = 0 ; 
		private void StartServer ()
		{
				Network.InitializeServer (maxPlayers, port, !Network.HavePublicAddress ());
				MasterServer.RegisterHost (typeName, gameName);
		}

		void OnServerInitialized ()
		{
				Debug.Log ("Server Initialized");
				//SpawnPlayer ();
		}

		private HostData[] hostList;

		private void RefreshHostList ()
		{
				MasterServer.RequestHostList (typeName);
		}

		void OnMasterServerEvent (MasterServerEvent msEvent)
		{
				if (msEvent == MasterServerEvent.HostListReceived)
						hostList = MasterServer.PollHostList ();
		}

		private void JoinServer (HostData hostData)
		{
				Network.Connect (hostData);
		}

		void OnConnectedToServer ()
		{
			Debug.Log ("Server Joined");
			SpawnPlayer ();
			
				
		}
		void OnPlayerConnected(NetworkPlayer player)
		{
			gracze [liczba] = player;
			liczba++;
		}

		private void SpawnPlayer ()
		{
				Network.Instantiate (playerPrefab, new Vector3 (0f, 5f, 0f), Quaternion.identity, 0);
		}

		void OnGUI ()
		{
				if (!Network.isClient && !Network.isServer) {
						if (GUI.Button (new Rect (100, 100, 250, 100), "Start Server")){

								StartServer ();
								Application.LoadLevel("server");

							}

						if (GUI.Button (new Rect (100, 250, 250, 100), "Refresh Hosts"))
								RefreshHostList ();

						if (hostList != null) {
								for (int i = 0; i < hostList.Length; i++) {
										if (GUI.Button (new Rect (400, 100 + (110 * i), 300, 100), hostList [i].gameName))
												JoinServer (hostList [i]);
								}
						}
				} /*else if (Network.isClient) {
						if (GUI.Button (new Rect (0, 100, 100, 50), "Disconnect"))
								Network.CloseConnection (Network.connections [0], true);
				} */else if (Network.isServer) {
			if (GUI.Button (new Rect (0, 100, 100, 50), "Close Server")) {
							//networkView.RPC("RemoteDisconnect", RPCMode.Others);


								Network.Disconnect (200);
								MasterServer.UnregisterHost ();
						}
				}
		}

		// Use this for initialization
		void Start ()
		{

		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}

	void OnPlayerDisconnected(NetworkPlayer player) {
		Debug.Log("Clean up after player " +  player);
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}
}
