using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;

namespace CS2_GrenadeTrail;

public class CS2_GrenadeTrail : BasePlugin
{
    public override string ModuleName => "CS2-GrenadeTrail Color";

    public override string ModuleVersion => "0.0.2";
    public override string ModuleAuthor => "Letaryat | https://github.com/Letaryat";
    private readonly string[] projectiles = ["hegrenade_projectile", "flashbang_projectile", "smokegrenade_projectile", "decoy_projectile", "molotov_projectile"];

    private readonly string[] modelParticles = [
        "particles/letaryat/grenadeTrail.vpcf",
        "particles/letaryat/grenadeTrailBrokenColors.vpcf",
        "particles/letaryat/grenadeLaserSoftTrail.vpcf",
        "particles/letaryat/grenadeCrackTrail.vpcf",
        "particles/letaryat/grenadeelectric.vpcf",
        "particles/letaryat/grenadewater.vpcf"
    ];

    private int modelToUse = 0;

    private Color TrailColor = Color.FromArgb(255, 87, 216, 128);

    public override void Load(bool hotReload)
    {
        Console.WriteLine("CS2-GrenadeTrail Color Loaded");

        AddCommand("css_randomTrailColor", "Sets random color of trail", OnRandomColor);
        AddCommand("css_changeParticle", "Set trail model to argument (int)", OnChangeParticle);

        RegisterListener<Listeners.OnServerPrecacheResources>((ResourceManifest manifest) =>
        {
            foreach (var particles in modelParticles)
            {
                manifest.AddResource(particles);
            }
        });

        RegisterListener<Listeners.OnEntityCreated>((entity) =>
        {
            // As far as I remember this listener was inspired (probably copied 1:1 lol) from: 
            // github.com/schwarper/cs2-store/blob/main/Store/src/item/items/grenadetrail.cs

            if (!projectiles.Contains(entity.DesignerName)) return;
            CBaseGrenade grenade = new(entity.Handle);
            if (grenade.Handle == IntPtr.Zero) return;
            Server.NextFrame(() =>
            {
                var player = grenade.Thrower.Value?.Controller.Value;
                if (player == null) return;
                CreateNadeTrail(grenade.AbsOrigin!, grenade);
            });
        });
    }

    public void CreateNadeTrail(Vector start, CBaseGrenade grenade)
    {
        // Also inspired ( ͡° ͜ʖ ͡°) by: 
        // https://github.com/ipsvn/ChaseMod/blob/c2c003598aeb7d950a61872942fff89fcc21f553/NadeManager.cs#L149

        CParticleSystem particle = Utilities.CreateEntityByName<CParticleSystem>("info_particle_system")!;

        particle.EffectName = modelParticles[modelToUse];
        particle.Teleport(start, QAngle.Zero, Vector.Zero);

        particle.TintCP = 1;
        particle.Tint = TrailColor;
        particle.StartActive = true;
        particle.DispatchSpawn();

        particle.AcceptInput("SetParent", grenade, particle, "!activator");
        particle.AcceptInput("Start");
    }


    private void OnRandomColor(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (player == null) return;
        var pawn = player.PlayerPawn.Value;
        if (pawn == null || !pawn.IsValid) return;

        Random random = new Random();
        //int a = random.Next(0, 256);
        int r = random.Next(0, 256);
        int g = random.Next(0, 256);
        int b = random.Next(0, 256);

        TrailColor = Color.FromArgb(r, g, b, 255);
        Server.PrintToChatAll($"Changed color to: {TrailColor}");
    }

    [CommandHelper(minArgs: 1, usage: "id", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    private void OnChangeParticle(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (player == null) return;
        var pawn = player.PlayerPawn.Value;
        if (pawn == null || !pawn.IsValid) return;
        var id = Convert.ToInt32(commandInfo.GetArg(1));
        modelToUse = id;
        player.PrintToChat($"Changed to: {modelParticles[id]}");
    }

}