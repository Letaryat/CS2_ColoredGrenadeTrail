# CS2-CustomTrailAndTracers Colored
> [!CAUTION]
> Plugin is not intended for actual "use". It is more of a showcase that we can make custom particles that can be colored using C#.


Plugin that creates a nade trail after grenade is thrown, trail after player and custom bullet tracers. To use this plugin you need to create special addon with either BulletTracers or TrailModels.<br>
As there are a lot of plugins that create trail effect, I thought about creating a particle based on already existing one made by Valve (particles/ui/hud/ui_map_def_utility_trail.vpcf).<br> This version of that particle allows server owners / developers to change colors via C#. BulletTracers allows to change colors, position (start, end) and for some of them to change radius (thicc).
<br>Plugin is an **example** how to change colors, start and end points and radius via C# of particles that use Control Points.<br>

## [📺] Video presentation
https://www.youtube.com/watch?v=I2xDp8Ttw5E

<p align="center">
    <img src="image/pic.jpg" width="500">
</p>

### List of cool trails ingame that I also found:
```
particles/ui/hud/ui_map_def_utility_trail.vpcf - original one
particles/weapons/cs_weapon_fx/bumpmine_active.vpcf_c
particles/water_fx/waterfall_base.vpcf
particles/money_fx/moneybag_trail.vpcf_c
```

I would like to thank my Mom, Dad, my cat and most importantly guys from CSS discord who helped me to figure this out:
- Exkludera,
- Prefix,
- KeePassXC
- Schwarper and his CS2-Store,
- ipsvn and his ChaseMod
