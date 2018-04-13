using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeHUD.Plugins;
using PoeHUD.Hud;
using PoeHUD.Poe;
using PoeHUD.Poe.Components;
using PoeHUD.Controllers;
using PoeHUD.Models.Enums;

public class RFSkill
{
    public RFSkill()
    {
        rawRegen = 0;
        dmgTkn = 0;
        rfDegen = 0;
        rfDegenPct = 0;
        rfDps = 0;
        total = 0;
    }
    
    private const float RfDegenLife = 0.90f;
    private const float RfDegenES = 0.70f;

    public float rawRegen;
    public float dmgTkn;
    public float rfDegen;
    public float rfDegenPct;
    public float rfDps;
    public float total;

    public string GetValue(string key)
    {
        try
        {
            switch (key)
            {
                case "Raw Regen":
                    return rawRegen.ToString(Info.decimalStringFormat);
                case "Raw Degen":
                    return rfDegen.ToString(Info.decimalStringFormat);
                case "Dmg Taken":
                    return dmgTkn.ToString(Info.decimalStringFormat) + "%";
                case "Total %":
                    return rfDegenPct.ToString(Info.decimalSymbolStringFormat) + "%";
                case "Total":
                    return total.ToString(Info.decimalSymbolStringFormat);
                case "DoT DPS":
                    return Info.IsRFSocketd() ? rfDps.ToString(Info.decimalStringFormat) : "not found";
                default:
                    return "not found";
            }
        }
        catch (KeyNotFoundException e)
        {
            return "not found";
        }
    }

    public void UpdateInfo()
    {
        rawRegen = GetCorrectRegen();
        rfDegen = GetRFDegen();
        dmgTkn = (GetDamageTaken() * 100);
        rfDegenPct = (GetActualRFDegenPct() * 100);
        rfDps = GetRFDamage();
        total = GetActualRFDegen();
    }
    //  FINAL METHODS
    //
    //  Used to return the definitive value after formulas and verifications was applied

    /*
     * Get the correct regen ratio
     * If zealoth's oath     -      ES
     * Else                  -      LIFE
     */
    private float GetCorrectRegen()
    {
        return Info.IsPlayerWithZealothsOath() ? GetEsRegen() : GetLifeRegen();
    }

    /*
     * Get the correct needed regen
     * If zealoth's oath     -      ES
     * Else                  -      LIFE
     */
    private float GetNeededRegen()
    {
        return Info.IsPlayerWithZealothsOath() ? GetNeededEsRegen() : GetNeededLifeRegen();
    }

    /*
     * Get the correct needed regen percent
     * If zealoth's oath     -      ES
     * Else                  -      LIFE
     */
    private float GetNeededRegenPct()
    {
        return Info.IsPlayerWithZealothsOath() ? GetNeededEsRegenPct() : GetNeededLifeRegenPct();
    }

    //  FORMULA METHODS
    //
    //  Used to calculate the values

    /*
     * Return Life Regenarated per second
     */
    private float GetLifeRegen()
    {
        return Info.GetPlayerStatValue(GameStat.LifeRegenerationRatePerMinute) / 60;
    }

    /*
     * Return how much life regen is needed to
     * not take damage from RF degen
     */
    private float GetNeededLifeRegen()
    {
        return Info.GetLife() * GetNeededLifeRegenPct();
    }

    /*
     * Return how much life regen in percentage is needed to
     * not take damage from RF degen
     */
    private float GetNeededLifeRegenPct()
    {
        return RfDegenLife * GetFireResistMaxDif();
    }

    /*
     * Return Energy Shield Regenarated per second
     */
    private float GetEsRegen()
    {
        return Info.GetPlayerStatValue(GameStat.EnergyShieldRegenerationRatePerMinute) / 60;
    }

    /*
     * Return how much energy shield regen is needed to
     * not take damage from RF degen
     */
    private float GetNeededEsRegen()
    {
        return Info.GetES() * GetNeededEsRegenPct();
    }

    /*
     * Return how much energy shield regen in percentage is needed to
     * not take damage from RF degen
     */
    private float GetNeededEsRegenPct()
    {
        return RfDegenES * GetFireResistMaxDif();
    }

    /*
     * Return total raw degenerate damage
     */
    private float GetRFDegen()
    {
        return ((Info.GetLife() * RfDegenLife) + (Info.GetES() * RfDegenES)) * GetFireResistMaxDif() * GetDamageTaken();
    }

    /*
     * Return the actual degen/regen value
     */
    private float GetActualRFDegen()
    {
        return GetCorrectRegen() - GetRFDegen();
    }

    /*
     * Return the actual degen/regen percent value
     */
    private float GetActualRFDegenPct()
    {
        return GetActualRFDegen() / (Info.IsPlayerWithZealothsOath() ? Info.GetES() : Info.GetLife());
    }

    /*
     * Return the difference between 100% fire resistance and actual maximum fire resistance percentage
     */
    private float GetFireResistMaxDif()
    {
        return (1 - Info.GetFireResistPct() / 100);
    }

    /*
     * Return the multiplier for damage taken
     */
    private float GetDamageTaken()
    {
        return 1.0f - (Info.IsPlayerMoving() ? Info.GetPantheonMinor() : 0.0f);
    }

    /*
     * Return multiplied RF Damage
     */
    private float GetRFDamage()
    {
        float supportMoreMult = Info.GetMultiplier(ModType.MORE, Info.GetValues(ValueType.SUPPORT, Info.RF_SUPPORTS_MORE_DAMAGE));
        float playerMoreMult = Info.GetMultiplier(ModType.MORE, Info.GetValues(ValueType.PLAYER, Info.RF_STAT_MORE_DAMAGE));
        float playerIncMult = Info.GetMultiplier(ModType.INC, Info.GetValues(ValueType.PLAYER, Info.RF_STAT_INCREASED_DAMAGE));

        PoeHUD.DebugPlug.DebugPlugin.LogMsg(GetBaseRFDamage() + " " + supportMoreMult + " " + playerIncMult + " " + playerMoreMult, 1.0f);
        return GetBaseRFDamage() * 
            supportMoreMult * 
            playerIncMult * 
            playerMoreMult;
    }

    /*
     * Return Base RF Damage
     */
    private float GetBaseRFDamage()
    {
        return (Info.GetLife() + Info.GetES()) * 0.4f;
    }
}