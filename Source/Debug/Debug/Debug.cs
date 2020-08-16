using RimWorld;
using System;
using Verse;

namespace Debug
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using HarmonyLib;
    using RimWorld.Planet;
    using UnityEngine;

    [StaticConstructorOnStartup]
    public class Debug
    {
        static Debug()
        {
            Harmony harmony = new Harmony("rimworld.erdelf.debug");

            foreach (WorldObjectDef worldObjectDef in DefDatabase<WorldObjectDef>.AllDefs.Where(wod => wod.texture.NullOrEmpty()))
            {
                worldObjectDef.texture = WorldObjectDefOf.RoutePlannerWaypoint.texture;
            }

            //Harmony.DEBUG = true;
            //harmony.Patch(AccessTools.PropertyGetter(typeof(Settlement), nameof(Settlement.Material)), prefix: new HarmonyMethod(typeof(Debug), nameof(Debug.Prefix)));
        }

        public static void Prefix(Settlement __instance)
        {
            Log.Message($"{__instance.Label} {__instance.Faction?.Name} {__instance.Faction?.def.defName} {__instance.Faction?.def.settlementTexturePath}");
            Log.ResetMessageCount();
        }
    }

    public class DummyDef : ThingDef
    {
        public DummyDef() : base()
        {
            //Log.Message("Patch");
            Harmony harmony = new Harmony("rimworld.erdelf.dummy_checker");
            //harmony.Patch(typeof(ThingDef).GetMethods(AccessTools.all).First(mi => mi.HasAttribute<CompilerGeneratedAttribute>() && mi.Name.Contains("PostLoad")), prefix: new HarmonyMethod(typeof(Debug), nameof(Debug.Prefix)));
        }

        public override void PostLoad()
        {
            base.PostLoad();
        }


        public static void CheckDefName(ModMetaData __instance)
        {
            Log.Message(text: __instance.Name);
        }
    }
}
