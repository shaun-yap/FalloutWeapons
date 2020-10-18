using System.Collections.Generic;
using RimWorld;
using Verse;

namespace FalloutCollaborationProjectGas
{
    public class ThingThatAddsHediff : Gas
    {

        private int countOfTicks = 0;
        private int ticksEachApplication = 0;

        //I put these lists up here instead of allocating and deallocating memory during running that takes some of that "time"
        //Lists are not the best answer for everything but if they change in size they are better than arrays
         
        private List<Pawn> pawnsInGas = new List<Pawn>();
        public ThingThatAddsHediffDef Def
        {
            get
            {
                return def as ThingThatAddsHediffDef;
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            ticksEachApplication = Def.ticksBetweenApplication;
        }

        public override void Tick()
        {
            if (Destroyed)
                return;

            base.Tick();

            countOfTicks++;
            if (countOfTicks >= ticksEachApplication)
            {
                DoApplication();
                countOfTicks = 0;
            }

        }

        private void DoApplication()
        {

            if (Destroyed)
                return;
            if (Def.hediffToAdd == null)
            {
                Log.Warning("Gas " + Def.defName + " has a null hediff");
                return;
            }
            //Get all pawns in the could

            //First we fill the things, we get our current position and get all things also in the same posistion.
            List<Thing> thingsInGas = base.Position.GetThingList(base.Map);

            //This check is extra, it shouldnt be empty becase this gas is a thing and its in the map position.
            //When checking if something is equal you have to use to = signs otherwise it things you are trying to set the thing as whatever
            // count = 0 means assign 0 to count, count == 0 means check if count is 0 

            //I use a for loop instead of a foreach loop (its faster), here we go through the list of things in the gas
            for (int i = 0; i < thingsInGas.Count; i++)
            {
                //Try to cast each thing as a pawn
                Pawn currentPawn = thingsInGas[i] as Pawn;

                //If pawn is null that thing was not pawn becuase I couldnt cast it, if its already on the list of touching pawns I dont want to add it again
                //!= means NOT EQUAL ! (NOT) = (EQUAL), && Means AND, ! (NOT)  touching 
                //if you have a true false statement and you want to make sure its false use ! rather than this.touchingPawn.Contains(currentPawn)) == false
                //that is a lot harder to type, harder to read, and just plain longer. 
                if (currentPawn != null && !this.pawnsInGas.Contains(currentPawn))
                {

                    this.pawnsInGas.Add(currentPawn);
                    //Try to add the Hediff, I send the function the things from the Def I need so I dont have to grab them each time. 
                    TryAddHediffToPawn(currentPawn, Def.hediffToAdd, Def.severityPerApplication);

                }
            }

            //Go throught the list again as a clean up or you could just clear the list with:
            for (int j = 0; j < this.pawnsInGas.Count; j++)
            {

                Pawn pawnToCheck = this.pawnsInGas[j];
                if (!pawnToCheck.Spawned || pawnToCheck.Position != base.Position)
                {

                    this.pawnsInGas.Remove(pawnToCheck);
                }
            }

        }

        private void TryAddHediffToPawn(Pawn pawnToApplyTo, HediffDef hediffToAdd, float severityToApply)
        {
            //Get the pawns gas protectionand store it 
            float sensitivity = pawnToApplyTo.GetStatValue(StatDefOf.ToxicSensitivity, true);

            //Can this Pawn Be affected?  if !(NOT) get out of here and save us some time, if resistance is 1 (or 100% or higher) than is ensitivity will be 1 or more
            if (!PawnCanBeAffected(pawnToApplyTo) || sensitivity < 1)
                return;

            //Use the hediffMaker to help us make a new hediff use ILSpy to explore what it does 
            Hediff newHediff = HediffMaker.MakeHediff(hediffToAdd, pawnToApplyTo);

            //So if it has the hediff
            if (pawnToApplyTo.health.hediffSet.HasHediff(hediffToAdd))
            {
                // we take the old severity and add the value we want to apply after resistance
                //this math follows the order of operations PEMDAS so the multiple happens first severity * sensitivity
                // if severity is .10 or 10% and sensitivity is .1 or 10% then we get .01 or 1% to add
                // += means takes the old value and add the next value so first we multiple then we add it in this would make it
                // value = value + .01 if 
                pawnToApplyTo.health.hediffSet.GetFirstHediffOfDef(hediffToAdd).Severity += severityToApply * sensitivity;
            }
            else
            {
                //This else will fire if the top part didnt and so here we set the severity to apply
                newHediff.Severity = severityToApply * sensitivity;
                //and apply it
                pawnToApplyTo.health.AddHediff(newHediff);
            }

        }

        //NOTICE this has bool instead of void in front of the Functions name this means I am going to sent a bool back out.
        private bool PawnCanBeAffected(Pawn pawn)
        {
            //This is always true for now, why beucase you dont need to to limit right now, sure this is an extra step but this is very quick and wont slow things down at all.
            return true;
        }
    }
}

