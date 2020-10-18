using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace MutationTransformation
{
	public class FEV_Vat : Building_CryptosleepCasket
	{
		public bool IsOperating
		{
			get
			{
				CompPowerTrader compPowerTrader = this.powerTrader;
				if (compPowerTrader == null || compPowerTrader.PowerOn)
				{
					CompBreakdownable compBreakdownable = this.breakdownable;
					return compBreakdownable == null || !compBreakdownable.BrokenDown;
				}
				return false;
			}
		}

		public Pawn InnerPawn
		{
			get
			{
				return this.innerContainer.FirstOrDefault<Thing>() as Pawn;
			}
		}

		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			this.powerTrader = base.GetComp<CompPowerTrader>();
			this.breakdownable = base.GetComp<CompBreakdownable>();
		}

		public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
		{
			if (!ReachabilityUtility.CanReach(myPawn, this, PathEndMode.InteractionCell, Danger.Deadly, false, 0))
			{
				FloatMenuOption floatMenuOption = new FloatMenuOption(Translator.Translate("CannotUseNoPath"), null,
					MenuOptionPriority.Default, null, null, 0f, null, null);
				yield return floatMenuOption;
			}
			else if (!this.IsOperating)
			{
				FloatMenuOption floatMenuOption2 = new FloatMenuOption(Translator.Translate("CannotUseNotOperating"),
					null, MenuOptionPriority.Default, null, null, 0f, null, null);
				yield return floatMenuOption2;
			}
			else if (this.ContainedThing == null)
			{
				var jobDef = JobDefOf.EnterCryptosleepCasket;
				string label = "EnterToVat".Translate();
				Action action = delegate ()
				{
					Job job = JobMaker.MakeJob(jobDef, this);
					job.count = 1;
					myPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
				};
				yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption
						(label, action, MenuOptionPriority.Default, null, null, 0f, null, null), myPawn,
						this, "ReservedBy");
			}
			yield break;
		}

		public void DoMutation(string pawnKindDefname)
		{
			PawnKindDef pawnKindDef = PawnKindDef.Named(pawnKindDefname);
			Pawn mutant = FCUtils.GetPawnDuplicate(this.InnerPawn, pawnKindDef);
			ThingDef filth_Slime = ThingDefOf.Filth_Slime;
			if (mutant.Faction != Faction.OfPlayer)
			{
				mutant.SetFaction(Faction.OfPlayer);
				var letterLabel = "LetterLabelMessageRecruitSuccess".Translate() + ": " + mutant.LabelShortCap;
				Find.LetterStack.ReceiveLetter(letterLabel, letterLabel, LetterDefOf.PositiveEvent, mutant, null, null, null, null);
			}
			this.innerContainer.ClearAndDestroyContents(DestroyMode.Vanish);
			GenSpawn.Spawn(mutant, this.Position, this.Map);
			mutant.filth.GainFilth(filth_Slime);
			this.innerContainer.TryAdd(mutant);
			PortraitsCache.SetDirty(mutant);
			PortraitsCache.PortraitsCacheUpdate();
		}

		public override void DrawAt(Vector3 drawLoc, bool flip = false)
		{
			var glass = GraphicDatabase.Get<Graphic_Single>("Building/FEVvatglass", ShaderDatabase.MetaOverlay,
					new Vector3(6, 6), Color.white);
			if (this.innerContainer != null && this.innerContainer.Count > 0 && (this.ContainedThing is Pawn || this.ContainedThing is Corpse))
			{
				Vector3 newPos = drawLoc;
				newPos.z += 0.5f;
				if (this.ContainedThing is Corpse corpse)
				{
					// failed attempts to make standing corpse graphics, they give a lot of NRE and other errors 
					// perhaps solvable, but it has already taken too much time

					//Graphic nakedGraphic = GraphicDatabase.Get<Graphic_Multi>(corpse.InnerPawn.story.bodyType
					//	.bodyNakedGraphicPath, ShaderDatabase.CutoutSkin, corpse.InnerPawn.ageTracker.CurKindLifeStage
					//	.bodyGraphicData.drawSize, corpse.InnerPawn.story.SkinColor);
					//Graphic headGraphic = GraphicDatabase.Get<Graphic_Multi>(corpse.InnerPawn.story.HeadGraphicPath, ShaderDatabase.CutoutSkin, Vector2.one, corpse.InnerPawn.story.SkinColor);
					//Graphic hairGraphic = GraphicDatabase.Get<Graphic_Multi>(corpse.InnerPawn.story.hairDef.texPath, ShaderDatabase.Cutout, Vector2.one, corpse.InnerPawn.story.hairColor);
					//corpse.InnerPawn.Drawer.renderer.graphics.headGraphic = headGraphic;
					//corpse.InnerPawn.Drawer.renderer.graphics.nakedGraphic = nakedGraphic;
					//corpse.InnerPawn.Drawer.renderer.graphics.hairGraphic = hairGraphic;
					//corpse.InnerPawn.Drawer.renderer.graphics.headGraphic.Draw(drawLoc, Rot4.North, this);
					//corpse.InnerPawn.Drawer.renderer.graphics.nakedGraphic.Draw(drawLoc, Rot4.North, this);
					//corpse.InnerPawn.Drawer.renderer.graphics.hairGraphic.Draw(drawLoc, Rot4.North, this);

					// another failed attempt, i need to get current draw sizes for head, body and hair

					//corpse.InnerPawn.ageTracker = new Pawn_AgeTracker(corpse.InnerPawn);
					//
					//var bodyDrawSize = corpse.InnerPawn.kindDef.lifeStages[corpse.InnerPawn.ageTracker
					//	.CurLifeStageIndex].bodyGraphicData.drawSize;

					//corpse.InnerPawn.Drawer.renderer.graphics.nakedGraphic = GraphicDatabase.Get<Graphic_Multi>(corpse.InnerPawn.story.bodyType.bodyNakedGraphicPath, ShaderDatabase.CutoutSkin, bodyDrawSize, corpse.InnerPawn.story.SkinColor);
					//corpse.InnerPawn.Drawer.renderer.graphics.rottingGraphic = GraphicDatabase.Get<Graphic_Multi>(corpse.InnerPawn.story.bodyType.bodyNakedGraphicPath, ShaderDatabase.CutoutSkin, Vector2.one, PawnGraphicSet.RottingColor);
					//corpse.InnerPawn.Drawer.renderer.graphics.dessicatedGraphic = GraphicDatabase.Get<Graphic_Multi>(corpse.InnerPawn.story.bodyType.bodyDessicatedGraphicPath, ShaderDatabase.Cutout);
					//corpse.InnerPawn.Drawer.renderer.graphics.headGraphic = GraphicDatabaseHeadRecords.GetHeadNamed(corpse.InnerPawn.story.HeadGraphicPath, corpse.InnerPawn.story.SkinColor);
					//corpse.InnerPawn.Drawer.renderer.graphics.desiccatedHeadGraphic = GraphicDatabaseHeadRecords.GetHeadNamed(corpse.InnerPawn.story.HeadGraphicPath, PawnGraphicSet.RottingColor);
					//corpse.InnerPawn.Drawer.renderer.graphics.skullGraphic = GraphicDatabaseHeadRecords.GetSkull();
					//corpse.InnerPawn.Drawer.renderer.graphics.headStumpGraphic = GraphicDatabaseHeadRecords.GetStump(corpse.InnerPawn.story.SkinColor);
					//corpse.InnerPawn.Drawer.renderer.graphics.desiccatedHeadStumpGraphic = GraphicDatabaseHeadRecords.GetStump(PawnGraphicSet.RottingColor);
					//corpse.InnerPawn.Drawer.renderer.graphics.hairGraphic = GraphicDatabase.Get<Graphic_Multi>(corpse.InnerPawn.story.hairDef.texPath, ShaderDatabase.Cutout, Vector2.one, corpse.InnerPawn.story.hairColor);
					//
					//corpse.InnerPawn.Drawer.renderer.graphics.nakedGraphic.Draw(drawLoc, Rot4.North, this);
					//corpse.InnerPawn.Drawer.renderer.graphics.rottingGraphic.Draw(drawLoc, Rot4.North, this);
					//corpse.InnerPawn.Drawer.renderer.graphics.dessicatedGraphic.Draw(drawLoc, Rot4.North, this);
					//corpse.InnerPawn.Drawer.renderer.graphics.headGraphic.Draw(drawLoc, Rot4.North, this);
					//corpse.InnerPawn.Drawer.renderer.graphics.desiccatedHeadGraphic.Draw(drawLoc, Rot4.North, this);
					//corpse.InnerPawn.Drawer.renderer.graphics.skullGraphic.Draw(drawLoc, Rot4.North, this);
					//corpse.InnerPawn.Drawer.renderer.graphics.headStumpGraphic.Draw(drawLoc, Rot4.North, this);
					//corpse.InnerPawn.Drawer.renderer.graphics.desiccatedHeadStumpGraphic.Draw(drawLoc, Rot4.North, this);
					//corpse.InnerPawn.Drawer.renderer.graphics.hairGraphic.Draw(drawLoc, Rot4.North, this);

					//this way you can draw actual graphics of corpses, but they will lie in random positions
					corpse.InnerPawn.Rotation = Rot4.South;
					corpse.InnerPawn.DrawAt(newPos, flip);
				}
				else
				{
					this.ContainedThing.Rotation = Rot4.South;
					this.ContainedThing.DrawAt(newPos, flip);
				}
				base.DrawAt(drawLoc, flip);
				glass.Draw(drawLoc, Rot4.North, this);
			}
			else
			{
				base.DrawAt(drawLoc, flip);
				glass.Draw(drawLoc, Rot4.North, this);
			}
		}

		public override void Tick()
		{
			base.Tick();
			if (this.ContainedThing == null && this.mutationProgress > 0)
			{
				mutationProgress = 0;
			}
			if (this.ContainedThing is Pawn && !this.InnerPawn.Dead && base.GetComp<CompRefuelable>().HasFuel)
			{
				base.GetComp<CompRefuelable>().ConsumeFuel(0.01f);
				if (this.mutationProgress < 1000)
				{
					mutationProgress++;
				}
				else
				{
					mutationProgress = 0;
					float roll = Rand.RangeInclusive(1, 100);
					Log.Message("Roll: " + roll + " - pawn has ToxicSensitivity " + this.InnerPawn.GetStatValue(StatDefOf.ToxicSensitivity));
					roll *= this.InnerPawn.GetStatValue(StatDefOf.ToxicSensitivity, true);
					Log.Message("Roll multiply: " + roll);
					if (roll >= 85)
					{
						Log.Message("Do mutation");
						this.DoMutation("FCPSuperMutant_Colonist");
					}
					else if (roll >= 75 && roll < 85)
					{
						Log.Message("Do mutation (dumb)");
						this.DoMutation("FCPSuperMutant_ColonistDumb");
					}
					else
					{
						Log.Message("Kill");
						this.InnerPawn.Kill(new DamageInfo(DamageDefOf.Burn, 1000f));

						//this.innerContainer.TryDropAll(this.Position, this.Map, ThingPlaceMode.Near);
					}
				}
			}
		}

		public override string GetInspectString()
		{
			return base.GetInspectString() + "\n" + "MutationProgress".Translate() + 
				(((float)this.mutationProgress / 1000f) * 100f).ToString() + "%";
		}
		public override void EjectContents()
		{
			ThingDef filth_Slime = ThingDefOf.Filth_Slime;
			foreach (Thing thing in this.innerContainer)
			{
				Pawn pawn = thing as Pawn;
				if (pawn != null)
				{
					PawnComponentsUtility.AddComponentsForSpawn(pawn);
					pawn.filth.GainFilth(filth_Slime);
				}
			}
			if (!base.Destroyed)
			{
				SoundStarter.PlayOneShot(SoundDefOf.CryptosleepCasket_Eject, 
					SoundInfo.InMap(new TargetInfo(base.Position, base.Map, false), 0));
			}
			this.innerContainer.TryDropAll(this.InteractionCell, base.Map, ThingPlaceMode.Near, null, null);
			this.contentsKnown = true;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.mutationProgress, "mutationProgress", 0, true);
		}

		public override bool Accepts(Thing thing)
		{
			return this.innerContainer.Count == 0;
		}

		private CompPowerTrader powerTrader;

		private CompBreakdownable breakdownable;

		public int mutationProgress = 0;
	}
}

