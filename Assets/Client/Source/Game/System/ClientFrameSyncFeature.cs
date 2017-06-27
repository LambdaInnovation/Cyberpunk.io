using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;
using System.IO;

public class FrameRecvSystem : IExecuteSystem {
	GameContext gameCtx;
	NetworkContext networkCtx;

	IGroup<NetworkEntity> packetGroup;

	public FrameRecvSystem(Contexts ctxs) {
		gameCtx = ctxs.game;
		networkCtx = ctxs.network;
		
		packetGroup = networkCtx.GetGroup(NetworkMatcher.RecvPacket);
	}

	public void Execute() {
		var framePackets = RecvFramePackets();
		foreach (var framePacket in framePackets) {
			ApplyToGame(framePacket);
		}
	}
	
	List<FramePacket> RecvFramePackets() {
		List<FramePacket> ret = new List<FramePacket>();
		foreach (var packetEnt in packetGroup.GetEntities()) {
			var recv = packetEnt.recvPacket;
			if (recv.packet.packetType == PacketType.Frame) {
				using (var stream = new MemoryStream(recv.packet.data)) {
					var reader = new BinaryReader(stream);
					var framePacket = new FramePacket();
					framePacket.Read(reader);
					ret.Add(framePacket);
				}
			}
		}
		return ret;
	}

	void ApplyToGame(FramePacket packet) {
		Debug.Log("Recv " + packet);
	}
}

public class ClientFrameSyncFeature : Feature {

	public ClientFrameSyncFeature(Contexts ctxs) : base("Client Frame Sync") {
		Add(new FrameRecvSystem(ctxs));
	}

}