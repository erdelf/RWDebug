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
    using TunnelHiveSpawner = RimWorld.TunnelHiveSpawner;

    [StaticConstructorOnStartup]
    public class RWDebug
    {
        static RWDebug()
        {
            //AccessTools.Field(typeof(HealthCardUtility), "showAllHediffs").SetValue(null, true);
            Harmony harmony = new Harmony("rimworld.erdelf.debug");
            //Harmony.DEBUG = true;

            //Log.Message(string.Join("\n", DefDatabase<PawnKindDef>.AllDefs.Select(pkd => pkd.RaceProps).Where(rp => rp.IsMechanoid).Select(rp => rp.body).Distinct().Select(body => $"{body.defName}: {string.Join(" | ", body.AllPartsVulnerableToFrostbite.Select(bpr => bpr.Label))}")));

            //harmony.Patch(AccessTools.Method(typeof(DebugThingPlaceHelper), nameof(DebugThingPlaceHelper.IsDebugSpawnable)), prefix: new HarmonyMethod(typeof(RWDebug), nameof(Prefix)));


            //Log.Message(DefDatabase<PreceptDef>.GetNamed("TreeCutting_Horrible").comps.OfType<PreceptComp_KnowsMemoryThought>().Join(pc => pc.eventDef.defName, "\n"));


            //Log.Message(SolidBioDatabase.allBios.Join(pb => pb.name.ToStringFull + ": " + pb.childhood.skillGains.Join(sg => sg.skill.defName + ": " + sg.amount, ",") + " | " + pb.adulthood.skillGains.Join(sg => sg.skill.defName + ": " + sg.amount, ","), "\n"));
            //harmony.Patch(AccessTools.Method(typeof(SectionLayer_EdgeShadows), nameof(SectionLayer_EdgeShadows.ShouldDrawDynamic)), new HarmonyMethod(typeof(RWDebug), nameof(Prefix)));
            //harmony.Patch(AccessTools.Method("SectionLayer_SunShadows:ShouldDrawDynamic"),                                          new HarmonyMethod(typeof(RWDebug), nameof(Prefix)));
            //harmony.Patch(AccessTools.Method(typeof(SectionLayer_EdgeShadows), nameof(SectionLayer_EdgeShadows.ShouldDrawDynamic)), new HarmonyMethod(typeof(RWDebug), nameof(Prefix)));

            //harmony.Patch(AccessTools.Method("SectionLayer_SunShadows:Regenerate"), transpiler: new HarmonyMethod(typeof(RWDebug), nameof(TranspilerSunRegen)));

            //harmony.Patch(AccessTools.Method("SectionLayer_SunShadows:DrawLayer"),   new HarmonyMethod(typeof(RWDebug),             nameof(Prefix2)));
            //harmony.Patch(AccessTools.Method("SectionLayer_EdgeShadows:Regenerate"), transpiler: new HarmonyMethod(typeof(RWDebug), nameof(TranspilerEdgeRegen)));
            //harmony.Patch(AccessTools.Method("SectionLayer_SunShadows:Regenerate"), transpiler: new HarmonyMethod(typeof(RWDebug), nameof(TranspilerEdgeRegen)));

            //harmony.Patch(AccessTools.PropertyGetter(AccessTools.TypeByName("SectionLayer_EdgeShadows"), "Visible"), new HarmonyMethod(typeof(RWDebug), nameof(Prefix)));
            //harmony.Patch(AccessTools.PropertyGetter(AccessTools.TypeByName("SectionLayer_IndoorMask"),  "Visible"), new HarmonyMethod(typeof(RWDebug), nameof(Prefix)));
            //harmony.Patch(AccessTools.PropertyGetter(AccessTools.TypeByName("SectionLayer_SunShadows"),  "Visible"), new HarmonyMethod(typeof(RWDebug), nameof(Prefix)));
            //harmony.Patch(AccessTools.Method("Printer_Shadow:PrintShadow", [typeof(SectionLayer), typeof(Vector3), typeof(Vector3), typeof(Rot4)]), new HarmonyMethod(typeof(RWDebug), nameof(Prefix2)));

            HashSet<byte[]> bytes = new HashSet<byte[]>();
            try
            {
                while (true)
                    bytes.Add(new byte[1024 * 1024 * 1024]);
            }
            catch
            {
            }

        }

        public static HashSet<string> stacktraces = new HashSet<string>();
        public static int             callCount = 0;

        public static bool Prefix(ref bool __result)
        {
            __result = false;
            return false;
        }
        public static bool Prefix2()
        {
            return false;
        }

        private static readonly Color32 Test = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

        public static IEnumerable<CodeInstruction> TranspilerEdgeRegen(IEnumerable<CodeInstruction> instructions)
        {
            FieldInfo fii = AccessTools.Field(typeof(LayerSubMesh), "colors");

            FieldInfo fi = AccessTools.Field(typeof(SectionLayer_EdgeShadows), "Shadowed");

            List<CodeInstruction> instructionList = instructions.ToList();

            for (int index = 0; index < instructionList.Count; index++)
            {
                CodeInstruction instruction = instructionList[index];
                yield return instruction;
                if (instruction.LoadsField(fii) && instructionList[index+2].Calls(AccessTools.Method(typeof(List<Color32>), "Add")))
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(RWDebug), nameof(Test)));
                    index++;
                }
            }
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
