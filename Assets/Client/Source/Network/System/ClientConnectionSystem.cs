using System;
using System.Collections;
using System.Collections.Generic;
using Entitas;
using State = ClientConnectionComponent.State;
using UnityEngine;

public class ClientConnectionSystem : IExecuteSystem {
	const float HandshakeTimeout = 2.0f;

	const int MaxRetryTime = 4;

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

		if (recvPacketGroup.count != 0) {
			conn.packetUnreceivedTime = 0;
		}

		switch (conn.state) {
		case State.Disconnected: {
			if (startConnectionGroup.count != 0) {
				var info = startConnectionGroup.GetSingleEntity().clientStartConnection;

				conn.serverEP = info.serverEP;
				conn.state = State.Initalize;
				// Send Sync packet to server
				ctx.CreateEntity()
					.AddSendPacket(conn.serverEP, new Packet(PacketType.Sync, new byte[0]));
			}
		} break;
		case State.Initalize: {
			if (conn.timeoutCounter > HandshakeTimeout) {
				if (conn.retryTime > MaxRetryTime) {
					conn.state = State.Initalize;
				} else {
					// Send Sync packet to server
					ctx.CreateEntity()
						.AddSendPacket(conn.serverEP, new Packet(PacketType.Sync, new byte[0]));
					++conn.retryTime;
				}
			}
		} break;
		case State.Connected: {

		} break;
		case State.TearDown: {

		} break;
		} 
	}

}