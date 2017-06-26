using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Entitas;
using System.IO;
using UnityEngine;

using State = ServerConnectionComponent.State;

public class ServerConnectionSystem : IExecuteSystem {

	const float
		HandshakeTimeout = 2.0f,
		ConnectionTimeOut = 3.0f,
		TearDownTimeOut = 2.0f,
		KeepAliveTimeOut = 1.0f;
	
	const int MaxTearDownRetry = 3,
			  MaxInitializeRetry = 3;

	readonly NetworkContext ctx;

	readonly IGroup<NetworkEntity> serverConnGroup;
	readonly IGroup<NetworkEntity> packetGroup;

	public ServerConnectionSystem(Contexts ctxs) {
		ctx = ctxs.network;
		serverConnGroup = ctx.GetGroup(NetworkMatcher.ServerConnection);
		packetGroup = ctx.GetGroup(NetworkMatcher.RecvPacket);
	}

	public void Execute() {
		foreach (var entity in serverConnGroup.GetEntities()) {
			var conn = entity.serverConnection;
			var myPackets = packetGroup.GetEntities()
				.Select(e => e.recvPacket)
				.Where(packet => packet.source.Equals(conn.clientEP))
				.ToList();

			conn.packetUnreceivedTime += Time.deltaTime;
			conn.stateTime += Time.deltaTime;
			conn.timeoutCounter += Time.deltaTime;
			conn.keepAliveCounter += Time.deltaTime;
			
			if (myPackets.Count > 0) {
				conn.packetUnreceivedTime = 0;
			}

			var finAckPackets2 = myPackets.Where(recv => recv.packet.packetType == PacketType.FinAck).Count();
			if (finAckPackets2 > 0) {
				Debug.Log("FinAct at " + conn.state);
			}
		
			switch (conn.state) {
				case State.Initialize: {
					// Handle time-outs
					if (conn.timeoutCounter > HandshakeTimeout) {
						entity.Destroy();
					}

					// Handle Ack
					var ackPackets = myPackets.Where(packet => packet.packet.packetType == PacketType.Ack).Count();
					
					if (ackPackets > 0) {
						conn.state = State.Connected;
					}
					
				} break;
				case State.Connected: {
					// Handle time-outs
					if (conn.packetUnreceivedTime > ConnectionTimeOut) {
						StartTearDown(conn);
					}

					// KeepAlive
					if (conn.keepAliveCounter > KeepAliveTimeOut) {
						conn.keepAliveCounter = 0;

						ctx.CreateEntity()
							.AddSendPacket(conn.clientEP, new Packet(PacketType.KeepAlive, new byte[0]));
					}

					// Handle Fin
					var finPackets = myPackets
						.Where(recv => recv.packet.packetType == PacketType.Fin)
						.Count();
					if (finPackets > 0) {
						ctx.CreateEntity()
							.AddSendPacket(conn.clientEP, new Packet(PacketType.FinAck, new byte[0]));
						entity.Destroy();
					}

				} break;
				case State.TearDown: {
					// Resend Fin
					if (conn.timeoutCounter > TearDownTimeOut) {
						if (conn.retryTime > MaxTearDownRetry) {
							entity.Destroy();
						} else {
							++conn.retryTime;
							conn.timeoutCounter = 0;
							SendFinPacket(conn);
						}
					}

					// Handle FinAck
					var finAckPackets = myPackets.Where(recv => recv.packet.packetType == PacketType.FinAck).Count();

					if (finAckPackets > 0) {
						entity.Destroy();
					}

				} break;
			}
		}
	}

	void StartTearDown(ServerConnectionComponent conn) {
		conn.state = State.TearDown;
		SendFinPacket(conn);
	}

	void SendFinPacket(ServerConnectionComponent conn) {
		ctx.CreateEntity()
			.AddComponent(NetworkComponentsLookup.SendPacket, new SendPacketComponent{
				target = conn.clientEP,
				packet = new Packet(PacketType.Fin, new byte[0])
			});
	}
}

// Tracks Sync packets and create connection by need
public class ServerStartConnectionSystem : ReactiveSystem<NetworkEntity> {

	readonly IGroup<NetworkEntity> serverConnectionGroup;

	NetworkContext ctx;

	public ServerStartConnectionSystem(Contexts ctxs) : base(ctxs.network) {
		serverConnectionGroup = ctxs.network.GetGroup(NetworkMatcher.ServerConnection);

		ctx = Contexts.sharedInstance.network;
	}

    protected override void Execute(List<NetworkEntity> entities) {
        var connDict = new Dictionary<IPEndPoint, ServerConnectionComponent>();
		foreach (var sg in serverConnectionGroup.GetEntities()) {
			var comp = sg.serverConnection;
			connDict.Add(comp.clientEP, comp);
		}

		foreach (var entity in entities) {
			var comp = entity.recvPacket;
			if (connDict.ContainsKey(comp.source))
				continue;

			ServerConnectionComponent serverConn;
			connDict.TryGetValue(comp.source, out serverConn);

			// Start new ServerConnection
			if (serverConn == null) {
				serverConn = new ServerConnectionComponent();
				serverConn.clientEP = comp.source;
				serverConn.playerMetadata = new PlayerMetadata();

				using (var reader = new BinaryReader(
					new MemoryStream(comp.packet.data))) {
					Debug.Log("Packet length " + comp.packet.data.Length);
					serverConn.playerMetadata.ReadBytes(reader);
				}

				ctx.CreateEntity()
					.AddComponent(NetworkComponentsLookup.ServerConnection, serverConn);
			}
			
			// Send SyncAck packet to client
			ctx.CreateEntity()
				.AddComponent(NetworkComponentsLookup.SendPacket, new SendPacketComponent {
					target = comp.source,
					packet = new Packet(PacketType.SyncAck, new byte[0])
				});
		}
    }

    protected override bool Filter(NetworkEntity entity) {
        return entity.hasRecvPacket && entity.recvPacket.packet.packetType == PacketType.Sync;
    }

    protected override ICollector<NetworkEntity> GetTrigger(IContext<NetworkEntity> context) {
        return context.CreateCollector(NetworkMatcher.RecvPacket);
    }
}

public class ServerConnectionFeature : Feature {

	public ServerConnectionFeature(Contexts ctxs): base("Server Connection") {
		Add(new ServerConnectionSystem(ctxs));
		Add(new ServerStartConnectionSystem(ctxs));
	}

}