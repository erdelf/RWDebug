using Harmony;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace CharacterBodyExpansion
{
    using System.Reflection;
    using System.Reflection.Emit;

    [StaticConstructorOnStartup]
    public class CharacterBodyExpansion
    {
        static CharacterBodyExpansion()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.erdelf.CharacterBodyExpansion");
            harmony.Patch(AccessTools.Method(typeof(ApparelGraphicRecordGetter), nameof(ApparelGraphicRecordGetter.TryGetGraphicApparel)), transpiler: new HarmonyMethod(typeof(CharacterBodyExpansion), nameof(CharacterBodyExpansion.Transpiler)));
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            FieldInfo defInfo = AccessTools.Field(typeof(Def), nameof(Def.defName));

            foreach (CodeInstruction instruction in instructions)
            {

                if (instruction.opcode == OpCodes.Ldfld && instruction.operand == defInfo)
                    yield return new CodeInstruction(OpCodes.Call, operand: AccessTools.Method(typeof(CharacterBodyExpansion), nameof(GetApparelBodyDefname)));
                else
                    yield return instruction;
            }

        }

        public static string GetApparelBodyDefname(Def def) => 
            def.GetModExtension<BodyDefApparelParentageInfo>()?.parent.defName ?? def.defName;
    }

    public class BodyDefApparelParentageInfo : DefModExtension
    {
        public BodyTypeDef parent;
    }
}