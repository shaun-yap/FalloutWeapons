using System;
using System.Text;
using RimWorld;
using Verse;

namespace Special
{
	public class SpecialComp : ThingComp
	{
		public int Strength;
		public int Perception;
		public int Endurance;
		public int Charisma;
		public int Intelligence;
		public int Agility;
		public int Luck;

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look<int>(ref this.Strength, "Strength", 1);
			Scribe_Values.Look<int>(ref this.Perception, "Perception", 1);
			Scribe_Values.Look<int>(ref this.Endurance, "Endurance", 1);
			Scribe_Values.Look<int>(ref this.Charisma, "Charisma", 1);
			Scribe_Values.Look<int>(ref this.Intelligence, "Intelligence", 1);
			Scribe_Values.Look<int>(ref this.Agility, "Agility", 1);
			Scribe_Values.Look<int>(ref this.Luck, "Luck", 1);
		}
	}
}

