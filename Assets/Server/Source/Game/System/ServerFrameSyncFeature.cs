using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;
using System.IO;
using System;

public class FrameSendSystem : IExecuteSystem {

	GameContext gameCtx;
	NetworkContext networkCtx;

	IGroup<GameEntity> syncedEntities;
	IGroup<NetworkEntity> serverConnectionGroup;

	public FrameSendSystem(Contexts ctxs) {
		gameCtx = ctxs.game;
		networkCtx = ctxs.network;

		syncedEntities = gameCtx.GetGroup(GameMatcher.FrameSync);
		serverConnectionGroup = networkCtx.GetGroup(NetworkMatcher.ServerConnection);
	}

	public void Execute() {
		var framePacket = BuildFramePacket();
	
		byte[] packetData;
		using (var stream = new MemoryStream()) {
			var writer = new BinaryWriter(stream);
			framePacket.Write(writer);
			packetData = stream.ToArray();
		}

		var packet = new Packet(PacketType.Frame, packetData);

		foreach (var connEntity in serverConnectionGroup.GetEntities()) {
			var conn = connEntity.serverConnection;
			if (conn.state == ServerConnectionComponent.State.Connected) {
				networkCtx.CreateEntity()
					.AddSendPacket(conn.clientEP, packet);
			}
		}
	}

	FramePacket BuildFramePacket() {
		FramePacket packet = new FramePacket();

		var entities = syncedEntities.GetEntities();
		packet.entities = new EntitySyncData[entities.Length];
		
		int eidx = 0;
		foreach (var ent in syncedEntities.GetEntities()) {
			var syncData = new EntitySyncData();
			packet.entities[eidx] = syncData;

			syncData.entityID = ent.frameSync.entityID;

			var syncComponents = ent.GetSyncComponents();
			syncData.components = new ComponentSyncData[syncComponents.Length];
			for (int j = 0; j < syncComponents.Length; ++j) {
				var pair = syncComponents[j];
				var compData = new ComponentSyncData();
				syncData.components[j] = compData;

				compData.componentID = (byte) pair.index;

				using (var stream = new MemoryStream()) {
					var writer = new BinaryWriter(stream);
					pair.component.WriteBytes(writer);
					compData.data = stream.ToArray();
				}
			}

			++eidx;
		}

		return packet;
	}

}

public class AssignEntityIDSystem : ReactiveSystem<GameEntity>, IInitializeSystem {
	GameContext ctx;
	ServerSyncInfoComponent syncInfo;

	public AssignEntityIDSystem(Contexts ctxs) : base(ctxs.game) {
		ctx = ctxs.game;
	}

    public void Initialize() {
        var entity = ctx.CreateEntity();
		entity.AddServerSyncInfo(0);
		syncInfo = entity.serverSyncInfo;
    }

    protected override void Execute(List<GameEntity> entities) {
        foreach (var ent in entities) {
			ent.frameSync.entityID = syncInfo.lastEntityID++;
		}
    }

    protected override bool Filter(GameEntity entity) {
        return entity.hasFrameSync;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) {
        return context.CreateCollector(GameMatcher.FrameSync);
    }
}

public class ServerFrameSyncFeature : Feature {

	public ServerFrameSyncFeature(Contexts ctxs) : base("Server Frame Sync") {
		Add(new FrameSendSystem(ctxs));
		Add(new AssignEntityIDSystem(ctxs));
	}

}