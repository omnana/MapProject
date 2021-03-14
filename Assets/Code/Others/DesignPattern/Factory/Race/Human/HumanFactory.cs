using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanFactory : FoodFactory
{
    public override Food CreateFood()
    {
        return new Apple();
    }

    public override Weapon CreateWeapon()
    {
        throw new System.NotImplementedException();
    }

    public override Hat CreateHat()
    {
        throw new System.NotImplementedException();
    }
}
