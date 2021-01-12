namespace AdaptiveRoads.LifeCycle {
    using AdaptiveRoads.Manager;
    using AdaptiveRoads.Patches;
    using AdaptiveRoads.UI.RoadEditor;
    using CitiesHarmony.API;
    using ICities;
    using KianCommons;
    using System;
    using System.Diagnostics;
    using UnityEngine.SceneManagement;
    using Newtonsoft.Json;
    using UnityEngine;
    using KianCommons.Serialization;

    public static class LifeCycle {
        public static string HARMONY_ID = "CS.Kian.AdaptiveRoads";
        public static string HARMONY_ID_MANUAL = "CS.Kian.AdaptiveRoads.Manual";

        public static SimulationManager.UpdateMode UpdateMode => SimulationManager.instance.m_metaData.m_updateMode;
        public static LoadMode Mode => (LoadMode)UpdateMode;
        public static string Scene => SceneManager.GetActiveScene().name;

        public static bool Loaded;
        public static bool bHotReload = false;
        const bool fastTestHarmony = false;

        public static void Enable() {
            try {
                Log.Debug("Testing StackTrace:\n" + new StackTrace(true).ToString(), copyToGameLog: false);
                KianCommons.UI.TextureUtil.EmbededResources = false;
                HelpersExtensions.VERBOSE = false;
                Loaded = false;

                HarmonyHelper.EnsureHarmonyInstalled();
                //LoadingManager.instance.m_simulationDataReady += SimulationDataReady; // load/update data
                LoadingManager.instance.m_levelPreLoaded += Preload;
                if (LoadingManager.instance.m_loadingComplete)
                    HotReload();

                if (fastTestHarmony) {
                    HarmonyHelper.DoOnHarmonyReady(() => {
                        HarmonyUtil.InstallHarmony(HARMONY_ID_MANUAL);
                        HarmonyUtil.InstallHarmony(HARMONY_ID);
                    });
                }

                var foo = new subclass.Foo { x = 1, y = new[] { 1, 2, 3 } ,v=new Vector3(1,2,3)};
                string data = JsonConvert.SerializeObject(foo, JsonUtil.Settings);
                Log.Debug(data);
                var foo2 = JsonConvert.DeserializeObject<subclass.Foo2>(data, JsonUtil.Settings);
                Log.Debug("v=" + foo2.v.ToSTR());
            } catch (Exception ex) {
                Log.Exception(ex);
            }
        }

        public class subclass {
            public class Foo {
                public int x;
                public int[] y;
                public Vector3 v;
            }

            //[XmlRoot("Foo")]
            public class Foo2 {
                public int x;
                public Vector3 v;
                public int[] y;
            }
        }


        public static void HotReload() {
            bHotReload = true;
            Preload();
            //SimulationDataReady();
            Load();
        }


        public static void Disable() {
            //LoadingManager.instance.m_simulationDataReady -= SimulationDataReady;
            LoadingManager.instance.m_levelPreLoaded -= Preload;
            Unload(); // in case of hot unload
            Exit();
            if (fastTestHarmony) {
                HarmonyUtil.UninstallHarmony(HARMONY_ID);
                HarmonyUtil.UninstallHarmony(HARMONY_ID_MANUAL);
            }
        }

        public static void Preload() {
            Log.Info("LifeCycle.Preload() called");
            if (!HideCrosswalksPatch.patched && PluginUtil.GetHideCrossings().IsActive()) {
                HarmonyUtil.ManualPatch(typeof(HideCrosswalksPatch), HARMONY_ID_MANUAL);
                HideCrosswalksPatch.patched = true;
            }
            HelpersExtensions.VERBOSE = false;
        }

        public static void Load() {
            try {
                Log.Info("LifeCycle.Load() called");
                Log.Info("testing stack trace:\n" + Environment.StackTrace, false);

                NetworkExtensionManager.Instance.OnLoad();
                HarmonyUtil.InstallHarmony(HARMONY_ID);
                NetInfoExtionsion.EnsureExtended_EditedNetInfos();
                HintBox.Create();
                Log.Info("LifeCycle.Load() successfull!");

            } catch (Exception e) {
                Log.Error(e.ToString() + "\n --- \n");
                throw e;
            }
        }

        public static void Unload() {
            Log.Info("LifeCycle.Release() called");
            HintBox.Release();
            HarmonyUtil.UninstallHarmony(HARMONY_ID);
            NetworkExtensionManager.Instance.OnUnload();
        }

        public static void Exit() {
            Log.Info("LifeCycle.Exit() called");
            HarmonyUtil.UninstallHarmony(HARMONY_ID_MANUAL);
            HideCrosswalksPatch.patched = false;
        }
    }
}
