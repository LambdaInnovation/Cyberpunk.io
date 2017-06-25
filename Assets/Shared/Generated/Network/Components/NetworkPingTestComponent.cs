//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class NetworkEntity {

    public PingTestComponent pingTest { get { return (PingTestComponent)GetComponent(NetworkComponentsLookup.PingTest); } }
    public bool hasPingTest { get { return HasComponent(NetworkComponentsLookup.PingTest); } }

    public void AddPingTest(System.Net.IPEndPoint newTarget, int newTestIntervalMsec, float newCooldown) {
        var index = NetworkComponentsLookup.PingTest;
        var component = CreateComponent<PingTestComponent>(index);
        component.target = newTarget;
        component.testIntervalMsec = newTestIntervalMsec;
        component.cooldown = newCooldown;
        AddComponent(index, component);
    }

    public void ReplacePingTest(System.Net.IPEndPoint newTarget, int newTestIntervalMsec, float newCooldown) {
        var index = NetworkComponentsLookup.PingTest;
        var component = CreateComponent<PingTestComponent>(index);
        component.target = newTarget;
        component.testIntervalMsec = newTestIntervalMsec;
        component.cooldown = newCooldown;
        ReplaceComponent(index, component);
    }

    public void RemovePingTest() {
        RemoveComponent(NetworkComponentsLookup.PingTest);
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

    static Entitas.IMatcher<NetworkEntity> _matcherPingTest;

    public static Entitas.IMatcher<NetworkEntity> PingTest {
        get {
            if (_matcherPingTest == null) {
                var matcher = (Entitas.Matcher<NetworkEntity>)Entitas.Matcher<NetworkEntity>.AllOf(NetworkComponentsLookup.PingTest);
                matcher.componentNames = NetworkComponentsLookup.componentNames;
                _matcherPingTest = matcher;
            }

            return _matcherPingTest;
        }
    }
}