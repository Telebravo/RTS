using UnityEngine;
using System.Collections;

public enum Weapons { Unarmed, HK416, AK47, Minimi, TankCannon, ArtCannon }
public enum DamageType { Kinetic, Explosive }

public class Weapon
{
    public string displayName;
    public int damage;
    public int firerate;
    public int range;
    public int minRange = 0;
    public int burstLength;
    public DamageType damageType;

    public static Weapon Get(Weapons weapon)
    {
        switch (weapon)
        {
            case Weapons.Unarmed:
                return new Unarmed();
            case Weapons.HK416:
                return new HK416N();
            case Weapons.AK47:
                return new AK47();
            case Weapons.Minimi:
                return new Minimi();
            case Weapons.TankCannon:
                return new TankCannon();
            case Weapons.ArtCannon:
                return new ArtCannon();
        }
        return null;
    }
}
public class Unarmed : Weapon
{
    public Unarmed()
    {
        displayName = "Unarmed";
        damage = 0; ;
        firerate = 1;
        range = 1;
        burstLength = 0;
        damageType = DamageType.Kinetic;
    }
}
public class HK416N : Weapon
{
    public HK416N()
    {
        displayName = "HK416N";
        damage = 50;;
        firerate = 30;
        range = 30;
        burstLength = 0;
        damageType = DamageType.Kinetic;
    }
}
public class AK47 : Weapon
{
    public AK47()
    {
        displayName = "AK-47";
        damage = 60;
        firerate = 30;
        range = 350;
        burstLength = 0;
        damageType = DamageType.Kinetic;
    }
}
public class Minimi : Weapon
{
    public Minimi()
    {
        displayName = "FN Minimi";
        damage = 60;
        firerate = 300;
        range = 800;
        burstLength = 0;
        damageType = DamageType.Kinetic;
    }
}
public class TankCannon : Weapon
{
    public TankCannon()
    {
        displayName = "Tank Cannon";
        damage = 1000;
        firerate = 6; //Skudd i minuttet
        range = 60;
        burstLength = 0;
        damageType = DamageType.Explosive;
    }
}
public class ArtCannon : Weapon
{
    public ArtCannon()
    {
        displayName = "Artilery Cannon";
        damage = 5000;
        firerate = 4; //Skudd i minuttet
        range = 500;
        minRange = 20;
        burstLength = 0;
        damageType = DamageType.Explosive;
    }
}