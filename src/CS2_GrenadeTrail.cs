using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
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
        "particles/letaryat/grenadewater.vpcf",
        "particles/letaryat/bullettracer.vpcf",
        "particles/letaryat/bullettracerdwa.vpcf",
        "particles/letaryat/bullettracertrzy.vpcf", //8
        "particles/letaryat/bullettracercztiry.vpcf",
        "particles/letaryat/bullettracerpinc.vpcf",
        "particles/letaryat/bullettracerszes.vpcf", // 11
        "particles/letaryat/bullettracerosiedem.vpcf",
        "particles/letaryat/bullettracerosiem.vpcf", // 13
        "particles/letaryat/bullettrail.vpcf",
    ];

    private int modelToUse = 0;

    private Color TrailColor = Color.FromArgb(255, 87, 216, 128);

    //ty k4ryuu
    public static MemoryFunctionWithReturn<CParticleSystem, int, Vector, bool> SetControlPointValue { get; }
            = new("55 48 89 E5 41 57 41 56 41 55 49 89 D5 31 D2 41 54 41 89 F4");

    public override void Load(bool hotReload)
    {
        Console.WriteLine("CS2-GrenadeTrail Color Loaded");

        AddCommand("css_randomTrailColor", "Sets random color of trail", OnRandomColor);
        AddCommand("css_particle", "Set trail model to argument (int)", OnChangeParticle);

        //Listeners:
        RegisterListener<Listeners.OnServerPrecacheResources>((ResourceManifest manifest) =>
        {
            foreach (var particles in modelParticles)
            {
                manifest.AddResource(particles);
            }
            manifest.AddResource("models/chicken/chicken.vmdl");
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

        //Events:
        RegisterEventHandler<EventBulletImpact>(OnBulletImpact);
    }

    //Methods:
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

    public Vector CalculateRJ(Vector playerPos, Vector bulletPos)
    {
        var distanceVector = new Vector
        {
            X = playerPos.X - bulletPos.X,
            Y = playerPos.Y - bulletPos.Y,
            Z = playerPos.Z - bulletPos.Z
        };
        float distance = (float)Math.Sqrt(
            distanceVector.X * distanceVector.X +
            distanceVector.Y * distanceVector.Y +
            distanceVector.Z * distanceVector.Z
        );
        var normalizedDirection = new Vector
        {
            X = distanceVector.X / distance,
            Y = distanceVector.Y / distance,
            Z = distanceVector.Z / distance
        };
        return new Vector
        {
            X = normalizedDirection.X,
            Y = normalizedDirection.Y,
            Z = normalizedDirection.Z
        };
    }

    //Exkludera bullet impact:
    public static Vector GetEyePosition(CCSPlayerController player)
    {
        Vector absorigin = player.PlayerPawn.Value!.AbsOrigin!;
        CPlayer_CameraServices camera = player.PlayerPawn.Value!.CameraServices!;

        return new Vector(absorigin.X, absorigin.Y, absorigin.Z + camera.OldPlayerViewOffsetZ);
    }

    public void CreateParticleBullet(Vector endPos, Vector start)
    {
        /*
        Control Point = 4 - LifeTime
        Control Point = 5 - Start
        Control Point = 6 - End
        */
        /*
        Vector direction = endPos - start;
        float length = MathF.Sqrt(direction.X * direction.X + direction.Y * direction.Y + direction.Z * direction.Z);

        if (length != 0)
        {
            direction.X /= length;
            direction.Y /= length;
            direction.Z /= length;
        }
        
        direction *= 500.0f;
        */
        var particle = Utilities.CreateEntityByName<CParticleSystem>("info_particle_system")!;

        particle.EffectName = modelParticles[modelToUse];
        particle.Teleport(start);

        SetControlPointValue.Invoke(particle, 5, start);
        SetControlPointValue.Invoke(particle, 6, endPos);


        /*
        particle.DataCP = 2;
        particle.DataCPValue.X = direction.X; particle.DataCPValue.Y = direction.Y; particle.DataCPValue.Z = direction.Z;
        */
        particle.TintCP = 1;
        particle.Tint = TrailColor;
        particle.StartActive = true;

        particle.DispatchSpawn();

        particle.AcceptInput("Start");
    }

    //Commands:
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

    //Events:

    private HookResult OnBulletImpact(EventBulletImpact @event, GameEventInfo info)
    {
        var bullet = @event;
        var player = @event.Userid;
        if (player == null) return HookResult.Continue;
        var pawn = player.PlayerPawn.Value;
        if (pawn == null || !pawn.IsValid) return HookResult.Continue;

        Vector startPos = GetEyePosition(player);
        Vector endPos = new Vector(bullet.X, bullet.Y, bullet.Z);

        Server.PrintToChatAll($"Bullet impact: {endPos}");

        CreateParticleBullet(endPos, startPos);

        return HookResult.Continue;
    }

}