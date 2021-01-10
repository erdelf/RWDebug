using RimWorld;
using Verse;

namespace Debug
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Security.Policy;
    using HarmonyLib;
    using RimWorld.Planet;
    using UnityEngine;
    using UnityEngine.Scripting;
    using Verse.AI;
    using Object = System.Object;

    [StaticConstructorOnStartup]
    public class RWDebug
    {
        static RWDebug()
        {
            Harmony harmony = new Harmony("rimworld.erdelf.debug");
            Harmony.DEBUG = true;
            harmony.Patch(AccessTools.Method(typeof(CastPositionFinder), "CastPositionPreference"), transpiler: new HarmonyMethod(typeof(RWDebug), nameof(Transpiler)));
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionList = instructions.ToList();

            FieldInfo avoidCoverInfo = AccessTools.Field(typeof(PawnKindDef), nameof(PawnKindDef.aiAvoidCover));

            bool done = false;

            for (int i = 0; i < instructionList.Count; i++)
            {
                CodeInstruction instruction = instructionList[i];
                yield return instruction;

                if (!done && instructionList[i + 2].OperandIs(avoidCoverInfo))
                {
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RWDebug), nameof(AvoidCover)));
                    i += 2;
                    done = true;
                }
            }
        }

        public static bool AvoidCover(Pawn pawn)
        {
            return true;
        }
    }

    public class ExpressionTree
    {
        public ExpressionTree child;

        public FieldInfo fi;
        public int listIndex = -1;

        public object SetValue(object instance, float newVal)
        {
            if (this.child != null)
            {
                object value = this.listIndex == -1 ? this.fi.GetValue(instance) : AccessTools.Method(typeof(IList), "get_Item").Invoke(instance, new object[] {this.listIndex});
                return this.child.SetValue(value, newVal);
            }
            else
            {
                if (!float.IsNaN(newVal))
                {
                    if(this.fi.FieldType == typeof(int))
                        this.fi.SetValue(instance, Mathf.RoundToInt(newVal));
                    else
                        this.fi.SetValue(instance, newVal);
                }

                return this.fi.GetValue(instance);
            }
        }
    }


    [StaticConstructorOnStartup]
    public static class ResearchEdit
    {
        public static Dictionary<string, (ExpressionTree tree, Dictionary<Def, float> baseValues)> baseValues = 
            new Dictionary<string, (ExpressionTree tree, Dictionary<Def, float>)>();

        static ResearchEdit()
        {
            Harmony harmony = new Harmony("Balthoraz.ResearchEdit");
            harmony.Patch(AccessTools.Method(typeof(ResearchManager), nameof(ResearchManager.ReapplyAllMods)), new HarmonyMethod(typeof(ResearchEdit), nameof(ReapplyPrefix)), new HarmonyMethod(typeof(ResearchEdit), nameof(ReapplyPostfix)));

            AccessTools.FieldRef<ResearchProjectDef, List<ResearchMod>> modRef =
                AccessTools.FieldRefAccess<ResearchProjectDef, List<ResearchMod>>("researchMods");

            foreach (DefEditor defEditor in
                DefDatabase<ResearchProjectDef>.AllDefsListForReading.SelectMany(rpd => modRef(rpd) ?? new List<ResearchMod>()).OfType<DefEditor>())
            {
                if (!baseValues.ContainsKey(defEditor.target))
                {
                    string[] strings = defEditor.target.Split('.');

                    ExpressionTree tree = new ExpressionTree
                                          {
                                              fi = AccessTools.Field(defEditor.defType, strings[0])
                                          };

                    baseValues.Add(defEditor.target, (tree, new Dictionary<Def, float>()));

                    strings = strings.Skip(1).ToArray();


                    foreach (string s in strings)
                    {
                        ExpressionTree child = new ExpressionTree();

                        if (s.StartsWith("li[") && tree.fi.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            child.listIndex = int.Parse(s.Substring(3, s.Length - 4));
                        }
                        else
                        {
                            child.fi = AccessTools.Field(tree.fi.FieldType, s);
                        }

                        tree = tree.child = child;
                    }
                }

                defEditor.defRef = GenDefDatabase.GetDef(defEditor.defType, defEditor.def);

                (ExpressionTree expressionTree, Dictionary<Def, float> dictionary) = baseValues[defEditor.target];

                if (!dictionary.ContainsKey(defEditor.defRef))
                {
                    object value = expressionTree.SetValue(defEditor.defRef, float.NaN);
                    dictionary.Add(defEditor.defRef, value as int? ?? (float) value);
                }
            }
        }

        private static void ReapplyPrefix()
        {
            foreach ((ExpressionTree tree, Dictionary<Def, float> baseValue) in baseValues.Values)
            {
                foreach (Def def in baseValue.Keys) 
                    tree.SetValue(def, baseValue[def]);
            }
        }

        private static void ReapplyPostfix()
        {
            CostListCalculator.Reset();
        }
    }

    public class DefEditor : ResearchMod
    {
        private enum DefEditorMode
        {
            Add,
            Mult
        }

        public string def;
        public Type defType = typeof(ThingDef);
        public Def defRef;

        public string target;
        private float value;
        private DefEditorMode mode = DefEditorMode.Add;

        public override void Apply()
        {
            (ExpressionTree ex, Dictionary<Def, float> _) = ResearchEdit.baseValues[this.target];


            object val = ex.SetValue(this.defRef, Single.NaN);
            float newValue = val as int? ?? (float) val;

            switch (this.mode)
            {
                case DefEditorMode.Add:
                    newValue += this.value;
                    break;
                case DefEditorMode.Mult:
                    newValue *= this.value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            ex.SetValue(this.defRef, newValue);
        }
    }

    public class DummyDef : ThingDef
    {
        public DummyDef() : base()
        {
            //Log.Message("Patch");
            Harmony harmony = new Harmony("rimworld.erdelf.dummy_checker");
            //harmony.Patch(typeof(ThingDef).GetMethods(AccessTools.all).First(mi => mi.HasAttribute<CompilerGeneratedAttribute>() && mi.Name.Contains("PostLoad")), prefix: new HarmonyMethod(typeof(RWDebug), nameof(RWDebug.Prefix)));
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
