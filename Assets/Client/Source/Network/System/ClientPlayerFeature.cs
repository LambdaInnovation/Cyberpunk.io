using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;
using System;

public class ClientPlayerCreateSytstem : ReactiveSystem<GameEntity> {
	PlayerSettings settings;

	public ClientPlayerCreateSytstem(Contexts ctxs, PlayerSettings settings) : base(ctxs.game) {
		this.settings = settings;
	}

    protected override void Execute(List<GameEntity> entities) {
        foreach (var ent in entities) {
			var player = ent.player;
			ent.AddView(null);
			ent.AddSprite(settings.playerSprites[0], false, null);
		}
    }

    protected override bool Filter(GameEntity entity) {
        return entity.hasPlayer;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) {
        return context.CreateCollector(GameMatcher.Player);
    }
}

public class ClientPlayerFeature : Feature {

	public ClientPlayerFeature(Contexts ctxs, PlayerSettings playerSettings) : base("Client Player") {
		Add(new ClientPlayerCreateSytstem(ctxs, playerSettings));
	}
}
