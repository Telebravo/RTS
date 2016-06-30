using UnityEngine;
using System.Collections;

public enum Weapons { Unarmed, HK416, Minimi, AK47, RheinmetallL44, ArtCannon }
public enum DamageType { Kinetic, Explosive }

public class Weapon
{
    public string displayName;
    public int damage;
    public int firerate;
    public int range;
    public int burstLength = 0;
    public Ammunition ammo;
    public AmmunitionSize ammoSize;
    public DamageType damageType;

    public static Weapon Get(Weapons weapon)
    {
        switch (weapon)
        {
            case Weapons.Unarmed:
                return new Unarmed();
            case Weapons.HK416:
                return new HK416N();
            case Weapons.Minimi:
                return new Minimi();
            case Weapons.AK47:
                return new AK47();
            case Weapons.RheinmetallL44:
                return new RheinmetallL44();
            case Weapons.ArtCannon:
                return new ArtCannon();
        }
        return null;
    }
    public bool SetAmmo(Ammunition ammo)
    {
        if (ammoSize == ammo.size)
        {
            this.ammo = ammo;
            return true;
        }
        else
        {
            return false;
        }    
    }
}
public class Unarmed : Weapon
{
    public Unarmed()
    {
        displayName = "Unarmed";
        damage = 0;
        firerate = 1;
        range = 1;
    }
}

//////////////////// HÅNDVÅPEN ////////////////////

public class HK416N : Weapon
{
    public HK416N()
    {
        displayName = "HK416N";
        damage = 50;;
        firerate = 30;
        range = 30;
        ammoSize = AmmunitionSize.NATO556x45;
        ammo = new NATO556();
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
        ammoSize = AmmunitionSize.NATO556x45;
        ammo = new NATO556();
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
        ammoSize = AmmunitionSize.NATO762x51;
        ammo = new NATO762();
    }
}

//////////////////// KANONER ////////////////////

public class RheinmetallL44 : Weapon
{
    public RheinmetallL44()
    {
        displayName = "Rheinmetall L/44 120mm";
        damage = 1000;
        firerate = 6;
        range = 60;
        ammoSize = AmmunitionSize.Tank120;
        ammo = new DM63();
    }
}
public class ArtCannon : Weapon
{
    public ArtCannon()
    {
        displayName = "Artilery Cannon";
        damage = 5000;
        firerate = 4;
        range = 500;
        ammoSize = AmmunitionSize.Tank120;
        ammo = new DM63();
    }
}