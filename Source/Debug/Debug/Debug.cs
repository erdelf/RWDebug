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
    using HarmonyLib;
    using JetBrains.Annotations;
    using RimWorld.Planet;
    using UnityEngine;
    using Object = System.Object;

    [StaticConstructorOnStartup]
    public class Debug
    {
        static Debug()
        {
             Harmony harmony = new Harmony("rimworld.erdelf.debug");



             //HarmonyInstance.DEBUG = true;
             harmony.Patch(AccessTools.Method(typeof(PawnApparelGenerator), "CanUsePair"), prefix: new HarmonyMethod(typeof(Debug), nameof(Debug.Prefix)));

        }

        public static void Prefix(ThingStuffPair pair, Pawn pawn)
        {
            Log.Message($"{pair.thing?.defName ?? "NO DEF"} {pair.thing?.modContentPack.PackageId ?? "NO MOD"}\n{pair.stuff?.defName ?? "NO STUFF"} {pair.stuff?.modContentPack.PackageId ?? "NO MOD"}\n{pawn != null} {pawn?.Name?.ToStringFull ?? "NO NAME"} {pawn?.def?.label}", true);
            Log.ResetMessageCount();
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