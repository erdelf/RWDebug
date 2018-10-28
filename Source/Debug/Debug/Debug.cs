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
            harmony.Patch(AccessTools.Property(typeof(Name), nameof(Name.UsedThisGame)).GetGetMethod(), new HarmonyMethod(typeof(Debug), nameof(UsedPrefix)));
            harmony.Patch(AccessTools.Method(typeof(NameUseChecker), nameof(NameUseChecker.NameWordIsUsed)), new HarmonyMethod(typeof(Debug), nameof(UsedPrefix)));
            harmony.Patch(AccessTools.Property(typeof(NameUseChecker), nameof(NameUseChecker.AllPawnsNamesEverUsed)).GetGetMethod(), new HarmonyMethod(typeof(Debug), nameof(NamePrefix)));

            //HarmonyInstance.DEBUG = true;

            LongEventHandler.QueueLongEvent(() =>
            {


                Current.Game = new Game
                {
                    World = new World
                    {
                        factionManager = new FactionManager(),
                        worldPawns = new WorldPawns()
                    },
                    storyteller = new Storyteller(StorytellerDefOf.Cassandra, DifficultyDefOf.Rough)
                };


                Find.FactionManager.Add(FactionGenerator.NewGeneratedFaction(FactionDefOf.PlayerTribe));

                FactionDefOf.PlayerColony.isPlayer = false;
                FactionDefOf.PlayerColony.hidden   = true;
                Faction faction = FactionGenerator.NewGeneratedFaction(FactionDefOf.PlayerColony);
                Find.FactionManager.Add(faction);

                FactionDefOf.PlayerColony.pawnNameMaker = RulePackDef.Named("NamerPersonTribal");

                int i = 0;
                for (int j = 0; j < 100000; j++)
                {
                    if (PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDefOf.Villager, faction, forceGenerateNewPawn: true, canGeneratePawnRelations: false)).health.hediffSet
                       .HasHediff(HediffDefOf.Gunshot))
                        Log.Message($"Pawn with gunshots generated");
                }
            }, "GENERATING PAWNS", true, null);

        }

        public static bool UsedPrefix() => false;

        public static bool NamePrefix(ref IEnumerable<Name> __result)
        {
            __result = new[] {new NameSingle("erdelf"), new NameSingle("Mehni"), new NameSingle("Jecrell")};

            return false;
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