using Harmony;
using RimWorld;
using System;
using Verse;

namespace Debug
{
    using System.Collections.Generic;
    using System.ComponentModel;
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
             HarmonyInstance harmony = HarmonyInstance.Create("rimworld.erdelf.debug");



             HarmonyInstance.DEBUG = true;
            harmony.Patch(AccessTools.Property(typeof(Text), nameof(Text.Font)).GetSetMethod(), prefix: new HarmonyMethod(typeof(Debug), nameof(Prefix)));

        }

        public static void Prefix(ref GameFont value)
        {
            value = GameFont.Medium;
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