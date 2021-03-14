using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FoodFactory
{
    public abstract Food CreateFood();

    public abstract Weapon CreateWeapon();

    public abstract Hat CreateHat();
}
