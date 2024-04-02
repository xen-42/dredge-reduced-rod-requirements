using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Winch.Core;

namespace ReducedRodRequirements;

[HarmonyPatch]
public class ReducedRodRequirements : MonoBehaviour
{
    public void Awake()
    {
        WinchCore.Log.Debug($"{nameof(ReducedRodRequirements)} has loaded!");

        new Harmony(nameof(ReducedRodRequirements)).PatchAll();
    }

    private static HashSet<HarvestableType> _harvestableTypeCache;

    /* Before running certain methods that use the list, we set it to everything, then we set it back after */

    [HarmonyPrefix]
    [HarmonyPatch(typeof(InteractPointUI), nameof(InteractPointUI.Show))]
    [HarmonyPatch(typeof(HarvestMinigameView), nameof(HarvestMinigameView.RefreshHarvestTarget))]
    public static void Prefix()
    {
        _harvestableTypeCache = GameManager.Instance.PlayerStats.HarvestableTypes;
        GameManager.Instance.PlayerStats.HarvestableTypes = Enum.GetValues(typeof(HarvestableType)).Cast<HarvestableType>().ToHashSet();
        
        // Don't let them skip unlocking the dredge tool
        if (!_harvestableTypeCache.Contains(HarvestableType.DREDGE))
        {
            GameManager.Instance.PlayerStats.HarvestableTypes.Remove(HarvestableType.DREDGE);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(InteractPointUI), nameof(InteractPointUI.Show))]
    [HarmonyPatch(typeof(HarvestMinigameView), nameof(HarvestMinigameView.RefreshHarvestTarget))]
    public static void Postfix()
    {
        GameManager.Instance.PlayerStats.HarvestableTypes = _harvestableTypeCache;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(HarvestMinigameView), nameof(HarvestMinigameView.RefreshHarvestTarget))]
    public static void HarvestMinigameView_RefreshHarvestTarget(HarvestMinigameView __instance)
    {
        // Leave it using the default broken behaviour since that just fishes really slowly
        var isHarvestable = __instance.currentPOI.IsHarvestable;
        // Shows "damaged" but we don't write that
        if (isHarvestable != HarvestQueryEnum.INVALID_NO_STOCK && isHarvestable != HarvestQueryEnum.INVALID_INCORRECT_TIME)
        {
            __instance.cannotStartText.StringReference.SetReference(LanguageManager.STRING_TABLE, "poi-label.insufficient-equipment.fish");
        }
        // Never show the broken equipment overlay since
        __instance.brokenEquipmentOverlay.SetActive(false);
    }
}
