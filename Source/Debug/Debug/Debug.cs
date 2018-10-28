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
            //HarmonyInstance.DEBUG = true;
            
            // Things/Pawn/Animal/Monkey/Monkey

            
            Type[] types = typeof(Pawn).Assembly.GetTypes().SelectMany(t => t.GetNestedTypes(AccessTools.all).Concat(t)).ToArray();
            Log.Message($"{types.Length} types with {types.Sum(t => t.GetFields(AccessTools.all).Count(f => f.DeclaringType == t))} fields and {types.Sum(t => t.GetMethods(AccessTools.all).Count(m => m.DeclaringType == t))} methods.");
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            FieldInfo defInfo = AccessTools.Field(type: typeof(Thing), name: nameof(Thing.def));

            CodeInstruction[] codeInstructions = instructions as CodeInstruction[] ?? instructions.ToArray();
            for (int index = 0; index < codeInstructions.Length; index++)
            {
                CodeInstruction instruction = codeInstructions[index];
                if (instruction.opcode == OpCodes.Ldfld && instruction.operand == defInfo)
                {
                    index += 1;
                    instruction = new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(type: typeof(Debug), name: nameof(CreateRecipes)));
                }
                yield return instruction;
            }
        }

        public static List<RecipeDef> CreateRecipes(Pawn p)
        {
            List<RecipeDef> recipes = p.def.AllRecipes;

            return recipes;
        }

    }


    public class DummyDef : ThingDef
    {
        /*
        public DummyDef() : base()
        {
            Log.Message("dummyCheck");
            HarmonyInstance.Create("rimworld.erdelf.dummy_checker").Patch(
                AccessTools.Method(typeof(PawnKindDef), nameof(ThingDef.ConfigErrors)),
                new HarmonyMethod(typeof(DummyDef), nameof(checkDefName)), null);
        }

        public static void checkDefName(PawnKindDef __instance)
        {
            Log.Message(__instance.defName + ": " + string.Join(" | ", __instance.weaponTags?.ToArray() ?? new string[] { "" }));
        }*/
    }
}