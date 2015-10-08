using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour {
	
	private const string typeName = "UniqueGameName";
	private const string gameName = "P2 Networking";
	
	public int thisPlayer;
	public GameObject playerPrefab;
	public GameObject kinectPrefab;
	public GameObject target;
	public static int playerCount = Network.connections.Length;
	//int state
	public static NetworkPlayer[] players = new NetworkPlayer[4];
	public static int playerID;
	private HostData[] hostList;
	
	private void StartServer()
	{
		Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
	}
	// Use this for initialization
	void OnServerInitialized()
	{
		Debug.Log("Server Initializied");
	}
	void OnConnectedToServer()
	{
		
	}
	void OnPlayerConnected(NetworkPlayer player) {
		playerID = int.Parse(player.ToString());
		Debug.Log("Player " + playerID + " connected from " + player.ipAddress + ":" + player.port);
		Debug.Log("Amount of players connected is " + Network.connections.Length);
	}
	
	void OnGUI()
	{
		if (!Network.isClient && !Network.isServer)
		{
			if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
				StartServer();
			
			if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts"))
				RefreshHostList();
			
			if (hostList != null)
			{
				for (int i = 0; i < hostList.Length; i++)
				{
					if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
						JoinServer(hostList[i]);
				}
			}
		}
	}
	
	private void RefreshHostList()
	{
		MasterServer.RequestHostList(typeName);
	}
	
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}
	
	private void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	}
	
}

