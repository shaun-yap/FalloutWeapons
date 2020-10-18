﻿using System;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace FalloutCore
{
	[StaticConstructorOnStartup]
	public class WeatherEvent_ToxicThunderstorm : WeatherEvent_LightningFlash
	{
		public WeatherEvent_ToxicThunderstorm(Map map) : base(map)
		{
		}

		public WeatherEvent_ToxicThunderstorm(Map map, IntVec3 forcedStrikeLoc) : base(map)
		{
			this.strikeLoc = forcedStrikeLoc;
		}

		public override void FireEvent()
		{
			base.FireEvent();
			if (!this.strikeLoc.IsValid)
			{
				this.strikeLoc = CellFinderLoose.RandomCellWith((IntVec3 sq) => GenGrid.Standable(sq, this.map) && !this.map.roofGrid.Roofed(sq), this.map, 1000);
			}
			this.boltMesh = LightningBoltMeshPool.RandomBoltMesh;
			if (!GridsUtility.Fogged(this.strikeLoc, this.map))
			{
				GenExplosion.DoExplosion(this.strikeLoc, this.map, 8f, DamageDefOf.EMP, null, -1, -1f, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false);
				Vector3 loc = this.strikeLoc.ToVector3Shifted();
				for (int i = 0; i < 4; i++)
				{
					MoteMaker.ThrowAirPuffUp(loc, map);
					MoteMaker.ThrowLightningGlow(loc, map, 4.5f);
					MoteMaker.ThrowMicroSparks(loc, map);
				}
			}
			SoundInfo soundInfo = SoundInfo.InMap(new TargetInfo(this.strikeLoc, this.map, false), 0);
			SoundStarter.PlayOneShot(SoundDefOf.Thunder_OnMap, soundInfo);
		}

		public override void WeatherEventDraw()
		{
			Graphics.DrawMesh(this.boltMesh, this.strikeLoc.ToVector3ShiftedWithAltitude(24), Quaternion.identity, FadedMaterialPool.FadedVersionOf(WeatherEvent_ToxicThunderstorm.LightningMat, base.LightningBrightness), 0);
		}

		private IntVec3 strikeLoc = IntVec3.Invalid;

		private Mesh boltMesh;

		private static readonly Material LightningMat = MatLoader.LoadMat("Weather/LightningBolt", -1);
	}
}

