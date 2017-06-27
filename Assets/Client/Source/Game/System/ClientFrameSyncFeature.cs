using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;
using System.IO;

public class FrameRecvSystem : IExecuteSystem {
	GameContext gameCtx;
	NetworkContext networkCtx;

	IGroup<NetworkEntity> packetGroup;

	readonly Dictionary<int, GameEntity> syncedEntities = new Dictionary<int, GameEntity>();

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
		foreach (var entityData in packet.entities) {
			if (!syncedEntities.ContainsKey(entityData.entityID)) {
				var ent = gameCtx.CreateEntity();
				ent.AddFrameSync(entityData.entityID);
				syncedEntities.Add(entityData.entityID, ent);
			}
			var entity = syncedEntities[entityData.entityID];
			foreach (var compData in entityData.components) {
				var compID = compData.componentID;
				if (!entity.HasComponent(compID)) {
					var ctor = GameComponentsLookup.componentTypes[compID].GetConstructor(new System.Type[0]);
					entity.AddComponent(compID, (IComponent) ctor.Invoke(new object[0]));
				}

				var comp = (IFrameSyncComponent) entity.GetComponent(compID);
				using (var stream = new MemoryStream(compData.data)) {
					var reader = new BinaryReader(stream);
					comp.ReadBytes(reader);
				}
 			}
		}
	}
}

public class ClientFrameSyncFeature : Feature {

	public ClientFrameSyncFeature(Contexts ctxs) : base("Client Frame Sync") {
		Add(new FrameRecvSystem(ctxs));
	}

}