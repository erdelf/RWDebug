using RimWorld;
using System;
using Verse;

namespace Debug
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using HarmonyLib;
    using UnityEngine;

    [StaticConstructorOnStartup]
    public class Debug
    {
        static Debug()
        {
            Log.Message("EVIL:\n" + string.Join("\n", DefDatabase<WorldObjectDef>.AllDefs.Where(def => def.Material == null).Select(def => def.modContentPack.Name + ": " + def.defName)));

            Harmony harmony = new Harmony("rimworld.erdelf.debug");

            //Harmony.DEBUG = true;
            //harmony.Patch(AccessTools.Method(typeof(CharacterCardUtility), nameof(CharacterCardUtility.DrawCharacterCard)), transpiler: new HarmonyMethod(typeof(Debug), nameof(Debug.Transpiler)));
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen)
        {
            IEnumerable<CodeInstruction> codeInstructions = Transpiler2(instructions, gen);
            foreach (CodeInstruction codeInstruction in codeInstructions)
            {
                //Log.Message(codeInstruction.ToString());
                Log.ResetMessageCount();
                yield return codeInstruction;
            }
        }
    }

    public class DummyDef : ThingDef
    {
        public DummyDef() : base()
        {
            //Log.Message("Patch");
            Harmony harmony = new Harmony("rimworld.erdelf.dummy_checker");
            //harmony.Patch(typeof(ThingDef).GetMethods(AccessTools.all).First(mi => mi.HasAttribute<CompilerGeneratedAttribute>() && mi.Name.Contains("PostLoad")), prefix: new HarmonyMethod(typeof(Debug), nameof(Debug.Prefix)));
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
