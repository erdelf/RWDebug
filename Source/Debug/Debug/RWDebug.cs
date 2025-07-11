using RimWorld;
using Verse;

namespace Debug
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Security.Policy;
    using System.Text;
    using System.Xml;
    using HarmonyLib;
    using KCSG;
    using RimWorld.Planet;
    using RimWorld.QuestGen;
    using UnityEngine;
    using UnityEngine.Scripting;
    using Verse.AI;
    using Object = System.Object;
    using TunnelHiveSpawner = RimWorld.TunnelHiveSpawner;

    [StaticConstructorOnStartup]
    public class RWDebug
    {
        static RWDebug()
        {
            //AccessTools.Field(typeof(HealthCardUtility), "showAllHediffs").SetValue(null, true);
            //Harmony harmony = new Harmony("rimworld.erdelf.debug");
            //Harmony.DEBUG = true;

            //Log.Message(string.Join("\n", DefDatabase<PawnKindDef>.AllDefs.Select(pkd => pkd.RaceProps).Where(rp => rp.IsMechanoid).Select(rp => rp.body).Distinct().Select(body => $"{body.defName}: {string.Join(" | ", body.AllPartsVulnerableToFrostbite.Select(bpr => bpr.Label))}")));

            //harmony.Patch(AccessTools.Method(typeof(DebugThingPlaceHelper), nameof(DebugThingPlaceHelper.IsDebugSpawnable)), prefix: new HarmonyMethod(typeof(RWDebug), nameof(Prefix)));


            //Log.Message(DefDatabase<PreceptDef>.GetNamed("TreeCutting_Horrible").comps.OfType<PreceptComp_KnowsMemoryThought>().Join(pc => pc.eventDef.defName, "\n"));


            //Log.Message(SolidBioDatabase.allBios.Join(pb => pb.name.ToStringFull + ": " + pb.childhood.skillGains.Join(sg => sg.skill.defName + ": " + sg.amount, ",") + " | " + pb.adulthood.skillGains.Join(sg => sg.skill.defName + ": " + sg.amount, ","), "\n"));
        }

        public static HashSet<string> stacktraces = new HashSet<string>();
        public static int             callCount = 0;

        public static void Prefix(ThingDef def)
        {
            Log.ResetMessageCount();
            Log.Message("--------------------------------------------");
            Log.Message(def?.modContentPack?.Name);
            Log.Message(def?.modContentPack?.FolderName);
            Log.Message(def?.defName);
            Log.Message(def?.category.ToString());
        }
    }

    [StaticConstructorOnStartup]
    public class ShowMiddleMap : MapComponent
    {
        private IntVec3 strikeLoc = IntVec3.Invalid;
        public  IntVec3 StrikeLoc => this.strikeLoc == IntVec3.Invalid ? (this.strikeLoc = new IntVec3(this.map.Size.x / 2, 0, this.map.Size.z / 2)) : this.strikeLoc;


        public ShowMiddleMap(Map map) : base(map)
        {
        }

        public override void MapComponentUpdate()
        {
            base.MapComponentUpdate();
            if (Find.CurrentMap == this.map && DebugMod.Instance.Settings.showMiddleMap)
            {
                GenDraw.DrawRadiusRing(this.StrikeLoc, DebugMod.Instance.Settings.radius, DebugMod.Instance.Settings.color);
            }
        }
    }

    public class DebugModSettings : ModSettings
    {
        public bool  showMiddleMap = true;
        public float radius        = 2f;
        public Color color         = Color.white;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.showMiddleMap, nameof(this.showMiddleMap), true);
            Scribe_Values.Look(ref this.radius,        nameof(this.radius), 2f);

            Scribe_Values.Look(ref this.color,         nameof(this.color), Color.white);
        }
    }

    public class DebugMod : Mod
    {
        public static DebugMod Instance { get; private set; }
        private DebugModSettings settings;

        public DebugModSettings Settings
        {
            get => this.settings ??= this.GetSettings<DebugModSettings>();
        }

        public override string SettingsCategory() => "Middle of the Map Marker";

        public DebugMod(ModContentPack content) : base(content) => 
            Instance = this;

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);

            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("Show middle", ref this.Settings.showMiddleMap);
            this.Settings.radius = listingStandard.SliderLabeled("radius", this.Settings.radius, 0.25f, 10f);
            listingStandard.Gap(20f);
            listingStandard.Label("Color");
            Widgets.DrawBoxSolid(listingStandard.GetRect(50f), this.Settings.color);
            
            if (listingStandard.ButtonText("Change Color"))
            {
                Find.WindowStack.Add(new Dialog_ColorPickerMiddleOfMap());
            }
            
            listingStandard.End();
        }
    }

    public class Dialog_ColorPickerMiddleOfMap : Dialog_ColorPickerBase
    {
        public Dialog_ColorPickerMiddleOfMap() : base(Widgets.ColorComponents.All, Widgets.ColorComponents.All)
        {
            this.oldColor = DebugMod.Instance.Settings.color;
            this.color = this.oldColor;
        }

        public override Vector2 InitialSize { get; } = new Vector2(600f, 500f);

        protected override void        SaveColor(Color color)
        {
            DebugMod.Instance.Settings.color = color;
        }

        protected override bool        ShowDarklight           => true;
        protected override Color       DefaultColor            => this.oldColor;

        protected override List<Color> PickableColors { get; } =
            Enumerable.Range(0, 1).SelectMany(r => Enumerable.Range(10, 1).SelectMany(g => Enumerable.Range(0, 11).Select(b => Color.HSVToRGB(r++ / 10f, g-- / 10f, b / 10f)))).ToList();
        protected override float ForcedColorValue
        {
            get
            {
                Color.RGBToHSV(this.color, out _, out _, out float v);
                return v;
            }
        }

        protected override bool  ShowColorTemperatureBar => true;
    }


    public class DummyDef : ThingDef
    {
        public DummyDef() : base()
        {
            //Log.Message("Patch");
            //Harmony harmony = new Harmony("rimworld.erdelf.dummy_checker");
            //harmony.Patch(typeof(ThingDef).GetMethods(AccessTools.all).First(mi => mi.HasAttribute<CompilerGeneratedAttribute>() && mi.Name.Contains("PostLoad")), prefix: new HarmonyMethod(typeof(RWDebug), nameof(RWDebug.Prefix)));
            //harmony.Patch(AccessTools.Method(AccessTools.TypeByName("KCSG.StartupActions"), "AddDef"), prefix: new HarmonyMethod(typeof(RWDebug), nameof(Prefix)));
        }

        public static void Prefix()
        {
            string stackTrace = StackTraceUtility.ExtractStackTrace();
            RWDebug.stacktraces.Add(stackTrace);
            RWDebug.callCount++;
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
