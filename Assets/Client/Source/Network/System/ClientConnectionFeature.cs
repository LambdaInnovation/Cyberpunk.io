using System;
using System.Collections;
using System.Collections.Generic;
using Entitas;
using State = ClientConnectionComponent.State;
using UnityEngine;
using System.IO;
using System.Linq;

public class ClientConnectionSystem : IExecuteSystem {
	const float HandshakeTimeout = 2.0f,
		ConnectionTimeOut = 3.0f,
		TearDownTimeOut = 2.0f,
		KeepAliveTimeOut = 1f;

	const int MaxTearDownRetry = 3,
			  MaxInitializeRetry = 3;

	IGroup<NetworkEntity> connectionGroup;
	IGroup<NetworkEntity> startConnectionGroup;
	IGroup<NetworkEntity> recvPacketGroup;

	NetworkContext ctx;

	public ClientConnectionSystem(Contexts ctxs) {
		ctx = ctxs.network;
		connectionGroup = ctx.GetGroup(NetworkMatcher.ClientConnection);
		startConnectionGroup = ctx.GetGroup(NetworkMatcher.ClientStartConnection);
		recvPacketGroup = ctx.GetGroup(NetworkMatcher.RecvPacket);
	}

	public void Execute() {
		var entity = connectionGroup.GetSingleEntity();
		var conn = entity.clientConnection;

		conn.packetUnreceivedTime += Time.deltaTime;
		conn.stateTime += Time.deltaTime;
		conn.timeoutCounter += Time.deltaTime;
		conn.keepAliveCounter += Time.deltaTime;

		if (recvPacketGroup.count != 0) {
			conn.packetUnreceivedTime = 0;
		}

		var myPackets = recvPacketGroup.GetEntities()
			.Select(e => e.recvPacket);

		switch (conn.state) {
		case State.Disconnected: {
			if (startConnectionGroup.count != 0) {
				var startConnection = startConnectionGroup.GetSingleEntity();
				var info = startConnection.clientStartConnection;

				conn.serverEP = info.serverEP;
				conn.playerMetadata = info.playerMetadata;
				conn.state = State.Initalize;

				SendSyncPacket(conn);

				startConnection.Destroy();
			}
		} break;
		case State.Initalize: {
			// Resend Sync
			if (conn.timeoutCounter > HandshakeTimeout) {
				if (conn.retryTime > MaxInitializeRetry) {
					conn.state = State.Disconnected;
				} else {
					SendSyncPacket(conn);
					++conn.retryTime;
				}

				conn.timeoutCounter = 0;
			}

			// React to SyncAck packet
			var syncAckPackets = myPackets
				.Where(e => e.packet.packetType == PacketType.SyncAck)
				.Count();

			if (syncAckPackets > 0) {
				conn.state = State.Connected;
				// Send Ack
				SendAckPacket(conn);
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
					.AddSendPacket(conn.serverEP, new Packet(PacketType.KeepAlive, new byte[0]));
			}

			// Handle Fin
			var finPackets = myPackets
				.Where(recv => recv.packet.packetType == PacketType.Fin)
				.Count();
			if (finPackets > 0) {
				ctx.CreateEntity()
					.AddSendPacket(conn.serverEP, new Packet(PacketType.FinAck, new byte[0]));
				conn.state = State.Disconnected;
			}

		} break;
		case State.TearDown: {
			// Resend Fin
			if (conn.timeoutCounter > TearDownTimeOut) {
				if (conn.retryTime > MaxTearDownRetry) {
					conn.state = State.Disconnected;
				} else {
					++conn.retryTime;
					conn.timeoutCounter = 0;
					SendFinPacket(conn);
				}
			}

			// Handle FinAck
			var finAckPackets = myPackets.Where(recv => recv.packet.packetType == PacketType.FinAck).Count();

			if (finAckPackets > 0) {
				conn.state = State.Disconnected;
			}

		} break;
		} 
	}

	void SendSyncPacket(ClientConnectionComponent conn) {
		var stream = new MemoryStream();
		var writer = new BinaryWriter(stream);
		conn.playerMetadata.WriteBytes(writer);
		writer.Flush();
		var metadataBytes = stream.ToArray();
		ctx.CreateEntity()
			.AddSendPacket(conn.serverEP, new Packet(PacketType.Sync, metadataBytes));
	}

	void SendAckPacket(ClientConnectionComponent conn) {
		ctx.CreateEntity()
			.AddSendPacket(conn.serverEP, new Packet(PacketType.Ack, new byte[0]));
	}

	void StartTearDown(ClientConnectionComponent conn) {
		conn.state = State.TearDown;
		SendFinPacket(conn);
	}

	void SendFinPacket(ClientConnectionComponent conn) {
		ctx.CreateEntity()
			.AddComponent(
			NetworkComponentsLookup.SendPacket, 
			new SendPacketComponent{
				target = conn.serverEP,
				packet = new Packet(PacketType.Fin, new byte[0])
			});
	}

}

public class ClientConnectionFeature : Feature {

	public ClientConnectionFeature(Contexts ctxs) : base("Client Connection") {
		Add(new ClientConnectionSystem(ctxs));
	}

}