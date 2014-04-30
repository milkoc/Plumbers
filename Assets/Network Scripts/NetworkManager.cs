﻿using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	public GameObject playerPrefab;

    private const string typeName = "SuperPlumbingSquadSE";
    private const string gameName = "Room1";

	private const int port = 11000;
	private const int maxPlayers = 4;

    private void StartServer() {
		Network.InitializeServer(maxPlayers, port, !Network.HavePublicAddress());
        MasterServer.RegisterHost(typeName, gameName);
    }

    void OnServerInitialized() {
        Debug.Log("Server Initialized");
    }

    private HostData[] hostList;

    private void RefreshHostList() {
        MasterServer.RequestHostList(typeName);
    }

    void OnMasterServerEvent(MasterServerEvent msEvent) {
        if (msEvent == MasterServerEvent.HostListReceived)
            hostList = MasterServer.PollHostList();
    }

    private void JoinServer(HostData hostData) {
        Network.Connect(hostData);
    }

    void OnConnectedToServer() {
        Debug.Log("Server Joined");
		SpawnPlayer();
    }

	private void SpawnPlayer() {
		Network.Instantiate(playerPrefab, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
	}

    void OnGUI() {
        if (!Network.isClient && !Network.isServer) {
            if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
                StartServer();

            if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts"))
                RefreshHostList();

            if (hostList != null) {
                for (int i = 0; i < hostList.Length; i++) {
                    if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
                        JoinServer(hostList[i]);
                }
            }
        }
    }

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
