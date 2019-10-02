using System.Collections;
using System.Collections.Generic;
//using Doozy.Engine;
using ExitGames.Client.Photon;
using Hammerplay.Utils;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConnectionManager : MonoBehaviourPunCallbacks, IOnEventCallback {

	[SerializeField]
	private Button homeButton, retryButton, cancelButton;

	[SerializeField]
	private TextMeshProUGUI logText;

	public override void OnEnable () {
		PhotonNetwork.AddCallbackTarget (this);

		if (!CommandLineReader.IsServer) {
			homeButton.onClick.AddListener (Home);
			retryButton.onClick.AddListener (Retry);
			cancelButton.onClick.AddListener (Cancel);
		}
	}

	public override void OnDisable () {
		PhotonNetwork.RemoveCallbackTarget (this);

		homeButton.onClick.RemoveAllListeners ();
		retryButton.onClick.RemoveAllListeners ();
		cancelButton.onClick.RemoveAllListeners ();
	}

	private void Start () {
		//LogMessage (CommandLineReader.IsServer.ToString ());
		//GameEventMessage.SendEvent ("OnGameLoaded");
		
		PhotonNetwork.ConnectUsingSettings ();
		if (!CommandLineReader.IsServer) {
			homeButton.gameObject.SetActive (false);
			retryButton.gameObject.SetActive (false);
			cancelButton.gameObject.SetActive (true);
		} else {
			homeButton.gameObject.SetActive (false);
			retryButton.gameObject.SetActive (false);
			cancelButton.gameObject.SetActive (false);
		}
	}

	private void Home () {
		PhotonNetwork.Disconnect ();
		//GameEventMessage.SendEvent ("MainMenu");
		SceneManager.LoadScene ("Empty");
	}

	private void Retry () {
		PhotonNetwork.Disconnect ();
		PhotonNetwork.ConnectUsingSettings ();
	}

	private void Cancel () {
		LogMessage ("Disconnected");
		PhotonNetwork.Disconnect ();

		homeButton.gameObject.SetActive (true);
		retryButton.gameObject.SetActive (true);
		cancelButton.gameObject.SetActive (false);
	}

	public override void OnConnectedToMaster () {
		if (!CommandLineReader.IsServer) {
			LogMessage ("Connected to MasterServer");
			PhotonNetwork.JoinRandomRoom ();
		} else {
			LogMessage ("[SERVER] Connected to MasterServer");
			PhotonNetwork.JoinRoom (CommandLineReader.RoomId.ToString ());
		}
	}

	public override void OnCreatedRoom () {
		LogMessage ("Waiting for a player to join");
	}

	public override void OnJoinRandomFailed (short returnCode, string message) {

		LogMessage (string.Format ("Couldn't join any room", message));

		if (!CommandLineReader.IsServer) {
			var roomName = string.Format ("{0}", UnityEngine.Random.Range (10, 1000));
			PhotonNetwork.CreateRoom (roomName,
				new RoomOptions () {
					IsVisible = true,
						MaxPlayers = 3
				}, TypedLobby.Default);

			homeButton.gameObject.SetActive (true);
			retryButton.gameObject.SetActive (true);
			cancelButton.gameObject.SetActive (false);
		}
	}

	public override void OnJoinedRoom () {
		
		if (!CommandLineReader.IsServer) {
			homeButton.gameObject.SetActive (false);
			retryButton.gameObject.SetActive (false);
			cancelButton.gameObject.SetActive (true);
		} else {
			LogMessage ("[SERVER] Joined Room");
			// Once the server version of the game is in, we switch the master-client them, giving it authority.
			PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
		}
	}

	public override void OnPlayerEnteredRoom (Player newPlayer) {
		if (PhotonNetwork.IsMasterClient) {
			if (PhotonNetwork.CurrentRoom.PlayerCount == 3) {
				PhotonNetwork.CurrentRoom.IsVisible = false;
				LogMessage ("[SERVER] Starting the game");
				byte evCode = 0; // Custom Event 0: Used as "MoveUnitsToTargetPosition" event
				//object[] content = new object[] { new Vector3 (10.0f, 2.0f, 5.0f), 1, 2, 5, 10 }; // Array contains the target position and the IDs of the selected units
				RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
				SendOptions sendOptions = new SendOptions { Reliability = true };
				PhotonNetwork.RaiseEvent (evCode, null, raiseEventOptions, sendOptions);
			}
		}
	}

	/* public override void OnPlayerLeftRoom (Player otherPlayer) {
		base.OnPlayerLeftRoom(otherPlayer);
		Debug.LogFormat ("{0} is disconnected", otherPlayer);

		if (OnForfeit != null)
			OnForfeit (otherPlayer);
	}*/

	public void OnEvent (EventData photonEvent) {
		switch (photonEvent.Code) {
			case 0:
				StartGame ();
				break;
		}
	}

	private void LogMessage (string message) {
		logText.text = message;
		Debug.Log (message);
	}

	private void StartGame () {
		homeButton.gameObject.SetActive (false);
		retryButton.gameObject.SetActive (false);
		cancelButton.gameObject.SetActive (false);

		LogMessage ("Both the players are in, begin game");

		if (OnGameStart != null)
			OnGameStart ();

		gameObject.SetActive (false);
	}

	public delegate void StartGameHandler ();

	public static event StartGameHandler OnGameStart;

	public delegate void ForfeitHandler (Player player);

	public static event ForfeitHandler OnForfeit;

}