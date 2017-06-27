using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;
using System;

public class ServerAddPlayerSystem : ReactiveSystem<NetworkEntity> {

    readonly GameContext gameCtx;
    readonly NetworkContext netCtx;

	public ServerAddPlayerSystem(Contexts ctxs) : base(ctxs.network) {
        gameCtx = ctxs.game;
        netCtx = ctxs.network;
    }

    protected override void Execute(List<NetworkEntity> entities) {
        foreach (var e in entities) {
            var conn = e.connectionStart.conn;

            var entity = gameCtx.CreateEntity();
            entity.AddFrameSync(0);
            entity.AddPlayer(conn.playerMetadata);

            conn.playerView = entity;
        }
    }

    protected override bool Filter(NetworkEntity entity) {
        return entity.hasConnectionStart;
    }

    protected override ICollector<NetworkEntity> GetTrigger(IContext<NetworkEntity> context) {
        return context.CreateCollector(NetworkMatcher.ConnectionStart);
    }
}

public class ServerRemovePlayerSystem : ReactiveSystem<NetworkEntity> {

	public ServerRemovePlayerSystem(Contexts ctxs) : base(ctxs.network) { }

    protected override void Execute(List<NetworkEntity> entities) {
        foreach (var ent in entities) {
            ent.connectionEnd.conn.playerView.Destroy();
        }
    }

    protected override bool Filter(NetworkEntity entity) {
        return entity.hasConnectionEnd;
    }

    protected override ICollector<NetworkEntity> GetTrigger(IContext<NetworkEntity> context) {
        return context.CreateCollector(NetworkMatcher.ConnectionEnd);
    }
}

public class ServerPlayerFeature : Feature {

	public ServerPlayerFeature(Contexts ctxs) : base("Server Player") {
        Add(new ServerAddPlayerSystem(ctxs));
        Add(new ServerRemovePlayerSystem(ctxs));
	}
}
