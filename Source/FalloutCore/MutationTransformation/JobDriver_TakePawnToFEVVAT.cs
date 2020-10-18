using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MutationTransformation
{
    public class JobDriver_TakePawnToFEVVAT : JobDriver
    {
        public JobDriver_TakePawnToFEVVAT()
        {
            this.rotateToFace = TargetIndex.B;
        }
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null, errorOnFailed);
            return this.pawn.Reserve(this.job.targetB, this.job, 1, -1, null, errorOnFailed);
        }
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOn(() => ((Building_CryptosleepCasket)this.TargetB.Thing).HasAnyContents);
            yield return Toils_Reserve.Reserve(TargetIndex.A, 1, -1, null);
            yield return Toils_Reserve.Reserve(TargetIndex.B, 1, -1, null);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
            yield return Toils_Haul.StartCarryThing(TargetIndex.A, false, false);
            yield return Toils_Haul.CarryHauledThingToCell(TargetIndex.B);
            yield return new Toil
            {
                initAction = delegate ()
                {
                    IStrippable strippable = TargetA.Thing as IStrippable;
                    if (strippable != null)
                    {
                        strippable.Strip();
                    }
                }
            };
            yield return Toils_Haul.DepositHauledThingInContainer(TargetIndex.B, TargetIndex.A);
            yield return Toils_Reserve.Release(TargetIndex.A);
            yield return Toils_Reserve.Release(TargetIndex.B);
            yield break;
        }
    }
}

