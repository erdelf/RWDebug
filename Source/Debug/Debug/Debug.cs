using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace Debug
{
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Schema;

    [StaticConstructorOnStartup]
    public class Debug
    {
        static Debug()
        {
            XmlDocument CreateDoc() =>
                LoadedModManager.CombineIntoUnifiedXML(LoadedModManager.RunningMods.SelectMany(m => DirectXmlLoader.XmlAssetsInModFolder(m, "Defs/")).ToList(),
                                                                        new Dictionary<XmlNode, LoadableXmlAsset>());

            void testTime(string path, int count)
            {
                long[] length = new long[count];

                for (int i = 0; i < count; i++)
                {
                    XmlDocument xmlDoc = CreateDoc();
                    StringBuilder sb = new StringBuilder();
                    Stopwatch     sw = new Stopwatch();
                    sw.Start();
                    foreach (XmlNode selectNode in xmlDoc.SelectNodes(path)) sb.AppendLine(selectNode.Name);
                    sw.Stop();
                    length[i] = sw.ElapsedTicks;
                }

                Log.Message($"{length.Average()}\t for {path} with {count} runs");
            }

            testTime(@"Defs/ThingDef", 1);
            testTime(@"*/ThingDef",    1);
            testTime(@"Defs/ThingDef", 100);
            testTime(@"*/ThingDef",    100);
            testTime(@"Defs/ThingDef", 1000);
            testTime(@"*/ThingDef",    1000);
        }
    }

    public class DummyDef : ThingDef
    {

        public DummyDef() : base()
        {
            //HarmonyInstance harmony = HarmonyInstance.Create("rimworld.erdelf.dummy_checker");
            //harmony.Patch(AccessTools.Method(typeof(DefDatabase<ThingDef>), "AddAllInMods"), transpiler: new HarmonyMethod(typeof(Debug), nameof(Debug.Transpiler)));
        }


        public static void checkDefName(ModMetaData __instance)
        {
            Log.Message(__instance.Name);
        }
    }
}