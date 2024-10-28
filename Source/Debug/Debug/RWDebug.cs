using RimWorld;
using Verse;

namespace Debug
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Security.Policy;
    using HarmonyLib;
    using KCSG;
    using RimWorld.Planet;
    using RimWorld.QuestGen;
    using UnityEngine;
    using UnityEngine.Scripting;
    using Verse.AI;
    using Object = System.Object;

    [StaticConstructorOnStartup]
    public class RWDebug
    {
        static RWDebug()
        {
            //AccessTools.Field(typeof(HealthCardUtility), "showAllHediffs").SetValue(null, true);
            Harmony harmony = new Harmony("rimworld.erdelf.debug");
            //Harmony.DEBUG = true;

            //Log.Message(string.Join("\n", DefDatabase<PawnKindDef>.AllDefs.Select(pkd => pkd.RaceProps).Where(rp => rp.IsMechanoid).Select(rp => rp.body).Distinct().Select(body => $"{body.defName}: {string.Join(" | ", body.AllPartsVulnerableToFrostbite.Select(bpr => bpr.Label))}")));

            harmony.Patch(AccessTools.Method(typeof(DebugThingPlaceHelper), nameof(DebugThingPlaceHelper.IsDebugSpawnable)), prefix: new HarmonyMethod(typeof(RWDebug), nameof(Prefix)));


            //Log.Message(DefDatabase<PreceptDef>.GetNamed("TreeCutting_Horrible").comps.OfType<PreceptComp_KnowsMemoryThought>().Join(pc => pc.eventDef.defName, "\n"));


            //Log.Message(SolidBioDatabase.allBios.Join(pb => pb.name.ToStringFull + ": " + pb.childhood.skillGains.Join(sg => sg.skill.defName + ": " + sg.amount, ",") + " | " + pb.adulthood.skillGains.Join(sg => sg.skill.defName + ": " + sg.amount, ","), "\n"));
        }

        public static HashSet<string> stacktraces = new HashSet<string>();
        public static int             callCount = 0;

        public static void Prefix(ThingDef def)
        {
            Log.ResetMessageCount();
            Log.Message("--------------------------------------------");
            Log.Message(def?.modContentPack?.Name);
            Log.Message(def?.modContentPack?.FolderName);
            Log.Message(def?.defName);
            Log.Message(def?.category.ToString());
        }
    }


    public class DummyDef : ThingDef
    {
        public DummyDef() : base()
        {
            //Log.Message("Patch");
            //Harmony harmony = new Harmony("rimworld.erdelf.dummy_checker");
            //harmony.Patch(typeof(ThingDef).GetMethods(AccessTools.all).First(mi => mi.HasAttribute<CompilerGeneratedAttribute>() && mi.Name.Contains("PostLoad")), prefix: new HarmonyMethod(typeof(RWDebug), nameof(RWDebug.Prefix)));
            //harmony.Patch(AccessTools.Method(AccessTools.TypeByName("KCSG.StartupActions"), "AddDef"), prefix: new HarmonyMethod(typeof(RWDebug), nameof(Prefix)));
        }

        public static void Prefix()
        {
            string stackTrace = StackTraceUtility.ExtractStackTrace();
            RWDebug.stacktraces.Add(stackTrace);
            RWDebug.callCount++;
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
