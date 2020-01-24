using Harmony;
using RimWorld;
using System;
using Verse;

namespace Debug
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using JetBrains.Annotations;
    using RimWorld.Planet;
    using TMPro;
    using UnityEngine;
    using Object = System.Object;

    [StaticConstructorOnStartup]
    public class Debug
    {
        static Debug()
        {
             HarmonyInstance harmony = HarmonyInstance.Create("rimworld.erdelf.debug");



             HarmonyInstance.DEBUG = true;
            harmony.Patch(AccessTools.Method(typeof(InspectGizmoGrid), nameof(InspectGizmoGrid.DrawInspectGizmoGridFor)), transpiler: new HarmonyMethod(typeof(Debug), nameof(Transpiler)));

        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionList = instructions.ToList();
            MethodInfo clearInfo = AccessTools.Method(typeof(List<object>), nameof(List<object>.Clear));

            for (int i = 0; i < instructionList.Count; i++)
            { 
                CodeInstruction instruction = instructionList[i];

                yield return instruction;

                if (instruction.opcode == OpCodes.Ldsfld && i > 0 && instructionList[i - 1].operand == clearInfo && instructionList[i+1].opcode == OpCodes.Call)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Debug), nameof(GizmosCheck)));
                }
            }
        }

        public static List<Gizmo> GizmosCheck(List<Gizmo> gizmos, IEnumerable<object> selected)
        {
            return gizmos;
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