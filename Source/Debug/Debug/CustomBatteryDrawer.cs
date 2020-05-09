using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomBatteryDrawer
{
    using RimWorld;
    using UnityEngine;
    using Verse;

    [StaticConstructorOnStartup]
    class Building_CustomBatteryDrawer : Building_Battery
    {
        private static readonly Material batteryBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.9f, 0.85f, 0.2f));

        private static readonly Material batteryBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f));

        //[TweakValue("CustomBatteryDrawPosX", -2, 2)]
        private static float CustomBatteryDrawPosX = -0.51f;

        //[TweakValue("CustomBatteryDrawPosY", -2, 2)]
        private static float CustomBatteryDrawPosY = -0.695f;

        //[TweakValue("CustomBatteryDrawSizeX", -3, 3)]
        private static float CustomBatteryDrawSizeX = 0.72f;

        //[TweakValue("CustomBatteryDrawSizeY", -3, 3)]
        private static float CustomBatteryDrawSizeY = 0.28f;


        public override void Draw()
        {
            DrawAt(DrawPos);
            CompPowerBattery           comp = GetComp<CompPowerBattery>();
            GenDraw.FillableBarRequest r    = default(GenDraw.FillableBarRequest);
            r.center      = DrawPos + Vector3.up * 0.1f + new Vector3(CustomBatteryDrawPosX, 0, CustomBatteryDrawPosY);
            r.size        = new Vector2(CustomBatteryDrawSizeX, CustomBatteryDrawSizeY);
            r.fillPercent = comp.StoredEnergy / comp.Props.storedEnergyMax;
            r.filledMat   = batteryBarFilledMat;
            r.unfilledMat = batteryBarUnfilledMat;
            r.margin      = 0.15f;
            Rot4 rotation = base.Rotation;
            rotation.Rotate(RotationDirection.Clockwise);
            r.rotation = rotation;
            GenDraw.DrawFillableBar(r);
            //base.Draw();
        }
    }
}
