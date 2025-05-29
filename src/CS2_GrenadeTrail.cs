using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace CS2_GrenadeTrail;

public class CS2_GrenadeTrail : BasePlugin
{
    public override string ModuleName => "CS2-GrenadeTrail Color";

    public override string ModuleVersion => "0.0.1";
    public override string ModuleAuthor => "Letaryat | https://github.com/Letaryat";
    public string Model = "particles/letaryat/testparticle.vpcf";
    public override void Load(bool hotReload)
    {
        Console.WriteLine("CS2-GrenadeTrail Color Loaded");

        RegisterListener<Listeners.OnServerPrecacheResources>((ResourceManifest manifest) =>
        {
            manifest.AddResource(Model);
        });

        RegisterListener<Listeners.OnEntityCreated>((entity) =>
        {
            // As far as I remember this listener was inspired (probably copied 1:1 lol) from: 
            // github.com/schwarper/cs2-store/blob/main/Store/src/item/items/grenadetrail.cs
            // Trail should work also with other types of grenades

            if (entity.DesignerName != "hegrenade_projectile") return;
            CBaseCSGrenadeProjectile grenade = new(entity.Handle);
            if (grenade.Handle == IntPtr.Zero) return;
            Server.NextFrame(() =>
            {
                var player = grenade.Thrower.Value?.Controller.Value;
                if (player == null) return;
                CreateNadeTrail(grenade.AbsOrigin!, grenade);
            });
        });
    }

    public void CreateNadeTrail(Vector start, CBaseCSGrenadeProjectile grenade)
    {
        CParticleSystem particle = Utilities.CreateEntityByName<CParticleSystem>("info_particle_system")!;

        particle.EffectName = Model;
        particle.Teleport(start, QAngle.Zero, Vector.Zero);

        particle.TintCP = 1;
        particle.Tint = Color.FromArgb(255, 87, 216, 128); // Pink color UwU :3333
        particle.StartActive = true;
        particle.DispatchSpawn();

        particle.AcceptInput("SetParent", grenade, particle, "!activator");
        particle.AcceptInput("Start");
    }
}