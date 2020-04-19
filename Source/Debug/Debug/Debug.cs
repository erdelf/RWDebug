using RimWorld;
using System;
using Verse;

namespace Debug
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading;
    using HarmonyLib;
    using JetBrains.Annotations;
    using RimWorld.Planet;
    using UnityEngine;
    using Object = System.Object;

    [StaticConstructorOnStartup]
    public class Debug
    {
        static Debug()
        {
            Harmony harmony = new Harmony("rimworld.erdelf.debug");



            //HarmonyInstance.DEBUG = true;
            //harmony.Patch(AccessTools.Method(typeof(CompArt), nameof(CompArt.GenerateImageDescription)), prefix: new HarmonyMethod(typeof(Debug), nameof(Debug.Prefix)));
            Log.Message("hey");



            Texture2D mainTexture = ContentFinder<Texture2D>.Get("image path");
            RenderTexture tmp2 = RenderTexture.GetTemporary(
                                                            mainTexture.width,
                                                            mainTexture.height,
                                                            0,
                                                            RenderTextureFormat.Default,
                                                            RenderTextureReadWrite.Linear);
            Graphics.Blit(mainTexture, tmp2);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = tmp2;
            Texture2D myTexture2D = new Texture2D(mainTexture.width, mainTexture.height);
            myTexture2D.ReadPixels(new Rect(0, 0, tmp2.width, tmp2.height), 0, 0);
            myTexture2D.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(tmp2);

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\texture.png";

            System.IO.Directory.CreateDirectory(Path.GetDirectoryName(path));

            System.IO.File.WriteAllBytes(path, myTexture2D.EncodeToPNG());
        }

        public static bool Prefix(CompArt __instance, ref TaggedString __result)
        {
            if (__instance.Title.EqualsIgnoreCase("Silence with Gartner"))
            {
                __result = new TaggedString("An engraving on this furniture illustrates Gustav Gartner trying to light a fire while covered in frost. The scene takes place inside a snow-covered hickory forest. The scene takes place on the outskirts of a town. This artwork refers to Gartner freezing to death on 15th of Aprimay, 5500.");
                return false;
            }
            return true;
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