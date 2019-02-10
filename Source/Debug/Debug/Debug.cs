﻿using System.Collections.Generic;
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
            harmony.Patch(AccessTools.Method(typeof(HealthCardUtility), "DrawOverviewTab"), transpiler: new HarmonyMethod(typeof(Debug), nameof(Transpiler)));
            
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codeInstructions = instructions.ToList();

            MethodInfo labelFor = AccessTools.Method(typeof(PawnCapacityDef), nameof(PawnCapacityDef.GetLabelFor), new []{typeof(bool), typeof(bool)});

            int index = codeInstructions.FindIndex(ci => ci.operand == labelFor);

            codeInstructions[index].operand = AccessTools.Method(typeof(PawnCapacityDef), nameof(PawnCapacityDef.GetLabelFor), new[] {typeof(Pawn)});
            codeInstructions.RemoveRange(index-6, 6);

            return codeInstructions;
        }
    }


    public class DummyDef : ThingDef
    {
        /*
        public DummyDef() : base()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.erdelf.dummy_checker");
            

                harmony.Patch(AccessTools.Method(typeof(ModMetaData), "Init"), null,
                    new HarmonyMethod(typeof(DummyDef), nameof(checkDefName)));
        }


        public static void checkDefName(ModMetaData __instance)
        {
            Log.Message(__instance.Name);
        }*/
    }
}