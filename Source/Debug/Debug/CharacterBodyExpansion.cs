using Harmony;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace CharacterBodyExpansion
{
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    [StaticConstructorOnStartup]
    public class CharacterBodyExpansion
    {
        static CharacterBodyExpansion()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.erdelf.CharacterBodyExpansion");
            harmony.Patch(AccessTools.Method(typeof(ApparelGraphicRecordGetter), nameof(ApparelGraphicRecordGetter.TryGetGraphicApparel)), transpiler: new HarmonyMethod(typeof(CharacterBodyExpansion), nameof(CharacterBodyExpansion.Transpiler)));
            harmony.Patch(AccessTools.Method(typeof(PawnGenerator), "GenerateBodyType"), postfix: new HarmonyMethod(typeof(CharacterBodyExpansion), nameof(GenerateBodyTypePostfix)));

            foreach (BodyTypeDef def in DefDatabase<BodyTypeDef>.AllDefsListForReading)
            {
                bodyInheritance.Add(def, new HashSet<BodyTypeDef>());
                bodyInheritance[def].Add(def);
                BodyDefApparelParentageInfo info = def.GetModExtension<BodyDefApparelParentageInfo>();
                if (info != null)
                    bodyInheritance[info.parent].Add(def);
            }
        }

        public static Dictionary<BodyTypeDef, HashSet<BodyTypeDef>> bodyInheritance = new Dictionary<BodyTypeDef, HashSet<BodyTypeDef>>();

        public static void GenerateBodyTypePostfix(Pawn pawn) =>
            pawn.story.bodyType = bodyInheritance[key: pawn.story.bodyType].Where(predicate: bt =>
                                                                                  {
                                                                                      GenderPossibility gender = bt.GetModExtension<BodyDefApparelParentageInfo>()?.gender ?? GenderPossibility.Either;
                                                                                      return gender == GenderPossibility.Either ||
                                                                                             gender == GenderPossibility.Male && pawn.gender == Gender.Male ||
                                                                                                gender == GenderPossibility.Female && pawn.gender == Gender.Female;
                                                                                  }).RandomElement();

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
        public GenderPossibility gender;
    }
}