using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxHealthPassiveItem : PassiveItem
{
    protected override void ApplyModifier()
    {
        player.CurrentMaxHealth += passiveItemData.Multiplier;
    }
}
