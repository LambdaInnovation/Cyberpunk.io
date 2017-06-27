using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;
using System;

public class CleanupSystem : ICleanupSystem {

	IGroup<GameEntity> gameGroup;
	IGroup<NetworkEntity> networkGroup;

	public CleanupSystem(Contexts ctxs) {
		gameGroup = ctxs.game.GetGroup(GameMatcher.Cleanup);
		networkGroup = ctxs.network.GetGroup(NetworkMatcher.Cleanup);
	}

	public void Cleanup() {
		Process(gameGroup.GetEntities());
		Process(networkGroup.GetEntities());
	}

	void Process(Array a) {
		foreach (object e in a) { ((IEntity) e).Destroy(); }
	}

}
