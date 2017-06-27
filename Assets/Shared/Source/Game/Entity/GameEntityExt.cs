using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;
using System;

public static class GameEntityExt {

	public struct ComponentAndID {
		public IFrameSyncComponent component;
		public int index;
	}

	static int[] syncComponentIndices;
	static GameEntityExt() {
		List<int> indices = new List<int>();
		for (int i = 0; i < GameComponentsLookup.TotalComponents; ++i) {
			if (GameComponentsLookup.componentTypes[i].FindInterfaces(TypeMatches, typeof(IFrameSyncComponent)).Length > 0) {
				indices.Add(i);
			}
		}

		syncComponentIndices = indices.ToArray();
	}

	public static ComponentAndID[] GetSyncComponents(this GameEntity e) {
		List<ComponentAndID> list = new List<ComponentAndID>();
		for (int i = 0; i < syncComponentIndices.Length; ++i) {
			var index = syncComponentIndices[i];
			if (e.HasComponent(i)) {
				list.Add(new ComponentAndID { 
					component = (IFrameSyncComponent) e.GetComponent(i), index = i 
				});
			}
		}
		return list.ToArray();
	}

	static bool TypeMatches(Type type, object criteria) {
		return type == criteria;
	}


}
