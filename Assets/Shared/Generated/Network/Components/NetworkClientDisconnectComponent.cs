//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class NetworkEntity {

    static readonly ClientDisconnectComponent clientDisconnectComponent = new ClientDisconnectComponent();

    public bool isClientDisconnect {
        get { return HasComponent(NetworkComponentsLookup.ClientDisconnect); }
        set {
            if (value != isClientDisconnect) {
                if (value) {
                    AddComponent(NetworkComponentsLookup.ClientDisconnect, clientDisconnectComponent);
                } else {
                    RemoveComponent(NetworkComponentsLookup.ClientDisconnect);
                }
            }
        }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class NetworkMatcher {

    static Entitas.IMatcher<NetworkEntity> _matcherClientDisconnect;

    public static Entitas.IMatcher<NetworkEntity> ClientDisconnect {
        get {
            if (_matcherClientDisconnect == null) {
                var matcher = (Entitas.Matcher<NetworkEntity>)Entitas.Matcher<NetworkEntity>.AllOf(NetworkComponentsLookup.ClientDisconnect);
                matcher.componentNames = NetworkComponentsLookup.componentNames;
                _matcherClientDisconnect = matcher;
            }

            return _matcherClientDisconnect;
        }
    }
}