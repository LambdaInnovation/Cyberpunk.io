using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using Entitas;
using System;

public class NetworkingFeature : Feature {
	public UdpClient udp {
		get; private set;
	}

	public NetworkingFeature(Contexts ctxs) : base("Networking") {
		Add(new SendPacketSystem(ctxs, this));
		Add(new RecvPacketSystem(ctxs, this));
	}

	public override void Initialize() {
		udp = new UdpClient(NetworkConfig.port);
		base.Initialize();
	}

	public override void TearDown() {
		base.TearDown();
		udp.Close();
	}

}

public class RecvPacketSystem : IInitializeSystem, IExecuteSystem, ICleanupSystem {
	struct IncomingPacket {
		public IPEndPoint source;
		public Packet packet;
	}

	Thread thread;
	NetworkingFeature feature;

	Queue<IncomingPacket> incomingPackets = new Queue<IncomingPacket>();

	IGroup<NetworkEntity> recvGroup;

	public RecvPacketSystem(Contexts ctxs, NetworkingFeature feature) {
		this.feature = feature;
		recvGroup = ctxs.network.GetGroup(NetworkMatcher.RecvPacket);
	}

	public void Initialize() {
		Receive();
	}

	public void Execute() {
		IncomingPacket[] arr;
		lock (incomingPackets) {
			arr = incomingPackets.ToArray();
			incomingPackets.Clear();
		}

		var network = Contexts.sharedInstance.network;
		foreach (var inc in arr) {
			Debug.Log("Recv packet " + inc.packet.packetType + "@" + inc.source);
			network.CreateEntity()
				.AddRecvPacket(inc.source, inc.packet);
		}
	}

	public void Cleanup() {
		foreach (var entity in recvGroup.GetEntities()) {
			entity.Destroy();
		}
	}

	void Receive() {
		feature.udp.BeginReceive(ReceiveCallback, null);
	}

	void ReceiveCallback(IAsyncResult result) {
		var ep = new IPEndPoint(IPAddress.Any, 0);
		var data = feature.udp.EndReceive(result, ref ep);

		var incoming = new IncomingPacket {
			source = ep,
			packet = Packet.FromBytes(data)
		};

		lock (incomingPackets) {
			incomingPackets.Enqueue(incoming);
		}

		Receive();
	}

}


public class SendPacketSystem : ICleanupSystem {
	
	NetworkingFeature feature;
	IGroup<NetworkEntity> sendGroup;

	public SendPacketSystem(Contexts ctxs, NetworkingFeature feature) {
		this.feature = feature;
		sendGroup = ctxs.network.GetGroup(NetworkMatcher.SendPacket);
	}

	public void Cleanup() {
		foreach (var ent in sendGroup.GetEntities()) {
			var comp = ent.sendPacket;
			var packetData = comp.packet.ToBytes();
			feature.udp.Send(packetData, packetData.Length, comp.target);
			Debug.Log("Send Packet " + comp.packet);

			ent.Destroy();
		}
	}
}
