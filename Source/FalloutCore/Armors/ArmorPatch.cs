using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Quests;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace FalloutCore
{
    public static class ArmorPatch
    {
        [HarmonyPatch(typeof(ArmorUtility), "GetPostArmorDamage")]
        internal class ArmorPatchUtil
        {
            [HarmonyPrefix]
            private static bool PreFix(ref float __result, Pawn pawn, float amount, float armorPenetration, 
                BodyPartRecord part, ref DamageDef damageDef, out bool deflectedByMetalArmor, 
                out bool diminishedByMetalArmor)
            {
                //Log.Message("---------------------------");
                //Log.Message("Hit part: " + part);
                //foreach (var p in part.parts)
                //{
                //    Log.Message("Hit parts: " + p.def);
                //}
                //foreach (var p in part.groups)
                //{
                //    Log.Message("Hit groups: " + p);
                //}
                deflectedByMetalArmor = false;
                diminishedByMetalArmor = false;
                if (pawn?.apparel?.WornApparel?.Where(x => x.def.HasModExtension<AdditionalArmorProtection>()).Count() > 0)
                {
                    if (damageDef.armorCategory == null)
                    {
                        __result = amount;
                    }
                    StatDef armorRatingStat = damageDef.armorCategory.armorRatingStat;
                    if (pawn.apparel != null)
                    {
                        List<Apparel> wornApparel = pawn.apparel.WornApparel;
                        for (int i = wornApparel.Count - 1; i >= 0; i--)
                        {
                            Apparel apparel = wornApparel[i];
                            if (apparel.def.apparel.CoversBodyPart(part))
                            {
                                //Log.Message("Main armor Damage amount before: " + amount);
                                float num = amount;
                                bool flag;
                                ArmorPatchUtil.ApplyArmor(ref amount, armorPenetration,
                                        apparel.GetStatValue(armorRatingStat, true), apparel, ref damageDef, pawn, out flag);
                                if (amount < 0.001f)
                                {
                                    deflectedByMetalArmor = flag;
                                    __result = 0f;
                                }
                                if (amount < num && flag)
                                {
                                    diminishedByMetalArmor = true;
                                }
                                //Log.Message("Main Armor Damage amount after: " + amount);
                            }
                            if (apparel.def.HasModExtension<AdditionalArmorProtection>())
                            {
                                Log.Message(apparel + " has ModExtension");
                                foreach (var data in apparel.def.GetModExtension<AdditionalArmorProtection>().additionalArmors)
                                {
                                    if (ArmorPatchUtil.CoversBodyPart(part, data.bodyPart))
                                    {
                                        foreach (var stat in data.ArmorStats)
                                        {
                                            if (stat.stat == armorRatingStat)
                                            {
                                                //Log.Message("Hit part: " + part + ", covers :" + data.bodyPart);
                                                //Log.Message("Applied stat: " + stat.stat + ", stat value: " + stat.value);
                                                //Log.Message("Additional Armor Damage amount before: " + amount);
                                                float num = amount;
                                                bool flag;
                                                ArmorPatchUtil.ApplyArmor(ref amount, armorPenetration,
                                                        stat.value, apparel, ref damageDef, pawn, out flag);
                                                if (amount < 0.001f)
                                                {
                                                    deflectedByMetalArmor = flag;
                                                    __result = 0f;
                                                }
                                                if (amount < num && flag)
                                                {
                                                    diminishedByMetalArmor = true;
                                                }
                                                //Log.Message("Additional Armor Damage amount after: " + amount);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    float num2 = amount;
                    bool flag2;
                    ArmorPatchUtil.ApplyArmor(ref amount, armorPenetration, pawn.GetStatValue(armorRatingStat, true), null, ref damageDef, pawn, out flag2);
                    if (amount < 0.001f)
                    {
                        deflectedByMetalArmor = flag2;
                        __result = 0f;
                    }
                    if (amount < num2 && flag2)
                    {
                        diminishedByMetalArmor = true;
                    }
                    __result = amount;
                    return false;
                }
                return true;
            }

            public static bool CoversBodyPart(BodyPartRecord partRec, BodyPartGroupDef bodyPart)
            {
                for (int i = 0; i < partRec.groups.Count; i++)
                {
                    //Log.Message("bodyPart == partRec.groups[i] - " + bodyPart + " == " + partRec.groups[i]);
                    if (bodyPart == partRec.groups[i])
                    {
                        return true;
                    }
                }
                return false;
            }
            private static void ApplyArmor(ref float damAmount, float armorPenetration, float armorRating, Thing armorThing, ref DamageDef damageDef, Pawn pawn, out bool metalArmor)
            {
                if (armorThing != null)
                {
                    metalArmor = (armorThing.def.apparel.useDeflectMetalEffect || (armorThing.Stuff != null && armorThing.Stuff.IsMetal));
                }
                else
                {
                    metalArmor = pawn.RaceProps.IsMechanoid;
                }
                if (armorThing != null)
                {
                    float f = damAmount * 0.25f;
                    armorThing.TakeDamage(new DamageInfo(damageDef, (float)GenMath.RoundRandom(f), 0f, -1f, null, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null));
                }
                float num = Mathf.Max(armorRating - armorPenetration, 0f);
                float value = Rand.Value;
                float num2 = num * 0.5f;
                float num3 = num;
                if (value < num2)
                {
                    damAmount = 0f;
                    return;
                }
                if (value < num3)
                {
                    damAmount = (float)GenMath.RoundRandom(damAmount / 2f);
                    if (damageDef.armorCategory == DamageArmorCategoryDefOf.Sharp)
                    {
                        damageDef = DamageDefOf.Blunt;
                    }
                }
            }
        }
    }
}

