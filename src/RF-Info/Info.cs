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

public enum ModType
{
    MORE,
    INC
}
public enum ValueType
{
    PLAYER,
    SUPPORT
}

class Info
{
    public const string RF_SKILL_INTERNAL_NAME = "righteous_fire";
    public const string RF_SKILL_NAME = "RighteousFire";

    public static PoeHUD.Models.Enums.GameStat[] RF_STAT_INCREASED_DAMAGE = {
        PoeHUD.Models.Enums.GameStat.CombinedFireDamageOverTimePosPct,
        PoeHUD.Models.Enums.GameStat.CombinedFireDamagePosPct,
        PoeHUD.Models.Enums.GameStat.CombinedElementalDamagePosPct,
        PoeHUD.Models.Enums.GameStat.CombinedAllDamageOverTimePosPct,
        PoeHUD.Models.Enums.GameStat.CombinedAllAreaDamagePosPct,
        PoeHUD.Models.Enums.GameStat.CombinedAllDamagePosPct
    };

    public static PoeHUD.Models.Enums.GameStat[] RF_STAT_MORE_DAMAGE = {
        PoeHUD.Models.Enums.GameStat.CombinedFireDamageOverTimePosPctFinal,
        PoeHUD.Models.Enums.GameStat.CombinedFireDamagePosPctFinal,
        PoeHUD.Models.Enums.GameStat.CombinedElementalDamagePosPctFinal,
        PoeHUD.Models.Enums.GameStat.CombinedAllDamageOverTimePosPctFinal,
        PoeHUD.Models.Enums.GameStat.CombinedAllAreaDamagePosPctFinal,
        PoeHUD.Models.Enums.GameStat.CombinedAllDamagePosPctFinal
    };

    public static PoeHUD.Models.Enums.GameStat[] RF_SUPPORTS_MORE_DAMAGE = {
        PoeHUD.Models.Enums.GameStat.SupportEfficacyDamageOverTimePosPctFinal,
        PoeHUD.Models.Enums.GameStat.SupportGemElementalDamagePosPctFinal,
        PoeHUD.Models.Enums.GameStat.SupportAreaConcentrateAreaDamagePosPctFinal,
        PoeHUD.Models.Enums.GameStat.CombinedFireDamageOverTimePosPctFinal
    };

    public const string decimalStringFormat = "0.00";
    public const string decimalSymbolStringFormat = "+#.##;-#.##;" + decimalStringFormat;

    private static int rfIndex = -1;

    /**
    //  CLASS INTERNAL USAGE METHODS
    //
    //  Used as bases to get some information of the player
     **/

    /*
     * Return maximum player life
     */
    public static int GetLife()
    {
        return (int)GetPlayerStatValue(GameStat.MaximumLife);
    }

    /*
     * Return maximum player energy shield
     */
    public static int GetES()
    {
        return (int)GetPlayerStatValue(PoeHUD.Models.Enums.GameStat.MaximumEnergyShield);
    }

    /*
     * Return a list of modifiers values
     */
    public static float[] GetValues(ValueType valueType, GameStat[] keys)
    {
        List<float> values = new List<float>();
        foreach (GameStat stat in keys)
        {
            switch (valueType)
            {
                case ValueType.PLAYER:
                    values.Add(GetPlayerStatValue(stat));
                    break;
                case ValueType.SUPPORT:
                    values.Add(GetSupportStatValue(stat));
                    break;
            }
        }

        return values.ToArray();
    }

    /*
     * Return MORE multiplier
     */
    public static float GetMoreMultiplier(float[] values)
    {
        float pct = 1;
        foreach (float value in values)
        {
            pct *= (1 + (value/100));
        }

        return pct;
    }

    /*
     * Return MORE multiplier
     */
    public static float GetIncreasedMultiplier(float[] values)
    {
        float pct = 1;
        foreach (float value in values)
        {
            pct += value/100;
        }

        return pct;
    }

    /*
     * Return the multiplier
     */
    public static float GetMultiplier(ModType modType, float[] modValues)
    {
        switch (modType)
        {
            case ModType.MORE:
                return GetMoreMultiplier(modValues);
            case ModType.INC:
                return GetIncreasedMultiplier(modValues);
            default:
                return 1.0f;
        }
    }

    /*
     * Return Maximum player fire resistance percent
     */
    public static float GetFireResistPct()
    {
        return GetPlayerStatValue(PoeHUD.Models.Enums.GameStat.FireDamageResistancePct);
    }

    /*
     * Return the correct value if player has chosen "Abberath" as minor god
     */
    public static float GetPantheonMinor()
    {
        return GameController.Instance.Game.IngameState.Data.LocalPlayer.GetComponent<Player>().PantheonMinor.Equals(Player.PantheonGod.Abberath) ? -0.05f : 0;
    }

    /*
     * Return if player has Passive "Zealoth's Oath" activated
     * 
     * If has Passive active        -      TRUE
     * Else                         - FALSE
     */
    public static bool IsPlayerWithZealothsOath()
    {
        return GameController.Instance.Game.IngameState.Data.LocalPlayer.GetComponent<Player>().AllocatedPassives.Where(skill => skill.Id.Equals("keystone_life_regen_to_ES_regen1469")).Count() > 0;
    }

    /*
     * Return if player is moving
     * 
     * If moving    -   TRUE
     * Else         -   FALSE
     */
    public static bool IsPlayerMoving()
    {
        return GameController.Instance.Game.IngameState.Data.LocalPlayer.GetComponent<Actor>().isMoving;
    }

    /*
     * Return the Player Stat value correspondig the parameter key
     * 
     * key - GameStat to be returned
     */
    public static float GetPlayerStatValue(GameStat key)
    {
        try
        {
            return GameController.Instance.Game.IngameState.Data.LocalPlayer.GetComponent<Stats>().StatDictionary[key];
        }
        catch
        {
            return 0.0f;
        }
    }

    /*
     * Check if player has the asked stat
     */
     public static bool GetPlayerHasStat(GameStat key)
    {
        try
        {
            return GameController.Instance.Game.IngameState.Data.LocalPlayer.GetComponent<Stats>().StatDictionary[GameStat.DamageRemovedFromManaBeforeLifePct] != null;
        }
        catch
        {
            return false;
        }
    }

    /*
     * Return the Player Stat value correspondig the parameter key
     * 
     * key - GameStat to be returned
     */
    public static float GetSupportStatValue(GameStat key)
    {
        try
        {
            return GameController.Instance.Game.IngameState.Data.LocalPlayer.GetComponent<Actor>().ActorSkills[rfIndex].Stats[key];
        }
        catch
        {
            return 0.0f;
        }
    }

    /*
     * Return if player has MoM
     */
     public static bool IsMoMActive()
    {
        return GetPlayerHasStat(GameStat.DamageRemovedFromManaBeforeLifePct);
    }

    /*
     * Return if player has Righteous Fire socketed
     */
    public static bool IsRFSocketd()
    {
        rfIndex = GameController.Instance.Game.IngameState.Data.LocalPlayer.GetComponent<Actor>().ActorSkills.FindIndex(skill => skill.Name.Equals(Info.RF_SKILL_NAME));
        return rfIndex > -1 ? true : false;
    }
}