using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hammerplay.Utils {
	public class CommandLineReader : MonoBehaviour {

		string[] args = System.Environment.GetCommandLineArgs ();

		string input = "";

		private static CommandLineReader _instance;

		private void Awake () {
			DontDestroyOnLoad (this);
			_instance = this;

			for (int i = 0; i < args.Length; i++) {
				ProcessServerArgument (args[i]);
				ProcessRoomId (args[i]);
				ProcessIpAddressArgument (args[i]);
				ProcessUsernameArgument (args[i]);
			}

			//Application.targetFrameRate = 60;
		}

		private bool isServer = false;

		public static bool IsServer {
			get { 
				if (_instance == null)
					return false;
					
				return _instance.isServer && _instance.hasRoomId; 
			}
		}

		private bool hasRoomId = false;

		private int roomId = -1;

		public static int RoomId {
			get { return _instance.roomId; }
		}

		private string[] actionArguments = new string[] { "--server", "--roomId" };

		private void ProcessServerArgument (string argument) {
			if (argument.Contains ("--server"))
				isServer = true;
		}

		private void ProcessRoomId (string argument) {
			string[] roomIdArguments = argument.Split ('=');

			if (roomIdArguments.Length == 2) {

				int roomId = 0;
				bool isNumeric = int.TryParse (roomIdArguments[1], out roomId);

				if (roomIdArguments[0] == "--roomId" && isNumeric) {
					this.hasRoomId = true;
					this.roomId = roomId;
				}
			}
		}

		private bool hasIpAddress = false;

		public static bool HasIpAddress {
			get {
				if (_instance == null)
				return false;

				return _instance.hasIpAddress;
			}
		}

		private string ipAddress = "localhost";

		public static string IpAddress {
			get { return _instance.ipAddress;}
		}

        

        private void ProcessIpAddressArgument (string argument) {
			string[] ipAddressArguments = argument.Split ('=');

			if (ipAddressArguments.Length == 2) {

				string ipAddress = ipAddressArguments[1];
				
				if (ipAddressArguments[0] == "--ipAddress") {
					this.hasIpAddress = true;
					this.ipAddress = ipAddress;
				}
			}
		}


		private bool hasUsername = false;
		public static bool HasUsername { get {
			if (_instance == null)
			return false;

			return _instance.hasUsername;
		}}

		private string username;
        public static string Username { get {
			return _instance.username;
		}}

		private void ProcessUsernameArgument (string argument) {
			string[] usernameArguments = argument.Split ('=');

			if (usernameArguments.Length == 2) {

				string username = usernameArguments[1];
				
				if (usernameArguments[0] == "--username") {
					this.hasUsername = true;
					this.username = username;
				}
			}
		}
	}
}