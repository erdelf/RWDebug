﻿using RimWorld;
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
            //Harmony harmony = new Harmony("rimworld.erdelf.debug");
            //Harmony.DEBUG = true;

            //Log.Message(string.Join("\n", DefDatabase<PawnKindDef>.AllDefs.Select(pkd => pkd.RaceProps).Where(rp => rp.IsMechanoid).Select(rp => rp.body).Distinct().Select(body => $"{body.defName}: {string.Join(" | ", body.AllPartsVulnerableToFrostbite.Select(bpr => bpr.Label))}")));

            //harmony.Patch(AccessTools.Method(typeof(DefDatabase<SymbolDef>), nameof(DefDatabase<SymbolDef>.Add)), prefix: new HarmonyMethod(typeof(RWDebug), nameof(Prefix)));

            Dictionary<string, int> count = new Dictionary<string, int> { { "unclear origin", 0 } };
            foreach (SymbolDef symbolDef in DefDatabase<SymbolDef>.AllDefsListForReading)
            {
                try
                {
                    string name = symbolDef.modContentPack.Name;

                    if (!count.ContainsKey(name))
                        count.Add(name, 0);
                    count[name]++;
                }
                catch (Exception)
                {
                    count["unclear origin"]++;
                }
            }

            Log.Message("SYMBOLDEF debug");
            Log.Message(string.Join("\n", count.OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key + ": " + kvp.Value)));

            Log.Message(string.Join("\n", stacktraces));
        }

        public static HashSet<string> stacktraces = new HashSet<string>();
        public static int             callCount = 0;

        public static void Prefix()
        {
            
        }
    }

    public class DummyDef : ThingDef
    {
        public DummyDef() : base()
        {
            //Log.Message("Patch");
            Harmony harmony = new Harmony("rimworld.erdelf.dummy_checker");
            //harmony.Patch(typeof(ThingDef).GetMethods(AccessTools.all).First(mi => mi.HasAttribute<CompilerGeneratedAttribute>() && mi.Name.Contains("PostLoad")), prefix: new HarmonyMethod(typeof(RWDebug), nameof(RWDebug.Prefix)));
            harmony.Patch(AccessTools.Method(AccessTools.TypeByName("KCSG.StartupActions"), "AddDef"), prefix: new HarmonyMethod(typeof(RWDebug), nameof(Prefix)));
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
