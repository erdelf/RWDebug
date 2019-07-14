using Harmony;
using RimWorld;
using System;
using Verse;

namespace Debug
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using JetBrains.Annotations;
    using RimWorld.Planet;
    using UnityEngine;

    [StaticConstructorOnStartup]
    public class Debug
    {
        static Debug()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.erdelf.debug");
            harmony.Patch(AccessTools.Method(typeof(Alert_Thought), "AffectedPawns"), prefix: new HarmonyMethod(typeof(Debug), nameof(prefix)));
            HarmonyInstance.DEBUG = true;
            harmony.Patch(AccessTools.Method(typeof(PlayerItemAccessibilityUtility), "CacheAccessibleThings"), transpiler: new HarmonyMethod(typeof(Debug), nameof(transpiler)));
            HarmonyInstance.DEBUG = false;
        }

        public static void prefix(object __instance)
        {
            //Log.Message(__instance.GetType().FullName);
        }

        public static IEnumerable<CodeInstruction> transpiler(IEnumerable<CodeInstruction> instructions)
        {
            int i = -9;

            IEnumerable<CodeInstruction> PostLog(params CodeInstruction[] code)
            {
                string ld = string.Join(" | ", code.Select(c => c.ToString()).ToArray()) + "\n" + (i++).ToString();
                if (i > 0)
                {
                    yield return new CodeInstruction(OpCodes.Ldstr, operand: ld);
                    yield return new CodeInstruction(OpCodes.Ldc_I4_1);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Log), nameof(Log.Message)));
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Log), nameof(Log.ResetMessageCount)));
                }
            }

            List<CodeInstruction> instructionList = instructions.ToList();
            int                   counter         = i;
            for (int index = 0; index < instructionList.Count; index++)
            {
                CodeInstruction instruction = instructionList[index: index];

                if ((instruction.opcode == OpCodes.Callvirt && instruction.operand is MethodInfo && (instruction.operand.ToString().Contains("get_Count") ||
                                                                                                     instruction.operand.ToString().Contains("Clear")     ||
                                                                                                     instruction.operand == AccessTools
                                                                                                                        .Property(typeof(Pawn), nameof(Pawn.IsFreeColonist)).GetGetMethod() ||
                                                                                                     instruction.operand ==
                                                                                                     AccessTools.Property(typeof(Pawn), nameof(Pawn.Dead)).GetGetMethod() ||
                                                                                                     instruction.operand == AccessTools
                                                                                                                        .Property(typeof(Pawn), nameof(Pawn.Downed)).GetGetMethod() ||
                                                                                                     instruction.operand ==
                                                                                                     AccessTools.Method(typeof(Pawn_WorkSettings), nameof(Pawn_WorkSettings.WorkIsActive)))
                    ) ||
                    instruction.opcode == OpCodes.Ble)
                {
                    counter++;
                    instructionList.InsertRange(index + 2, PostLog(instructionList[index - 1], instructionList[index]));

                    if (instruction.operand == AccessTools.Method(typeof(Pawn_WorkSettings), nameof(Pawn_WorkSettings.WorkIsActive)))
                        instructionList.InsertRange(index + 6 + 4, PostLog(instructionList[index + 6 + 3], instructionList[index + 6 + 4]));

                    if (counter == 5)
                    {
                        instructionList.InsertRange(index + 8  + 4,  PostLog(instructionList[index + 8  + 4]));
                        instructionList.InsertRange(index + 13 + 8,  PostLog(instructionList[index + 13 + 8]));
                        instructionList.InsertRange(index + 15 + 12, PostLog(instructionList[index + 16 + 12]));
                        instructionList.InsertRange(index + 18 + 16, PostLog(instructionList[index + 19 + 16]));
                    }
                }

                yield return instruction;

                if ((instruction.opcode == OpCodes.Stloc_S && instructionList[index-1].opcode == OpCodes.Isinst))
                {
                    yield return new CodeInstruction(new CodeInstruction(OpCodes.Ldloc_S, (byte)25));
                    yield return new CodeInstruction(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Debug), nameof(CheckPawn))));
                }
            }
        }

        static void CheckPawn(Pawn pawn)
        {
            Log.Message($"{pawn != null} {pawn?.IsFreeColonist} {!pawn?.Dead} {!pawn?.Downed} {pawn?.workSettings?.WorkIsActive(WorkTypeDefOf.Crafting)} {pawn?.Name.ToStringFull}", true);
        }
    }

    public class DummyDef : ThingDef
    {
        public DummyDef() : base()
        {
            //HarmonyInstance harmony = HarmonyInstance.Create("rimworld.erdelf.dummy_checker");
            //harmony.Patch(AccessTools.Method(typeof(DefDatabase<ThingDef>), "AddAllInMods"), transpiler: new HarmonyMethod(typeof(Debug), nameof(Debug.Transpiler)));
        }


        public static void CheckDefName(ModMetaData __instance)
        {
            Log.Message(text: __instance.Name);
        }
    }
}