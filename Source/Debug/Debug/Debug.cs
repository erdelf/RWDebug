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
        
        public DummyDef() : base()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.erdelf.dummy_checker");
            

                harmony.Patch(AccessTools.Method(typeof(ModMetaData), "Init"), null,
                    new HarmonyMethod(typeof(DummyDef), nameof(checkDefName)));
        }


        public static void checkDefName(ModMetaData __instance)
        {
            Log.Message(__instance.Name);
        }
    }
}