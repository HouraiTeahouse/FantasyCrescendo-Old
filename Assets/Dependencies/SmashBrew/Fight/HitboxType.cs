namespace HouraiTeahouse.SmashBrew {

    public enum HitboxType {

        // The values here are used as priority mulitpliers
        Inactive = 1,
        Offensive = Inactive << 1,
        Damageable = Offensive << 1,
        Invincible = Damageable << 1,
        Intangible = Invincible << 1,
        Shield = Intangible << 1,
        Absorb = Shield << 1,
        Reflective = Absorb << 1

    }

}
