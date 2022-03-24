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
            List<Backstory> backstories = BackstoryDatabase.allBackstories.Values.ToList();

            List<BodyTypeDef> maleBodyTypes = new List<BodyTypeDef>();
            List<BodyTypeDef> femaleBodyTypes = new List<BodyTypeDef>();

            Dictionary<BodyTypeDef, Dictionary<Gender, List<Backstory>>> bsGbtd = new Dictionary<BodyTypeDef, Dictionary<Gender, List<Backstory>>>();

            AccessTools.FieldRef<Backstory, BodyTypeDef> maleRef = AccessTools.FieldRefAccess<Backstory, BodyTypeDef>("bodyTypeMaleResolved");
            AccessTools.FieldRef<Backstory, BodyTypeDef> femaleRef = AccessTools.FieldRefAccess<Backstory, BodyTypeDef>("bodyTypeFemaleResolved");

            foreach (Backstory backstory in backstories)
            {
                try
                {
                    BodyTypeDef maleBodyType   = maleRef.Invoke(backstory);   // backstory.BodyTypeFor(Gender.Male);
                    BodyTypeDef femaleBodyType = femaleRef.Invoke(backstory); // backstory.BodyTypeFor(Gender.Female);

                    Log.Message(backstory.identifier + " | " + maleBodyType?.defName + " | " + femaleBodyType?.defName);

                    if (maleBodyType != null)
                    {
                        maleBodyTypes.Add(maleBodyType);
                        if (!bsGbtd.ContainsKey(maleBodyType))
                            bsGbtd.Add(maleBodyType, new Dictionary<Gender, List<Backstory>>());
                        if (!bsGbtd[maleBodyType].ContainsKey(Gender.Male))
                            bsGbtd[maleBodyType].Add(Gender.Male, new List<Backstory>());
                        bsGbtd[maleBodyType][Gender.Male].Add(backstory);
                    }

                    if (femaleBodyType != null)
                    {
                        femaleBodyTypes.Add(femaleBodyType);
                        if (!bsGbtd.ContainsKey(femaleBodyType))
                            bsGbtd.Add(femaleBodyType, new Dictionary<Gender, List<Backstory>>());
                        if (!bsGbtd[femaleBodyType].ContainsKey(Gender.Female))
                            bsGbtd[femaleBodyType].Add(Gender.Female, new List<Backstory>());
                        bsGbtd[femaleBodyType][Gender.Female].Add(backstory);
                    }

                    Log.ResetMessageCount();
                }
                catch
                {
                    // ignored
                }
            }
            Log.Message("2");
            Log.Message("male: "   + string.Join(" | ", maleBodyTypes.Where(btd => btd   != null).Distinct().OrderBy(btd => btd.defName).Select(btd => btd.defName)));
            Log.Message("female: " + string.Join(" | ", femaleBodyTypes.Where(btd => btd != null).Distinct().OrderBy(btd => btd.defName).Select(btd => btd.defName)));

            Log.Message(string.Join("\n", bsGbtd.Select(btdDict => $"{btdDict.Key.defName}: {string.Join(" \t||\t ", btdDict.Value.Select(gl => $"{gl.Key.ToString()}: ({gl.Value.Count}) {gl.Value.First()}"))}")));




            //AccessTools.Field(typeof(HealthCardUtility), "showAllHediffs").SetValue(null, true);
            //Harmony harmony = new Harmony("rimworld.erdelf.debug");
            //Harmony.DEBUG = true;

            //Log.Message(string.Join("\n", DefDatabase<PawnKindDef>.AllDefs.Select(pkd => pkd.RaceProps).Where(rp => rp.IsMechanoid).Select(rp => rp.body).Distinct().Select(body => $"{body.defName}: {string.Join(" | ", body.AllPartsVulnerableToFrostbite.Select(bpr => bpr.Label))}")));

            //harmony.Patch(AccessTools.Method(typeof(HealthCardUtility), "VisibleHediffs"), prefix: new HarmonyMethod(typeof(RWDebug), nameof(Prefix)));
        }
        

        public static void Prefix()
        {
            /*
            if(a.GetStoreSettings() == null)
                if (a is Building_Storage bs)
                    bs.settings = new StorageSettings(a);
                else
                    Log.Message($"{a} at {a.Position} is having issues");

            if(b.GetStoreSettings() == null)
                if (b is Building_Storage bs)
                    bs.settings = new StorageSettings(b);
                else
                    Log.Message($"{b} at {b.Position} is having issues");

            Log.ResetMessageCount();*/
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
