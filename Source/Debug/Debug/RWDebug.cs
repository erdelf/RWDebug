using RimWorld;
using Verse;

namespace Debug
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
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
            //AccessTools.Field(typeof(HealthCardUtility), "showAllHediffs").SetValue(null, true);
            //Harmony harmony = new Harmony("rimworld.erdelf.debug");
            //Harmony.DEBUG = true;

            //Log.Message(string.Join("\n", DefDatabase<PawnKindDef>.AllDefs.Select(pkd => pkd.RaceProps).Where(rp => rp.IsMechanoid).Select(rp => rp.body).Distinct().Select(body => $"{body.defName}: {string.Join(" | ", body.AllPartsVulnerableToFrostbite.Select(bpr => bpr.Label))}")));

            //harmony.Patch(AccessTools.Method(typeof(HealthCardUtility), "VisibleHediffs"), prefix: new HarmonyMethod(typeof(RWDebug), nameof(Prefix)));
        }
        

        public static void Prefix()
        {
            /*
            if(a.GetStoreSettings() == null)
                if (a is Building_Storage bs)
                    bs.settings = new StorageSettings(a);
                else
                    Log.Message($"{a} at {a.Position} is having issues");

            if(b.GetStoreSettings() == null)
                if (b is Building_Storage bs)
                    bs.settings = new StorageSettings(b);
                else
                    Log.Message($"{b} at {b.Position} is having issues");

            Log.ResetMessageCount();*/
        }

        [DebugAction("General", allowedGameStates = AllowedGameStates.Playing)]
        public static void DumpPawnAtlasByDir()
        {
            List<DebugMenuOption> list = new List<DebugMenuOption>();

            for (int i = 7; i <= 16; i++)
            {
                int value = Mathf.RoundToInt(Mathf.Pow(2, i));
                list.Add(new DebugMenuOption(value.ToString(CultureInfo.CurrentCulture), DebugMenuOptionMode.Action, () =>
                                                                                                                     {
                                                                                                                         List<DebugMenuOption> list2 = new List<DebugMenuOption>();

                                                                                                                         int count = Mathf.CeilToInt(Mathf.Sqrt(Find.ColonistBar.GetColonistsInOrder().Count));

                                                                                                                         for (int j = 1; j <= count; j ++)
                                                                                                                         {
                                                                                                                             int val = j;
                                                                                                                             int k   = val * val;
                                                                                                                             list2.Add(new DebugMenuOption($"{k} | {val}*{val}", DebugMenuOptionMode.Action, () =>
                                                                                                                                                               LongEventHandler.QueueLongEvent(() => DumpAtlas(value, val), "Creating Pawn Atlas", false, null)));
                                                                                                                         }

                                                                                                                         Find.WindowStack.Add(new Dialog_DebugOptionListLister(list2));
                                                                                                                     }));
            }

            Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
        }

        public static void DumpAtlas(int atlasSize, int scaleFactor)
        {
            int pawnSize = atlasSize / scaleFactor;

            int pawnsPerAtlas = scaleFactor * scaleFactor;

            List<Rect> uvRects = new List<Rect>();
            for (int x = 0; x < atlasSize; x += pawnSize)
            {
                for (int y = 0; y < atlasSize; y += pawnSize)
                    uvRects.Add(new Rect((float)x / atlasSize, (float)y / atlasSize, (float)pawnSize / atlasSize, (float) pawnSize / atlasSize));
            }

            Pawn[] colonistsInOrder = Find.ColonistBar.GetColonistsInOrder().ToArray();

            string path = Application.dataPath + "\\atlasDump_PawnsByDir";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            else
                foreach (string file in Directory.GetFiles(path))
                    File.Delete(file);

            for (int c = 0; c < Mathf.CeilToInt((float) colonistsInOrder.Length / pawnsPerAtlas); c++)
            {
                int initialIndex = pawnsPerAtlas * c;

                for (int i = 0; i < 4; i++)
                {
                    Rot4 rot = new Rot4(i);
                    RenderTexture texture = new RenderTexture(atlasSize, atlasSize, 24, RenderTextureFormat.ARGB32, 0)
                                            {
                                                name = $"Atlas_{c}_{rot.ToStringHuman()}"
                                            };

                    for (int index = 0; index < pawnsPerAtlas && initialIndex + index < colonistsInOrder.Length; index++)
                    {
                        Pawn pawn = colonistsInOrder[initialIndex + index];

                        Find.PawnCacheCamera.rect = uvRects[index];
                        Find.PawnCacheRenderer.RenderPawn(pawn, texture, Vector3.zero, 1f, 0f, rot);
                        Find.PawnCacheCamera.rect = new Rect(0f, 0f, 1f, 1f);
                    }

                    TextureAtlasHelper.WriteDebugPNG(texture, $"{path}\\{texture.name}.png");
                }
            }
        }

        [DebugAction("General", allowedGameStates = AllowedGameStates.Playing)]
        public static void DumpPawnAtlasByDirIndividual()
        {
            List<DebugMenuOption> list = new List<DebugMenuOption>();

            for (int i = 7; i <= 16; i++)
            {
                int value = Mathf.RoundToInt(Mathf.Pow(2, i));
                list.Add(new DebugMenuOption(value.ToString(CultureInfo.CurrentCulture), DebugMenuOptionMode.Action, () =>
                {
                    LongEventHandler.QueueLongEvent(() => DumpAtlasIndividual(value), "Creating Pawn Atlas", false, null);
                }));
            }

            Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
        }

        public static void DumpAtlasIndividual(int atlasSize)
        {
            Rect uvRect =new Rect(0, 0, atlasSize, atlasSize);

            Pawn[] colonistsInOrder = Find.ColonistBar.GetColonistsInOrder().ToArray();

            string path = Application.dataPath + "\\atlasDump_PawnsByDirIndividual";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            else
                foreach (string file in Directory.GetFiles(path))
                    File.Delete(file);

            for (int c = 0; c < colonistsInOrder.Length; c++)
            {
                Pawn pawn = colonistsInOrder[c];

                for (int i = 0; i < 4; i++)
                {
                    Rot4 rot = new Rot4(i);
                    RenderTexture texture = new RenderTexture(atlasSize, atlasSize, 24, RenderTextureFormat.ARGB32, 0)
                    {
                        name = $"{(pawn.Name as NameTriple)?.Nick ?? pawn.Name.ToStringShort}_{rot.ToStringHuman()}"
                    };

                    Find.PawnCacheCamera.rect = uvRect;
                    Find.PawnCacheRenderer.RenderPawn(pawn, texture, Vector3.zero, 1f, 0f, rot);
                    Find.PawnCacheCamera.rect = new Rect(0f, 0f, 1f, 1f);

                    TextureAtlasHelper.WriteDebugPNG(texture, $"{path}\\{texture.name}.png");
                }
            }
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
