using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;

public class ClientConnectionTest : MonoBehaviour {
	Thread thread;
	UdpClient udpSend, udpReceive;

	void Start () {
		udpSend = new UdpClient();
		udpReceive = new UdpClient();
		
		thread = new Thread(new ThreadStart(ThreadFunc));
		thread.Start();
	}
	
	void OnApplicationQuit() {
		thread.Abort();
		udpSend.Close();
		udpReceive.Close();
	}

	void ThreadFunc() {
		while (true) {
			var ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
			udpSend.Connect(ep);
			udpSend.Send(new byte[] { 1, 2, 3, 4, 5, 4, 3, 2, 1 }, 9);
			print("Send data");
			
			var receivedData = udpSend.Receive(ref ep);

			string str = "Received from " + ep + ", ";
			foreach (var b in receivedData) {
				str += b.ToString() + ",";
			}

			Debug.Log(str);

			Thread.Sleep(500);
		}
		
	}
}
