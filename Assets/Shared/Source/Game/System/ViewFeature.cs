using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Entitas;

public class AddViewSystem : ReactiveSystem<GameEntity>, IInitializeSystem {

	Transform viewContainer;

	public AddViewSystem(Contexts ctxs) : base(ctxs.game) {}

    public void Initialize() {
        viewContainer = new GameObject("Views").transform;
    }

    protected override void Execute(List<GameEntity> entities)  {
		foreach (var ent in entities) {
			var go = new GameObject("Entity View");
			go.transform.SetParent(viewContainer, false);
			ent.view.gameObject = go;
		}
    }

    protected override bool Filter(GameEntity entity) {
        return entity.hasView;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) {
        return context.CreateCollector(GameMatcher.View);
    }
}

public class AddSpriteSystem : ReactiveSystem<GameEntity> {

	GameContext ctx;

	public AddSpriteSystem(Contexts ctxs): base(ctxs.game) {
		ctx = ctxs.game;
	}

    protected override void Execute(List<GameEntity> entities) {
        foreach (var ent in entities) {
			var sr = ent.view.gameObject.AddComponent<SpriteRenderer>();
			sr.sprite = ent.sprite.sprite;
			ent.sprite.spriteRenderer = sr;
		}
    }

    protected override bool Filter(GameEntity entity) {
        return entity.hasSprite;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) {
        return context.CreateCollector(GameMatcher.Sprite);
    }
}

public class UpdateSpriteSystem : IExecuteSystem {
	IGroup<GameEntity> spriteGroup;

	public UpdateSpriteSystem(Contexts ctxs) {
		spriteGroup = ctxs.game.GetGroup(GameMatcher.Sprite);
	}

	public void Execute() {
		foreach (var entity in spriteGroup.GetEntities()) {
			var comp = entity.sprite;
			if (comp.dynamic) {
				entity.sprite.spriteRenderer.sprite = entity.sprite.sprite;
			}
		}
	}

}

public class ViewFeature : Feature {

	public ViewFeature(Contexts ctxs) : base("View") {
		Add(new AddViewSystem(ctxs));
		Add(new AddSpriteSystem(ctxs));
		Add(new UpdateSpriteSystem(ctxs));
	}
	
}
