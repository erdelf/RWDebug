using System.Collections.Generic;
using System.Linq;
using Harmony;
using RimWorld;
using UnityEngine;
using Verse;
using System.Reflection.Emit;
using System.Reflection;
using System;
using RimWorld.Planet;

namespace Debug
{
    using System.Text;
    using Harmony.ILCopying;
    using Verse.Sound;

    [StaticConstructorOnStartup]
    public class Debug
    {
        static Debug()
        {
            HarmonyInstance harmony = HarmonyInstance.Create(id: "rimworld.erdelf.debug");
            harmony.Patch(AccessTools.Method(typeof(HediffMaker), nameof(HediffMaker.MakeHediff)), new HarmonyMethod(typeof(Debug), nameof(Prefix)));

        }

        public static void Prefix(HediffDef td)
        {
            Log.Message($"Generating {td?.defName ?? "TD null"} from {td?.modContentPack?.Name ?? "Mod null"}", true);
        }
    }


    public class DummyDef : ThingDef
    {
        /*
        public DummyDef() : base()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.erdelf.dummy_checker");
            

                harmony.Patch(AccessTools.Method(typeof(ModMetaData), "Init"), null,
                    new HarmonyMethod(typeof(DummyDef), nameof(checkDefName)));
        }


        public static void checkDefName(ModMetaData __instance)
        {
            Log.Message(__instance.Name);
        }*/
    }
}