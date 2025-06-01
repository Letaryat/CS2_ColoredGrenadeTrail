using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;
using CS2_GrenadeTrail.Managers;
using CS2_GrenadeTrail.Utils;

namespace CS2_GrenadeTrail;

public class CS2_GrenadeTrail : BasePlugin
{
    public override string ModuleName => "CS2-GrenadeTrail Color";

    public override string ModuleVersion => "0.0.2";
    public override string ModuleAuthor => "Letaryat | https://github.com/Letaryat";
    public readonly string[] projectiles = ["hegrenade_projectile", "flashbang_projectile", "smokegrenade_projectile", "decoy_projectile", "molotov_projectile"];

    public readonly string[] modelParticles = [
        "particles/letaryat/grenadeTrail.vpcf",
        "particles/letaryat/grenadeTrailBrokenColors.vpcf",
        "particles/letaryat/grenadeLaserSoftTrail.vpcf",
        "particles/letaryat/grenadeCrackTrail.vpcf",
        "particles/letaryat/grenadeelectric.vpcf",
        "particles/letaryat/grenadewater.vpcf",
    ];

    public readonly string[] tracerParticles = [
        "particles/letaryat/bullettrail.vpcf",
        "particles/letaryat_tracers/bullet_tracer1.vpcf",
        "particles/letaryat_tracers/bullet_tracer_crack.vpcf",
        "particles/letaryat_tracers/bullet_tracer_electric.vpcf",
        "particles/letaryat_tracers/bullet_tracer_water.vpcf",
        "particles/letaryat_tracers/bullettracerosiem.vpcf",
    ];
    public int modelToUse = 0;
    public int tracerToUse = 0;
    public Color TrailColor = Color.FromArgb(255, 87, 216, 128);

    //Managers:
    public EventManager? EventManager { get; private set; }
    public CommandsManager? CommandsManager { get; private set; }

    public PluginUtilities? PluginUtilties { get; private set; }

    public override void Load(bool hotReload)
    {
        Console.WriteLine("CS2-GrenadeTrail Color Loaded");
        //Managers
        EventManager = new EventManager(this);
        CommandsManager = new CommandsManager(this);
        PluginUtilties = new PluginUtilities(this);

        //Register:
        EventManager.RegisterEvents();
        CommandsManager.RegisterCommands();
    }

    public override void Unload(bool hotReload)
    {
        Console.WriteLine("CS2-GrenadeTrail Color Unloaded!");
    }

}