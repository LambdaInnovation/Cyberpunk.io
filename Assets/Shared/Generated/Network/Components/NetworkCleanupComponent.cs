//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class NetworkEntity {

    static readonly CleanupComponent cleanupComponent = new CleanupComponent();

    public bool isCleanup {
        get { return HasComponent(NetworkComponentsLookup.Cleanup); }
        set {
            if (value != isCleanup) {
                if (value) {
                    AddComponent(NetworkComponentsLookup.Cleanup, cleanupComponent);
                } else {
                    RemoveComponent(NetworkComponentsLookup.Cleanup);
                }
            }
        }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityInterfaceGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class NetworkEntity : ICleanup { }

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class NetworkMatcher {

    static Entitas.IMatcher<NetworkEntity> _matcherCleanup;

    public static Entitas.IMatcher<NetworkEntity> Cleanup {
        get {
            if (_matcherCleanup == null) {
                var matcher = (Entitas.Matcher<NetworkEntity>)Entitas.Matcher<NetworkEntity>.AllOf(NetworkComponentsLookup.Cleanup);
                matcher.componentNames = NetworkComponentsLookup.componentNames;
                _matcherCleanup = matcher;
            }

            return _matcherCleanup;
        }
    }
}