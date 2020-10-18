using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MutationTransformation
{
    public static class FloatMenuMakerMap_Patch
    {
        public static void TakePawnToFEV_Vat(Pawn pawn, LocalTargetInfo localTargetInfo, List<FloatMenuOption> opts)
        {
            if (localTargetInfo.Thing is Pawn target && (target.IsColonist || target.IsPrisoner ||
                (target.Faction.HostileTo(Faction.OfPlayer) && target.Downed))
                && target.health.hediffSet.hediffs
                .Where(x => x.def.defName == "RadiationProof").Count() == 0 
                && ReservationUtility.CanReserveAndReach(pawn, target, PathEndMode.OnCell,
                Danger.Deadly, 1, -1, null, true))
            {
                var containers = target.Map.listerBuildings.AllBuildingsColonistOfClass
                    <FEV_Vat>().Where(x => x.InnerPawn == null && ReservationUtility.CanReserveAndReach(pawn, x,
                    PathEndMode.OnCell, Danger.Deadly, 1, -1, null, true));
                var containmentBreach = (FEV_Vat)GenClosest.ClosestThing_Global
                    (target.Position, containers, 9999f);
                if (containmentBreach != null)
                {
                    JobDef jobDef = FalloutDefOf.TakePawnToFEVVAT;
                    Action action = delegate ()
                    {
                        Job job = JobMaker.MakeJob(jobDef, target, containmentBreach);
                        job.count = 1;
                        pawn.jobs.TryTakeOrderedJob(job, 0);
                    };
                    string text = TranslatorFormattedStringExtensions.Translate("TakePawnToFEVVAt",
                        target.LabelCap, target);
                    FloatMenuOption opt = new FloatMenuOption
                        (text, action, MenuOptionPriority.RescueOrCapture, null, target, 0f, null, null);
                    if (opts.Where(x => x.Label == text).Count() == 0)
                    {
                        opts.Add(opt);
                    }
                }
                //else
                //{
                //    string text = "NoContainersToTake".Translate();
                //    if (opts.Where(x => x.Label == text).Count() == 0)
                //    {
                //        opts.Add(new FloatMenuOption(text, null, MenuOptionPriority.Default, null, null,
                //            0f, null, null));
                //    }
                //}
        }
    }

        public static void TakePawnToStasis(Pawn pawn, LocalTargetInfo localTargetInfo, List<FloatMenuOption> opts)
        {
            if (localTargetInfo.Thing is Pawn target && ReservationUtility.CanReserveAndReach(pawn, target, PathEndMode.OnCell,
                Danger.Deadly, 1, -1, null, true))
            {
                var containers = target.Map.listerBuildings.AllBuildingsColonistOfClass
                    <Stasis>().Where(x => x.InnerPawn == null && ReservationUtility.CanReserveAndReach(pawn, x, 
                    PathEndMode.OnCell, Danger.Deadly, 1, -1, null, true));
                var containmentBreach = (Stasis)GenClosest.ClosestThing_Global
                    (target.Position, containers, 9999f);
                if (containmentBreach != null)
                {
                    JobDef jobDef = FalloutDefOf.TakePawnToStasis;
                    Action action = delegate ()
                    {
                        Job job = JobMaker.MakeJob(jobDef, target, containmentBreach);
                        job.count = 1;
                        pawn.jobs.TryTakeOrderedJob(job, 0);
                    };
                    string text = TranslatorFormattedStringExtensions.Translate("TakePawnToStasis",
                        target.LabelCap, target);
                    FloatMenuOption opt = new FloatMenuOption
                        (text, action, MenuOptionPriority.RescueOrCapture, null, target, 0f, null, null);
                    if (opts.Where(x => x.Label == text).Count() == 0)
                    {
                        opts.Add(opt);
                    }
                }
                //else
                //{
                //    string text = "NoStasisToTake".Translate();
                //    if (opts.Where(x => x.Label == text).Count() == 0)
                //    {
                //        opts.Add(new FloatMenuOption(text, null, MenuOptionPriority.Default, null, null,
                //            0f, null, null));
                //    }
                //}
            }
        }
        [HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
        public static class AddHumanlikeOrders_Patch
        {
            [HarmonyPostfix]
            public static void AddHumanlikeOrdersPostfix(Vector3 clickPos, Pawn pawn, ref List<FloatMenuOption> opts)
            {
                foreach (LocalTargetInfo localTargetInfo in GenUI.TargetsAt(clickPos, TargetingParameters.ForRescue(pawn), true))
                {
                    TakePawnToFEV_Vat(pawn, localTargetInfo, opts);
                    TakePawnToStasis(pawn, localTargetInfo, opts);
                }
                foreach (LocalTargetInfo localTargetInfo in GenUI.TargetsAt(clickPos, TargetingParameters.ForShuttle(pawn), true))
                {
                    TakePawnToFEV_Vat(pawn, localTargetInfo, opts);
                    TakePawnToStasis(pawn, localTargetInfo, opts);
                }
                foreach (LocalTargetInfo localTargetInfo in GenUI.TargetsAt(clickPos, TargetingParameters.ForArrest(pawn), true))
                {
                    TakePawnToFEV_Vat(pawn, localTargetInfo, opts);
                    TakePawnToStasis(pawn, localTargetInfo, opts);
                }
                foreach (LocalTargetInfo localTargetInfo in GenUI.TargetsAt(clickPos, TargetingParameters.ForStrip(pawn), true))
                {
                    TakePawnToFEV_Vat(pawn, localTargetInfo, opts);
                    TakePawnToStasis(pawn, localTargetInfo, opts);
                }
            }
        }
    }
}

