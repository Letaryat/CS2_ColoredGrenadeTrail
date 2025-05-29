# CS2_GrenadeTrail
Plugin that creates a nade trail after HE Grenade is thrown. To use this plugin you need to create special addon with TrailModel which is located in TrailModel/ in this repository. Then change Model path in plugin.<br>
As there are a lot of plugins that create trail effect, I thought about creating a particle based on already existing one made by Valve (particles/ui/hud/ui_map_def_utility_trail.vpcf) which colors could be changed via C#, since there are not many "useful" ones in the game or provided for free (or at least I cannot find anything). Plugin is an example how to change colors via C# of this particular particle.<br>

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

I would like to thanks my Mom, Dad, my cat and also guys from CSS discord who helped me to figure this out:
- Exkludera,
- Prefix,
- KeePassXC
