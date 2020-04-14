using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;

public class WallJumpCollectible : CollectibleBase, ICollectible
{
    public override void CollectibleEvent(PlayerController player)
    {
        player.EnableAbility(PlayerAbility.WallJump);
    }
}
