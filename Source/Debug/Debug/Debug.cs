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
    using System.Runtime.Remoting;
    using HarmonyLib;
    using RimWorld.Planet;
    using UnityEngine;

    [StaticConstructorOnStartup]
    public class Debug : MonoBehaviour
    {
        [TweakValue("ZDebug", 0f, 1f)]
        private static float InsectRed = 1f;

        private static void InsectRed_Changed() => Changed();

        [TweakValue("ZDebug", 0f, 1f)]
        private static float InsectGreen = 1f;

        private static void InsectGreen_Changed() => Changed();

        [TweakValue("ZDebug", 0f, 1f)]
        private static float InsectBlue = 1f;

        [TweakValue("ZDebug", 0f, 2f)]
        private static float Multiplier = 1f;

        private static void InsectBlue_Changed() => Changed();


        private static void Changed()
        {
            Find.CameraDriver.StartCoroutine(Coroutine());

                return;
            foreach (PawnKindLifeStage stage in PawnKindDefOf.Megascarab.lifeStages.Concat(PawnKindDefOf.Megaspider.lifeStages))
            {
                stage.bodyGraphicData.color = new Color(InsectRed, InsectGreen, InsectBlue);
                Traverse.Create(stage.bodyGraphicData).Method("Init").GetValue();
            }

            foreach (Pawn mapPawnsAllPawn in Find.CurrentMap.mapPawns.AllPawns)
                if (mapPawnsAllPawn.def.race.FleshType == FleshTypeDefOf.Insectoid)
                    mapPawnsAllPawn.Drawer.renderer.graphics.ResolveAllGraphics();
        }


        private static IEnumerator Coroutine()
        {
            Log.Message("0");
            float h = 0;
            while (true)
            {
                Log.Message("1");
                yield return new WaitForSecondsRealtime(0.1f);

                foreach (PawnKindLifeStage stage in PawnKindDefOf.Megascarab.lifeStages.Concat(PawnKindDefOf.Megaspider.lifeStages))
                {
                    stage.bodyGraphicData.color = Color.HSVToRGB(h, 1f, 1f);
                    Traverse.Create(stage.bodyGraphicData).Method("Init").GetValue();
                }

                if (Find.CurrentMap != null)
                    foreach (Pawn mapPawnsAllPawn in Find.CurrentMap.mapPawns.AllPawns)
                        if (mapPawnsAllPawn.def.race.FleshType == FleshTypeDefOf.Insectoid)
                            mapPawnsAllPawn.Drawer.renderer.graphics.ResolveAllGraphics();
                Log.Message("2");
                h += 0.1f * Multiplier;
                h %= 1f;
            }
        }


        static Debug()
        {
         
            Harmony harmony = new Harmony("rimworld.erdelf.debug");

            //harmony.Patch(AccessTools.Method(typeof(CreditsAssembler), nameof(CreditsAssembler.AllCredits)), postfix: new HarmonyMethod(typeof(Debug), nameof(Postfix)));

            //Harmony.DEBUG = true;
            //harmony.Patch(AccessTools.PropertyGetter(typeof(Settlement), nameof(Settlement.Material)), prefix: new HarmonyMethod(typeof(Debug), nameof(Debug.Prefix)));
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
