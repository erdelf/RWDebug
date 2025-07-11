using RimWorld;
using Verse;

namespace Debug
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Security.Policy;
    using System.Text;
    using System.Xml;
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
            List<LoadableXmlAsset> xmlAssets = LoadedModManager.RunningMods.SelectMany(m => DirectXmlLoader.XmlAssetsInModFolder(m, "Defs/")).ToList();
            XmlDocument CreateDoc() => 
                LoadedModManager.CombineIntoUnifiedXML(LoadedModManager.RunningMods.SelectMany(m => DirectXmlLoader.XmlAssetsInModFolder(m, "Defs/")).ToList(), new Dictionary<XmlNode, LoadableXmlAsset>());

            long[] testTime(string path, int count, long[] init = null)
            {
                long[] length = new long[count];

                init?.CopyTo(length, 0);

                for (int i = init?.Length ?? 0; i < count; i++)
                {
                    XmlDocument   xmlDoc = CreateDoc();
                    Stopwatch     sw     = new();
                    sw.Start();
                    foreach (XmlNode selectNode in xmlDoc.SelectNodes(path)!) 
                        _ = selectNode.Name;
                    sw.Stop();
                    length[i] = sw.ElapsedTicks;
                }

                Log.Message($"avg: {length.Average()}\tmin: {length.Min()}\tmax: {length.Max()}\nfor {path} with {count} runs");

                return length;
            }

            const string testOne = """Defs/TraderKindDef[defName="Base_Neolithic_Standard"]//tradeTag[text()="Artifact"]/../countRange""";
            const string testTwo = """Defs/TraderKindDef[defName="Base_Neolithic_Standard"]//*[tradeTag="Artifact"]/countRange""";
            const string testThree = "";


            long[] testOneTimes = testTime(testOne, 1);
            testOneTimes = testTime(testOne, 10,  testOneTimes);
            testOneTimes = testTime(testOne, 50,  testOneTimes);
            testOneTimes = testTime(testOne, 100, testOneTimes);
            testOneTimes = testTime(testOne, 200, testOneTimes);
            testOneTimes = testTime(testOne, 500, testOneTimes);
            testOneTimes = testTime(testOne, 750, testOneTimes);
            long[] testTwoTimes = testTime(testTwo, 1);
            testTwoTimes = testTime(testTwo, 10,  testTwoTimes);
            testTwoTimes = testTime(testTwo, 50,  testTwoTimes);
            testTwoTimes = testTime(testTwo, 100, testTwoTimes);
            testTwoTimes = testTime(testTwo, 200, testTwoTimes);
            testTwoTimes = testTime(testTwo, 500, testTwoTimes);
            testTwoTimes = testTime(testTwo, 750, testTwoTimes);
            testTime(testOne, 1000, testOneTimes);
            testTime(testTwo, 1000, testTwoTimes);



            //AccessTools.Field(typeof(HealthCardUtility), "showAllHediffs").SetValue(null, true);
            //Harmony harmony = new Harmony("rimworld.erdelf.debug");
            //Harmony.DEBUG = true;

            //Log.Message(string.Join("\n", DefDatabase<PawnKindDef>.AllDefs.Select(pkd => pkd.RaceProps).Where(rp => rp.IsMechanoid).Select(rp => rp.body).Distinct().Select(body => $"{body.defName}: {string.Join(" | ", body.AllPartsVulnerableToFrostbite.Select(bpr => bpr.Label))}")));

            //harmony.Patch(AccessTools.Method(typeof(DebugThingPlaceHelper), nameof(DebugThingPlaceHelper.IsDebugSpawnable)), prefix: new HarmonyMethod(typeof(RWDebug), nameof(Prefix)));


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
