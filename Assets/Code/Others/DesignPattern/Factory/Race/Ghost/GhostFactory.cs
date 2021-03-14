public class GhostFactory : FoodFactory
{
    public override Food CreateFood()
    {
        return new Soul();
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
