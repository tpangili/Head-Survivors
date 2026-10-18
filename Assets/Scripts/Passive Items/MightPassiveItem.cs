using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE: The Might stat refers to extra damage
// added to a weapon's base damage stat
public class MightPassiveItem : PassiveItem
{
    protected override void ApplyModifier()
    {
        player.currentMight *= 1 + passiveItemData.Multiplier / 100f;
    }
}
