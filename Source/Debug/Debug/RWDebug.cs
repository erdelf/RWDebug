using RimWorld;
using Verse;

namespace Debug
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Security.Policy;
    using HarmonyLib;
    using RimWorld.Planet;
    using UnityEngine;
    using UnityEngine.Scripting;
    using Verse.AI;
    using Object = System.Object;

    [StaticConstructorOnStartup]
    public class RWDebug
    {
        static RWDebug()
        {
            Harmony harmony = new Harmony("rimworld.erdelf.debug");
            Harmony.DEBUG = true;
            harmony.Patch(AccessTools.Method(typeof(HaulDestinationManager), "CompareHaulDestinationPrioritiesDescending"), prefix: new HarmonyMethod(typeof(RWDebug), nameof(Prefix)));
        }

        public static void Prefix(IHaulDestination a, IHaulDestination b)
        {
            if(a.GetStoreSettings() == null)
                Log.Message($"{a} at {a.Position} is having issues");

            if(b.GetStoreSettings() == null)
                Log.Message($"{b} at {b.Position} is having issues");

            Log.ResetMessageCount();
        }

        
    }

    public class DummyDef : ThingDef
    {
        public DummyDef() : base()
        {
            //Log.Message("Patch");
            Harmony harmony = new Harmony("rimworld.erdelf.dummy_checker");
            //harmony.Patch(typeof(ThingDef).GetMethods(AccessTools.all).First(mi => mi.HasAttribute<CompilerGeneratedAttribute>() && mi.Name.Contains("PostLoad")), prefix: new HarmonyMethod(typeof(RWDebug), nameof(RWDebug.Prefix)));
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
