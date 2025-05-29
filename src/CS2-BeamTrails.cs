using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using CounterStrikeSharp.API.Modules.Utils;

namespace TestPlugin;

public class TestPlugin : BasePlugin
{
    public override string ModuleName => "Hello World Plugin";

    public override string ModuleVersion => "0.0.1";
    public record struct BeamTrack(Vector LastPos, CBeam Beam);

    public Dictionary<CBaseCSGrenadeProjectile, BeamTrack> Grenades { get; set; } = [];

    public override void Load(bool hotReload)
    {
        Console.WriteLine("Hello World!");

        RegisterListener<Listeners.OnEntityCreated>((entity) =>
        {
            if (entity.DesignerName != "hegrenade_projectile") return;
            CBaseCSGrenadeProjectile grenade = new(entity.Handle);
            CParticleSystem beam = Utilities.CreateEntityByName<CParticleSystem>("info_particle_system")!;
            //CBeam beam = Utilities.CreateEntityByName<CEnvBeam>("beam")!;
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
            foreach (var nades in Grenades.ToList())
            {
                CBaseCSGrenadeProjectile grenade = nades.Key;
                var track = nades.Value;

                if (!grenade.IsValid)
                {
                    if (track.Beam.IsValid)
                    {
                        track.Beam.Remove();
                    }
                    Grenades.Remove(grenade);
                    continue;
                }

                var currentPos = grenade.AbsOrigin!;
                int segmentCount = 10; // więcej = bardziej gładkie
                /*
                for (int i = 0; i < segmentCount; i++)
                {
                    float t1 = i / (float)segmentCount;
                    float t2 = (i + 1) / (float)segmentCount;
                    var start = Lerp(track.LastPos, currentPos, t1);
                    var end = Lerp(track.LastPos, currentPos, t2);
                    CreateBeamBetweenPoints(start, end, grenade);
                }
                */
                CreateBeamBetweenPoints(currentPos, grenade.AbsOrigin!, grenade);


            }
        });


    }

    public void CreateBeamBetweenPoints(Vector start, Vector end, CBaseCSGrenadeProjectile grenade)
    {
        CBeam? beam = Utilities.CreateEntityByName<CBeam>("env_beam")!;
        Grenades[grenade] = new BeamTrack(start, beam);
        if (beam == null)
        {
            return;
        }

        //beam.StartEntity = grenade.AbsOrigin;
        
        beam.Render = Color.Lime;
        beam.Width = 5;

        //beam.EndWidth = 3.0f;

        beam.Teleport(start);

        
        beam.EndPos.X = end.X;
        beam.EndPos.Y = end.Y;
        beam.EndPos.Z = end.Z;
        
        beam.DispatchSpawn();

        AddTimer(20.0f, () =>
        {
            if (beam.IsValid) beam.AcceptInput("Kill");
        });

    }

    public static Vector Lerp(Vector a, Vector b, float t)
{
    return new Vector
    {
        X = a.X + (b.X - a.X) * t,
        Y = a.Y + (b.Y - a.Y) * t,
        Z = a.Z + (b.Z - a.Z) * t
    };
}

}


