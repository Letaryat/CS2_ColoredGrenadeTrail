using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace TestPlugin;

public class TestPlugin : BasePlugin
{
    public override string ModuleName => "Hello World Plugin";

    public override string ModuleVersion => "0.0.1";
    public record struct BeamTrack(Vector LastPos, CDynamicProp Beam);

    //public string Model = "particles/ui/hud/ui_map_def_utility_trail.vpcf";
    public string Model = "particles/explosions_fx/bumpmine_detonate_sparks.vpcf";
    public Dictionary<CBaseCSGrenadeProjectile, BeamTrack> Grenades { get; set; } = [];

    public override void Load(bool hotReload)
    {
        Console.WriteLine("Hello World!");

        RegisterListener<Listeners.OnServerPrecacheResources>((ResourceManifest manifest) =>
            {
                manifest.AddResource(Model);
            });

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


    public void CreateBeamBetweenPoints(Vector start, Vector end, CBaseCSGrenadeProjectile grenade)
    {
        CParticleSystem particle = Utilities.CreateEntityByName<CParticleSystem>("info_particle_system")!;

        particle.EffectName = Model;


        //particle.Tint = Color.AliceBlue;


        //Utilities.SetStateChanged(particle, "CParticleSystem", "m_clrTint");
        particle.DispatchSpawn();

        particle.Teleport(start);
        

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


