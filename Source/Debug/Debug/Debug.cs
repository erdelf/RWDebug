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
            harmony.Patch(AccessTools.Property(typeof(WorkGiverDef), nameof(WorkGiverDef.Worker)).GetGetMethod(), new HarmonyMethod(typeof(Debug), nameof(UsedPrefix)));

        }

        public static void UsedPrefix(WorkGiverDef __instance)
        {
            Log.Message(__instance.defName + " " + __instance.modContentPack.Name, true);
        }

        public static bool NamePrefix(ref IEnumerable<Name> __result)
        {
            __result = new[] {new NameSingle("erdelf"), new NameSingle("Mehni"), new NameSingle("Jecrell")};

            return false;
        }
    }


    public class DummyDef : ThingDef
    {
        /*
        public DummyDef() : base()
        {
            Log.Message("dummyCheck");
            HarmonyInstance.Create("rimworld.erdelf.dummy_checker").Patch(
                AccessTools.Method(typeof(PawnKindDef), nameof(ThingDef.ConfigErrors)),
                new HarmonyMethod(typeof(DummyDef), nameof(checkDefName)), null);
        }

        public static void checkDefName(PawnKindDef __instance)
        {
            Log.Message(__instance.defName + ": " + string.Join(" | ", __instance.weaponTags?.ToArray() ?? new string[] { "" }));
        }*/
    }
}