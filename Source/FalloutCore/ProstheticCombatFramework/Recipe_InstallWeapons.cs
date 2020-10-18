using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace OrenoPCF
{
	public class Recipe_InstallWeapons : Recipe_Surgery
	{
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
		{
			return MedicalRecipesUtility.GetFixedPartsToApplyOn(recipe, pawn, delegate (BodyPartRecord record)
			{
				IEnumerable<Hediff> source = pawn.health.hediffSet.hediffs.Where((Hediff x) => x.Part == record);
				if (source.Count() == 1 && source.First().def == recipe.addsHediff)
				{
					return false;
				}
				if (record.parent != null && !pawn.health.hediffSet.GetNotMissingParts().Contains(record.parent))
				{
					return false;
				}
				return (!pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(record) || pawn.health.hediffSet.HasDirectlyAddedPartFor(record)) ? true : false;
			});
		}

		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
		{
			bool flag = MedicalRecipesUtility.IsClean(pawn, part);
			bool flag2 = !PawnGenerator.IsBeingGenerated(pawn) && IsViolationOnPawn(pawn, part, Faction.OfPlayer);
			if (billDoer != null)
			{
				if (CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
				{
					return;
				}
				TaleRecorder.RecordTale(TaleDefOf.DidSurgery, billDoer, pawn);
				MedicalRecipesUtility.RestorePartAndSpawnAllPreviousParts(pawn, part, billDoer.Position, billDoer.Map);
				if (flag && flag2 && part.def.spawnThingOnRemoved != null)
				{
					ThoughtUtility.GiveThoughtsForPawnOrganHarvested(pawn);
				}
				if (flag2)
				{
					ReportViolation(pawn, billDoer, pawn.FactionOrExtraMiniOrHomeFaction, -70, "GoodwillChangedReason_NeedlesslyInstalledWorseBodyPart".Translate(recipe.addsHediff.label));
				}
			}
			else if (pawn.Map != null)
			{
				MedicalRecipesUtility.RestorePartAndSpawnAllPreviousParts(pawn, part, pawn.Position, pawn.Map);
			}
			else
			{
				pawn.health.RestorePart(part);
			}
			var hediff = HediffMaker.MakeHediff(recipe.addsHediff, pawn, part);
			pawn.health.AddHediff(hediff);
			var comp = hediff.TryGetComp<HediffComp_VerbGiverExtended>();
			var equipment = ingredients.Where(x => x.TryGetComp<CompEquippable>() != null).FirstOrDefault();
			if (comp != null && equipment != null)
			{
				comp.rangedVerb.verbTracker.directOwner = equipment.TryGetComp<CompEquippable>();
			}
		}

		public override bool IsViolationOnPawn(Pawn pawn, BodyPartRecord part, Faction billDoerFaction)
		{
			if ((pawn.Faction == billDoerFaction || pawn.Faction == null) && !pawn.IsQuestLodger())
			{
				return false;
			}
			if (recipe.addsHediff.addedPartProps != null && recipe.addsHediff.addedPartProps.betterThanNatural)
			{
				return false;
			}
			return HealthUtility.PartRemovalIntent(pawn, part) == BodyPartRemovalIntent.Harvest;
		}
	}
}
