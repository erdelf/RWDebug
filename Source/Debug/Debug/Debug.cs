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
        private static readonly Dictionary<TickerType, HashSet<Type>> compTypes;
        private static readonly Dictionary<ThingWithComps, List<ThingComp>> compList;


        static Debug()
        {
            HarmonyInstance harmony = HarmonyInstance.Create(id: "rimworld.erdelf.debug");
            harmony.Patch(AccessTools.Method(typeof(Dialog_DebugActionsMenu), "DoListingItems"),                 postfix: new HarmonyMethod(typeof(Debug), nameof(AddAction)));

            compTypes = new Dictionary<TickerType, HashSet<Type>>();
            compTypes.Add(TickerType.Normal, new HashSet<Type>());
            compTypes.Add(TickerType.Rare,   new HashSet<Type>());
            compTypes.Add(TickerType.Long, new HashSet<Type>());

            foreach (Type type in typeof(ThingComp).AllSubclassesNonAbstract())
            {
                if ((AccessTools.Method(type, nameof(ThingComp.CompTick)).GetMethodBody()?.GetILAsByteArray().Length ?? 0) <= 1)
                    compTypes[TickerType.Normal].Add(type);

                if ((AccessTools.Method(type, nameof(ThingComp.CompTickRare)).GetMethodBody()?.GetILAsByteArray().Length ?? 0) <= 1)
                    compTypes[TickerType.Rare].Add(type);
            }

            compList = new Dictionary<ThingWithComps, List<ThingComp>>();

            harmony.Patch(AccessTools.Method(typeof(ThingWithComps), nameof(ThingWithComps.InitializeComps)), postfix: new HarmonyMethod(typeof(Debug), nameof(InitializeCompPostfix)));

            HarmonyInstance.DEBUG = true;
            harmony.Patch(AccessTools.Method(typeof(ThingWithComps), nameof(ThingWithComps.Tick)), transpiler: new HarmonyMethod(typeof(Debug), nameof(CompTicker)));
            harmony.Patch(AccessTools.Method(typeof(ThingWithComps), nameof(ThingWithComps.TickRare)), transpiler: new HarmonyMethod(typeof(Debug), nameof(CompTicker)));
            HarmonyInstance.DEBUG = false;
        }

        private static bool doTick = true;

        public static void AddAction(Dialog_DebugActionsMenu __instance)
        {
           AccessTools.Method(typeof(Dialog_DebugActionsMenu), "DebugAction").Invoke(__instance, new object[] {"All Tick Info", new Action(() =>
           {

               string GiveString(Traverse<List<List<Thing>>> list, TickerType type)
               {
                   return
                       $"normal: {list.Value.Sum(l => l.Count)}\n"                                                                                                                                                                           +
                       $"check: {list.Value.Select(t => t.Where(th => (AccessTools.Method(th.GetType(), type == TickerType.Normal ? nameof(Entity.Tick) : type == TickerType.Rare ? nameof(Entity.TickRare) : nameof(Entity.TickLong)).DeclaringType != typeof(ThingWithComps)) || ((ThingWithComps) th).AllComps.Count > 0 || ((ThingWithComps) th).AllComps.TrueForAll(c => (AccessTools.Method(c.GetType(), type == TickerType.Normal ? nameof(ThingComp.CompTick) : nameof(ThingComp.CompTickRare)).GetMethodBody()?.GetILAsByteArray().Length ?? 0) > 6))).Sum(l => l.Count())}\n" +
                       $"{list.Value.Sum(t => t.Sum(th => (th as ThingWithComps)?.AllComps.Count ?? 0))} | {list.Value.Sum(t => t.Sum(th => (th as ThingWithComps)?.AllComps.Count(tc => ((AccessTools.Method(tc.GetType(), type == TickerType.Normal ? nameof(ThingComp.CompTick) : nameof(ThingComp.CompTickRare)).GetMethodBody()?.GetILAsByteArray().Length ?? 0) > 1))))}\n" +
                       $"{string.Join("\n", list.Value.SelectMany(th => th).Select(t => AccessTools.Method(t.GetType(), type == TickerType.Normal ? nameof(Entity.Tick) : type == TickerType.Rare ? nameof(Entity.TickRare) : nameof(Entity.TickLong))).OrderBy(x => x.GetMethodBody()?.GetILAsByteArray().Length ?? 0).Select(x => (x.GetMethodBody()?.GetILAsByteArray().Length ?? 0) + " " + x.DeclaringType).ToArray())}";
               }

               Traverse mgr = Traverse.Create(Find.TickManager);
               Traverse<List<List<Thing>>> norm = mgr.Field("tickListNormal").Field<List<List<Thing>>>("thingLists");
               Traverse<List<List<Thing>>> rare = mgr.Field("tickListRare").Field<List<List<Thing>>>("thingLists");
               Traverse<List<List<Thing>>> lon = mgr.Field("tickListLong").Field<List<List<Thing>>>("thingLists");

               Log.Message(GiveString(norm, TickerType.Normal));
               Log.Message(GiveString(rare, TickerType.Rare));
               Log.Message(GiveString(lon, TickerType.Long));
           })});
           AccessTools.Method(typeof(Dialog_DebugActionsMenu), "DebugAction").Invoke(__instance, new object[] {"Toggle Ticking", new Action(() => doTick = !doTick)});
           AccessTools.Method(typeof(Dialog_DebugActionsMenu), "DebugAction").Invoke(__instance, new object[] {"Remove Tick Info", new Action(() =>
           {
               void ClearList(Traverse<List<List<Thing>>> list, TickerType type)
               {
                   list.Value = list.Value.Select(t => t.Where(th =>(AccessTools.Method(th.GetType(), type == TickerType.Normal ? nameof(Entity.Tick) : type == TickerType.Rare ? nameof(Entity.TickRare) : nameof(Entity.TickLong)).DeclaringType != typeof(ThingWithComps)) || ((ThingWithComps) th).AllComps.Count > 0).ToList()).ToList();
               }

               Traverse                    mgr  = Traverse.Create(Find.TickManager);
               Traverse<List<List<Thing>>> norm = mgr.Field("tickListNormal").Field<List<List<Thing>>>("thingLists");
               Traverse<List<List<Thing>>> rare = mgr.Field("tickListRare").Field<List<List<Thing>>>("thingLists");
               Traverse<List<List<Thing>>> lon  = mgr.Field("tickListLong").Field<List<List<Thing>>>("thingLists");

               ClearList(norm, TickerType.Normal);
               ClearList(rare, TickerType.Rare);
               ClearList(lon, TickerType.Long);
           })});
        }
        

        

        public static void InitializeCompPostfix(ThingWithComps __instance)
        {
            if(__instance.def.tickerType != TickerType.Never)
                compList.Add(__instance, Traverse.Create(__instance).Field<List<ThingComp>>("comps").Value?.Where(tc => !compTypes[__instance.def.tickerType].Contains(tc.GetType())).ToList() ?? new List<ThingComp>());
        }

        public static IEnumerable<CodeInstruction> CompTicker(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionList = instructions.ToList();

            FieldInfo comps = AccessTools.Field(typeof(ThingWithComps), "comps");
            FieldInfo getCompList = AccessTools.Field(typeof(Debug), nameof(compList));
            MethodInfo getComps = AccessTools.Property(typeof(Dictionary<ThingWithComps, List<ThingComp>>), "Item").GetGetMethod();

            for (int index = 0; index < instructionList.Count; index++)
            {
                CodeInstruction instruction = instructionList[index];
                if (index < (instructionList.Count-2) && instructionList[index+1].operand == comps)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, getCompList) { labels = instruction.labels.ListFullCopy()};
                    instruction.labels.Clear();
                    yield return instruction;
                    yield return new CodeInstruction(OpCodes.Call, getComps);
                    index++;
                }
                else
                    yield return instruction;
            }
        }

        public static List<ThingComp> GetComps(ThingWithComps thing) => compList[thing]; 
    }

    public class DummyDef : ThingDef
    {
        
        public DummyDef() : base()
        {
            //HarmonyInstance harmony = HarmonyInstance.Create("rimworld.erdelf.dummy_checker");
            //harmony.Patch(AccessTools.Method(typeof(DefDatabase<ThingDef>), "AddAllInMods"), transpiler: new HarmonyMethod(typeof(Debug), nameof(Debug.Transpiler)));
        }


        public static void checkDefName(ModMetaData __instance)
        {
            Log.Message(__instance.Name);
        }
    }
}