using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

// Component to mark that the entity needs to be frame-synced.
public class FrameSyncComponent : IComponent {

    public ushort entityID;

}
