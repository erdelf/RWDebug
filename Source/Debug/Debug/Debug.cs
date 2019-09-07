using Harmony;
using RimWorld;
using System;
using Verse;

namespace Debug
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using JetBrains.Annotations;
    using RimWorld.Planet;
    using UnityEngine;
    using Object = System.Object;

    [StaticConstructorOnStartup]
    public class Debug
    {
        static Debug()
        {
            // HarmonyInstance harmony = HarmonyInstance.Create("rimworld.erdelf.debug");

            string   path   = Path.Combine(Path.Combine(LoadedModManager.RunningMods.First(mcp => mcp.assemblies.loadedAssemblies.Contains(typeof(Debug).Assembly)).RootDir, "Ressources"), "fonts");
            Font font = AssetBundle.LoadFromFile(path).LoadAllAssets<Font>()[0];

            Log.Message(font?.GetType()?.FullName);
            Log.Message(font?.name);

            foreach (GUIStyle fontStyle in Text.fontStyles.Concat(Text.textFieldStyles).Concat(Text.textAreaStyles))
            {
                fontStyle.font = font;
            }

            /*
            harmony.Patch(AccessTools.Method(typeof(Alert_Thought), "AffectedPawns"), prefix: new HarmonyMethod(typeof(Debug), nameof(prefix)));
            HarmonyInstance.DEBUG = true;
            harmony.Patch(AccessTools.Method(typeof(PlayerItemAccessibilityUtility), "CacheAccessibleThings"), transpiler: new HarmonyMethod(typeof(Debug), nameof(transpiler)));
            HarmonyInstance.DEBUG = false;
            */
        }
    }

    public class DummyDef : ThingDef
    {
        public DummyDef() : base()
        {
            //HarmonyInstance harmony = HarmonyInstance.Create("rimworld.erdelf.dummy_checker");
            //harmony.Patch(AccessTools.Method(typeof(DefDatabase<ThingDef>), "AddAllInMods"), transpiler: new HarmonyMethod(typeof(Debug), nameof(Debug.Transpiler)));
        }


        public static void CheckDefName(ModMetaData __instance)
        {
            Log.Message(text: __instance.Name);
        }
    }
}