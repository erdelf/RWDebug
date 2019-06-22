using Harmony;
using RimWorld;
using System;
using Verse;

namespace Debug
{
    using System.Linq;
    using JetBrains.Annotations;
    using UnityEngine;

    [StaticConstructorOnStartup]
    public class Debug
    {
        static Debug()
        {
            
        }
    }

    [UsedImplicitly]
    public class RWBY_CloneComp : ThingComp
    {
        public delegate void Action<T1, T2, T3, T4, T5, T6, T7, T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

        private Rot4 rotation;

        private PawnRenderer renderer;

        private int disappearTick;

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props: props);

            this.SetData(pawn: Find.CurrentMap.mapPawns.FreeColonists.RandomElement(), ticksToLive: GenDate.TicksPerHour, color: Color.red);
        }

        public void SetData(Pawn pawn, int ticksToLive, Color color)
        {
            Color colorTwo = color;
            this.disappearTick = GenTicks.TicksGame + ticksToLive;
            this.renderer = new PawnRenderer(pawn: pawn)
                            {
                                graphics =
                                {
                                    apparelGraphics = pawn.Drawer.renderer.graphics.apparelGraphics
                                                       .Select(selector: apr =>
                                                                             new
                                                                                 ApparelGraphicRecord(graphic: apr.graphic.GetColoredVersion(newShader: apr.graphic.Shader, newColor: color, newColorTwo: colorTwo),
                                                                                                      sourceApparel: apr.sourceApparel)).ToList(),
                                    hairGraphic  = pawn.Drawer.renderer.graphics.hairGraphic,
                                    headGraphic  = pawn.Drawer.renderer.graphics.headGraphic,
                                    nakedGraphic = pawn.Drawer.renderer.graphics.nakedGraphic,
                                }
                            };

            this.renderer.graphics.hairGraphic =
                this.renderer.graphics.hairGraphic.GetColoredVersion(newShader: this.renderer.graphics.hairGraphic.Shader, newColor: color, newColorTwo: colorTwo);
            this.renderer.graphics.headGraphic =
                this.renderer.graphics.headGraphic.GetColoredVersion(newShader: this.renderer.graphics.headGraphic.Shader, newColor: color, newColorTwo: colorTwo);
            this.renderer.graphics.nakedGraphic =
                this.renderer.graphics.nakedGraphic.GetColoredVersion(newShader: this.renderer.graphics.nakedGraphic.Shader, newColor: color, newColorTwo: colorTwo);


            this.rotation = pawn.Rotation;
            this.set = Delegate.CreateDelegate(type: typeof(Action<Vector3, float, bool, Rot4, Rot4, RotDrawMode, bool, bool>),
                                               firstArgument: this.renderer,
                                               method: AccessTools.Method(type: typeof(PawnRenderer), name: "RenderPawnInternal",
                                                                          parameters: new[]
                                                                                      {
                                                                                          typeof(Vector3), typeof(float), typeof(bool), typeof(Rot4),
                                                                                          typeof(Rot4), typeof(RotDrawMode), typeof(bool), typeof(bool)
                                                                                      }));
        }

        private Delegate set;

        public override void PostDraw()
        {
            base.PostDraw();
            this.set.DynamicInvoke(this.parent.DrawPos, 0f, true, this.rotation, this.rotation, RotDrawMode.Fresh, false, false);

            if (GenTicks.TicksGame <= this.disappearTick) return;

            Map     map = this.parent.Map;
            Vector3 vec = this.parent.DrawPos;

            for (int i = 0; i < 5; i++)
            {
                MoteMaker.ThrowSmoke(loc: vec, map: map, size: 5f     * Rand.Value);
                MoteMaker.ThrowDustPuff(loc: vec, map: map, scale: 5f * Rand.Value);
                MoteMaker.ThrowDustPuff(loc: vec, map: map, scale: 5f * Rand.Value);
            }

            this.parent.Destroy();
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