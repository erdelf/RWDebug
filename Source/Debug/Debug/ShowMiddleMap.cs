using RimWorld;
using Verse;

namespace Debug
{
    using HarmonyLib;
    using UnityEngine;

    [StaticConstructorOnStartup]
    public class ShowMiddleMap : MapComponent
    {
        private IntVec3 strikeLoc = IntVec3.Invalid;
        private static readonly Material LightningMat = MatLoader.LoadMat("Weather/LightningBolt");

        private Mesh strikeMesh;
        public Mesh StrikeMesh => this.strikeMesh ?? (this.strikeMesh = LightningBoltMeshPool.RandomBoltMesh);

        public IntVec3 StrikeLoc => this.strikeLoc == IntVec3.Invalid ? (this.strikeLoc = new IntVec3(this.map.Size.x / 2, 0, this.map.Size.z / 2)) : this.strikeLoc;


        public ShowMiddleMap(Map map) : base(map)
        {
        }

        public override void MapComponentUpdate()
        {
            base.MapComponentUpdate();
            if (Find.CurrentMap == this.map)
            {
                GenDraw.DrawRadiusRing(StrikeLoc, 2f);
            }
        }
    }


    public class DummyDef : ThingDef
    {
        public DummyDef() : base()
        {
            //Log.Message("Patch");
            Harmony harmony = new Harmony("rimworld.erdelf.dummy_checker");
            //harmony.Patch(typeof(ThingDef).GetMethods(AccessTools.all).First(mi => mi.HasAttribute<CompilerGeneratedAttribute>() && mi.Name.Contains("PostLoad")), prefix: new HarmonyMethod(typeof(ShowMiddleMap), nameof(ShowMiddleMap.Prefix)));
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
