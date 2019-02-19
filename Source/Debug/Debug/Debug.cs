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
            //harmony.Patch(AccessTools.Method(typeof(DefDatabase<ThingDef>), "AddAllInMods"), transpiler: new HarmonyMethod(typeof(Debug), nameof(Transpiler)));
            
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codeInstructions = instructions.ToList();

            MethodInfo labelFor = AccessTools.Method(typeof(PawnCapacityDef), nameof(PawnCapacityDef.GetLabelFor), new []{typeof(bool), typeof(bool)});


            for (int i = 0; i < codeInstructions.Count; i++)
            {
                CodeInstruction instruction = codeInstructions[i];

                if (instruction.opcode == OpCodes.Ldc_I4_7 && codeInstructions[i+1].operand == typeof(System.String))
                {
                    instruction.opcode = OpCodes.Ldc_I4_8;
                }

                yield return instruction;

                if (instruction.operand == (object) " lacks a defName. Giving name ")
                {
                    yield return new CodeInstruction(OpCodes.Stelem_Ref);
                    yield return new CodeInstruction(OpCodes.Dup);
                    yield return new CodeInstruction(OpCodes.Ldc_I4_6);
                    yield return new CodeInstruction(OpCodes.Ldloc_3);
                    yield return new CodeInstruction(codeInstructions[i-5]);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Debug), nameof(GetInfoFromDef)));
                    codeInstructions[i + 3].opcode = OpCodes.Ldc_I4_7;
                }
            }
        }

        public static string GetInfoFromDef(Def def)
        {
            return $"\n {def.defPackage.relFolder} / {def.defPackage.fileName}:  {string.Join("|", def.defPackage.defs.Select(d => d.defName).ToArray())}";
        }
    }


    public class DummyDef : ThingDef
    {
        
        public DummyDef() : base()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.erdelf.dummy_checker");
            harmony.Patch(AccessTools.Method(typeof(DefDatabase<ThingDef>), "AddAllInMods"), transpiler: new HarmonyMethod(typeof(Debug), nameof(Debug.Transpiler)));

            //harmony.Patch(AccessTools.Method(typeof(ModMetaData), "Init"), null,
            //    new HarmonyMethod(typeof(DummyDef), nameof(checkDefName)));
        }


        public static void checkDefName(ModMetaData __instance)
        {
            Log.Message(__instance.Name);
        }
    }
}