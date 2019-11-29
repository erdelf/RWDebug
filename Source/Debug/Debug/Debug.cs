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
    using TMPro;
    using UnityEngine;
    using Object = System.Object;

    [StaticConstructorOnStartup]
    public class Debug
    {
        static Debug()
        {
            // HarmonyInstance harmony = HarmonyInstance.Create("rimworld.erdelf.debug");


            string basePath = Path.Combine(LoadedModManager.RunningMods.First(mcp => mcp.assemblies.loadedAssemblies.Contains(typeof(Debug).Assembly)).RootDir, "Ressources");
            Font font = AssetBundle.LoadFromFile(Path.Combine(basePath, "fonts")).LoadAllAssets<Font>()[0];

            AssetBundle fontMesh = AssetBundle.LoadFromFile(Path.Combine(basePath, "fontsmesh"));

            Material mat = fontMesh.LoadAllAssets<Material>()[0];
            mat.mainTexture = fontMesh.LoadAllAssets<Texture>()[0];



            TextMeshPro textMeshPro = WorldFeatureTextMesh_TextMeshPro.WorldTextPrefab.GetComponent<TextMeshPro>();
            textMeshPro.font.material = mat;
            textMeshPro.fontMaterial = mat;
            textMeshPro.fontSharedMaterial = mat;
            textMeshPro.SetMaterialDirty();

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