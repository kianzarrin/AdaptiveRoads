namespace AdvancedRoads.Patches.TMPE {
    using System.Reflection;
    using TrafficManager.Manager.Impl;
    using CSUtil.Commons;
    using AdvancedRoads;
    using AdvancedRoads.Util;
    using HarmonyLib;
    using ColossalFramework;

    [HarmonyPatch]
    public static class GetDefaultPedestrianCrossingAllowed {
        public static MethodBase TargetMethod() {
            return typeof(JunctionRestrictionsManager).
                GetMethod(nameof(JunctionRestrictionsManager.GetDefaultPedestrianCrossingAllowed));
        }

        public static bool Prefix(ushort segmentId, bool startNode, ref bool __result) {
            ushort nodeID = startNode ? segmentId.ToSegment().m_startNode : segmentId.ToSegment().m_endNode;
            NetNodeExt data = NetworkExtensionManager.Instance.buffer[nodeID];

            return PrefixUtils.HandleTernaryBool(
                data?.GetDefaultPedestrianCrossingAllowed(),
                ref __result);

        }
    }
}