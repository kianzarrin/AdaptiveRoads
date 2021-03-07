namespace AdaptiveRoads.Patches.Segment {
    using HarmonyLib;
    using KianCommons;
    using KianCommons.Patches;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    [InGamePatch]
    [HarmonyPatch()]
    [HarmonyBefore("boformer.NetworkSkins")]
    public static class RenderInstance {
        public static MethodBase TargetMethod() {
            // private void NetSegment.RenderInstance(RenderManager.CameraInfo cameraInfo, ushort segmentID, int layerMask, NetInfo info, ref RenderManager.Instance data)
            var ret = typeof(NetSegment).GetMethod("RenderInstance", BindingFlags.NonPublic | BindingFlags.Instance); ;
            Assertion.Assert(ret != null, "did not manage to find original function to patch");
            return ret;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original) {
            try {
                var codes = TranspilerUtils.ToCodeList(instructions);
                CheckSegmentFlagsCommons.PatchCheckFlags(codes, original);
                Log.Info($"{ReflectionHelpers.ThisMethod} patched {original} successfully!");
                Log.Debug(codes.IL2STR());
                return codes;
            } catch(Exception e) {
                Log.Error(e.ToString());
                throw e;
            }
        }
    } // end class

    [HarmonyPatch()]
    public static class RenderInstanceOverlayPatch {
        public static MethodBase TargetMethod() {
            // private void NetSegment.RenderInstance(RenderManager.CameraInfo cameraInfo, ushort segmentID, int layerMask, NetInfo info, ref RenderManager.Instance data)
            var ret = typeof(NetSegment).GetMethod("RenderInstance", BindingFlags.NonPublic | BindingFlags.Instance); ;
            Assertion.Assert(ret != null, "did not manage to find original function to patch");
            return ret;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original) {
            try {
                var codes = TranspilerUtils.ToCodeList(instructions);
                SegmentOverlay.Patch(codes, original);
                Log.Info($"{ReflectionHelpers.ThisMethod} patched {original} successfully!");
                return codes;
            } catch(Exception e) {
                Log.Error(e.ToString());
                throw e;
            }
        }
    } // end class
} // end name space
