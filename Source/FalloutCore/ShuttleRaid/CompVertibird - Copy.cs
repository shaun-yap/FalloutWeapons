using RimWorld;
using ShuttleRaid;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace FalloutCore
{
    public class ScenPart_PlayerPawnsArriveMethodInVertibids : ScenPart
    {
        private bool doVertibirdArrival = false;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref doVertibirdArrival, "doVertibirdArrival", false);
        }

        public override string Summary(Scenario scen)
        {
            return "ScenPart_PlayerPawnsArriveMethodInVertibids".Translate();
        }
        public override void GenerateIntoMap(Map map)
        {
            if (Find.GameInitData != null)
            {
                List<Thing> list = new List<Thing>();
                List<Pawn> list2 = new List<Pawn>();
                foreach (ScenPart allPart in Find.Scenario.AllParts)
                {
                    list.AddRange(allPart.PlayerStartingThings());
                }

                foreach (Pawn startingAndOptionalPawn in Find.GameInitData.startingAndOptionalPawns)
                {
                    list2.Add(startingAndOptionalPawn);
                }
                foreach (Thing item in list)
                {
                    if (item.def.CanHaveFaction)
                    {
                        item.SetFactionDirect(Faction.OfPlayer);
                    }
                }

                foreach (Pawn pawn in list2)
                {
                    if (pawn.def.CanHaveFaction)
                    {
                        pawn.SetFactionDirect(Faction.OfPlayer);
                    }
                }
                Arrive(list, list2, map);
            }
        }

        public void Arrive(List<Thing> things, List<Pawn> pawns, Map map)
        {
            var skyfallers = new List<Skyfaller>();
            var shuttles = new List<Thing>();
            while (pawns.Any())
            {
                var group = pawns.Take(4);
                pawns = pawns.Skip(4).ToList();
                var shuttle = ThingMaker.MakeThing(ThingDef.Named("Vertibird"), null);
                var compTransporter = ThingCompUtility.TryGetComp<CompTransporter>(shuttle);
                foreach (Thing thing in group)
                {
                    compTransporter.innerContainer.TryAdd(thing, 1);
                }
                skyfallers.Add(SkyfallerMaker.MakeSkyfaller(ThingDef.Named("VertibirdIncoming"), shuttle));
                shuttle.SetFaction(Faction.OfPlayer);
                shuttles.Add(shuttle);
            }

            while (things.Any())
            {
                var group = things.Take(8);
                things = things.Skip(8).ToList();
                foreach (Thing thing in group)
                {
                    var shuttleCandidate = shuttles.RandomElement();
                    var compTransporter = ThingCompUtility.TryGetComp<CompTransporter>(shuttleCandidate);
                    compTransporter.innerContainer.TryAdd(thing, 1);
                }
            }

            foreach (var skyfaller in skyfallers)
            {
                IntVec3 dropCenter = DropCellFinder.FindRaidDropCenterDistant_NewTemp(map);
                GenPlace.TryPlaceThing(skyfaller, dropCenter, map, ThingPlaceMode.Near, null, null, default(Rot4));
            }

            foreach (var shuttle in shuttles)
            {
                var comp = shuttle.TryGetComp<CompVertibird>();
                var compTransporter = ThingCompUtility.TryGetComp<CompTransporter>(shuttle);
                comp.requiredColonistCount = 0;
                if (shuttle.Spawned)
                {
                    if (!compTransporter.LoadingInProgressOrReadyToLaunch)
                    {
                        TransporterUtility.InitiateLoading(Gen.YieldSingle<CompTransporter>(compTransporter));
                    }
                }
            }
        }
        public override void PostMapGenerate(Map map)
        {
            if (Find.GameInitData != null)
            {
                PawnUtility.GiveAllStartingPlayerPawnsThought(ThoughtDefOf.CrashedTogether);
            }
        }
    }
}