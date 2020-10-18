using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace FalloutCore
{
    [StaticConstructorOnStartup]
    public class WeatherOverlay_ToxicRain : SkyOverlay
    {
        public WeatherOverlay_ToxicRain()
        {
            this.worldOverlayMat = WeatherOverlay_ToxicRain.RainOverlayWorld;
            this.worldOverlayPanSpeed1 = 0.015f;
            this.worldPanDir1 = new Vector2(-0.25f, -1f);
            this.worldPanDir1.Normalize();
            this.worldOverlayPanSpeed2 = 0.022f;
            this.worldPanDir2 = new Vector2(-0.24f, -1f);
            this.worldPanDir2.Normalize();
        }

        public override void TickOverlay(Map map)
        {
            base.TickOverlay(map);
            Log.Message(map.weatherManager.curWeather.defName, true);
            if (map.weatherManager.curWeather.HasModExtension<WeatherEffects>())
            {
                var options = map.weatherManager.curWeather.GetModExtension<WeatherEffects>();
                if (options.ticksInterval > 0)
                {
                    if (Find.TickManager.TicksGame % options.ticksInterval == 0)
                    {
                        DoDamage(options, map);
                    }
                }
                else
                {
                    DoDamage(options, map);
                }
            }
        }

        public void DoDamage(WeatherEffects options, Map map)
        {
            for (int i = map.listerThings.AllThings.Count - 1; i >= 0; i--)
            {
                Thing thing = map.listerThings.AllThings[i];
                if (thing is Pawn pawn && thing.Spawned && !thing.Position.Roofed(map))
                {
                    DoPawnDamage(pawn, options);
                }
                else if (thing.Spawned && !thing.Position.Roofed(map))
                {
                    DoThingDamage(thing, options);
                }
            }
        }

        public void DoPawnDamage(Pawn p, WeatherEffects options)
        {
            if (!p.RaceProps.IsFlesh)
            {
                return;
            }
            foreach (var defName in options.hediffDefnames)
            {
                var hediffDef = HediffDef.Named(defName);
                if (options.severity * p.GetStatValue(StatDefOf.ToxicSensitivity, true) != 0f)
                {
                    Log.Message("Adjusting hediff " + hediffDef + " with severity " 
                        + options.severity * p.GetStatValue(StatDefOf.ToxicSensitivity, true) 
                        + " to a pawn: " + p);
                    HealthUtility.AdjustSeverity(p, hediffDef, options.severity * p.GetStatValue(StatDefOf.ToxicSensitivity, true));
                }
            }
        }

        public void DoThingDamage(Thing thing, WeatherEffects options)
        {
            if (thing is Plant)
            {
                if (Rand.Value < 0.0065f)
                {
                    thing.Kill(null, null);
                }
            }
            else if (thing.def.category == ThingCategory.Item)
            {
                CompRottable compRottable = thing.TryGetComp<CompRottable>();
                if (compRottable != null && compRottable.Stage < RotStage.Dessicated)
                {
                    compRottable.RotProgress += 3000f;
                }
            }
        }

        private static readonly Material RainOverlayWorld = MatLoader.LoadMat("Weather/RainOverlayWorld", -1);
    }
}

