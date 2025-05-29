using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;

namespace TestPlugin;

public class TestPlugin : BasePlugin
{
    public override string ModuleName => "Hello World Plugin";

    public override string ModuleVersion => "0.0.1";
    public record struct BeamTrack(Vector LastPos, CDynamicProp Beam);

    //public string Model = "particles/ui/hud/ui_map_def_utility_trail.vpcf";
    public string Model = "particles/letaryat/testparticle.vpcf";
    //public string Model = "particles/kolka/grenade/line.vpcf";
    public Dictionary<CBaseCSGrenadeProjectile, BeamTrack> Grenades { get; set; } = [];

    public override void Load(bool hotReload)
    {
        Console.WriteLine("Hello World!");

        RegisterListener<Listeners.OnServerPrecacheResources>((ResourceManifest manifest) =>
            {
                manifest.AddResource(Model);
            });

        AddCommand("css_spawnBeam", "test", DoRingParticle);

        RegisterListener<Listeners.OnEntityCreated>((entity) =>
        {
            if (entity.DesignerName != "hegrenade_projectile") return;
            CBaseCSGrenadeProjectile grenade = new(entity.Handle);
            if (grenade.Handle == IntPtr.Zero) return;
            Server.NextFrame(() =>
            {
                Server.PrintToChatAll("Rzucam nade!");
                var player = grenade.Thrower.Value?.Controller.Value;
                if (player == null) return;
                CreateBeamBetweenPoints(grenade.AbsOrigin!, grenade.AbsOrigin!, grenade);


            });
        });

        RegisterListener<Listeners.OnTick>(() =>
        {
            foreach (var nades in Grenades)
            {
                CBaseCSGrenadeProjectile grenade = nades.Key;
                var track = nades.Value;
                CDynamicProp beam = nades.Value.Beam;
                if (!grenade.IsValid)
                {
                    if (beam.IsValid)
                    {
                        beam.Remove();
                    }
                    Grenades.Remove(grenade);
                    continue;
                }

                var currentPos = grenade.AbsOrigin;



                CreateBeamBetweenPoints(track.LastPos, currentPos!, grenade);
            }
        });

    }

    private void DoRingParticle(CCSPlayerController? player, CommandInfo commandInfo)
    {
        Server.PrintToChatAll("SIEMA");
        var particle = Utilities.CreateEntityByName<CParticleSystem>("info_particle_system");

        if (particle!.IsValid)
        particle!.EffectName = Model;
        particle.Teleport(player!.PlayerPawn.Value!.AbsOrigin, QAngle.Zero, Vector.Zero);
        /*
        particle.DataCP = 1;
        particle.DataCPValue.X = 1.0f;
        particle.DataCPValue.Y = 0f;
        particle.DataCPValue.Z = 0f;
        */

        particle.TintCP = 1;
        particle.Tint = Color.FromArgb(8, 255, 255);

        particle.StartActive = true;
        particle.DispatchSpawn();


    }

    public void CreateBeamBetweenPoints(Vector start, Vector end, CBaseCSGrenadeProjectile grenade)
    {
        CParticleSystem particle = Utilities.CreateEntityByName<CParticleSystem>("info_particle_system")!;

        particle.EffectName = Model;
        particle.Teleport(start, QAngle.Zero, Vector.Zero);

        particle.TintCP = 1;
        particle.Tint = Color.FromArgb(255, 87, 216, 128);
        particle.StartActive = true;
        particle.DispatchSpawn();

        particle.AcceptInput("SetParent", grenade, particle, "!activator");
        particle.AcceptInput("Start");

    }



    /*
    public void CreateBeamBetweenPoints(Vector start, Vector end, CBaseCSGrenadeProjectile grenade)
    {
        CDynamicProp? particle = Utilities.CreateEntityByName<CDynamicProp>("prop_dynamic");
        Grenades[grenade] = new BeamTrack(start, particle!);


        particle!.SetModel(Model);
        //particle.EffectName = Model;
        particle.DispatchSpawn();
        particle.Teleport(start, new QAngle(), new Vector());
        //particle.AcceptInput("Start");

        //beam.DispatchSpawn();
        /*
        AddTimer(20.0f, () =>
        {
            if (beam.IsValid) beam.AcceptInput("Kill");
        });
    

    }
        */

}


