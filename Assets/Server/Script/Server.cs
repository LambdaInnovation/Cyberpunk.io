using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using UnityEngine;

using UnityEditor;

public class Server : MonoBehaviour {

	Thread thread;
	UdpClient udp;

	void Start () {
		thread = new Thread(new ThreadStart(ThreadFunc));
		thread.Start();
	}

	void OnApplicationQuit() {
		print("OnApplicationQuit");
		thread.Abort();
		if (udp != null)
			udp.Close();
	}

	void ThreadFunc() {
		udp = new UdpClient(12345);
		Debug.Log("Server thread starting");

		// udp.Client.Blocking = false;

		while (true) {
			var remoteEP = new IPEndPoint(IPAddress.Any, 12345);
			var data = udp.Receive(ref remoteEP);
			Debug.Log("receive data from " + remoteEP);
			string s = "";
			foreach (var d in data) {
				s += d.ToString();
			}
			Debug.Log(s);

			udp.Send(new byte[] { R(), R(), R() }, 3, remoteEP);
		}
	}

	static System.Random rand = new System.Random();

	static byte R() {
		return (byte) rand.Next(10);
	}

}
