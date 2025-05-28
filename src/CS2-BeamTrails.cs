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
    public record struct BeamTrack(Vector LastPos, CBeam Beam);

    public Dictionary<CBaseCSGrenadeProjectile, BeamTrack> Grenades { get; set; } = [];

    public override void Load(bool hotReload)
    {
        Console.WriteLine("Hello World!");

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

                CBeam beam = Utilities.CreateEntityByName<CEnvBeam>("beam")!;
                if (beam == null) return;




                CreateBeamBetweenPoints(grenade.AbsOrigin!, grenade.AbsOrigin!);
                Grenades[grenade] = new BeamTrack(grenade.AbsOrigin!, beam);
                /*
                beam.Render = Color.FromName("Green");
                beam.Width = 5;



                beam.Flags = 1 << 8;
                beam.BeamFlags = 1 << 8;

                beam.LifeState = 1;

                beam.Teleport(player.AbsOrigin);
                //beam.EndPos.Add(grenade.AbsOrigin!);

                Grenades[grenade] = new BeamTrack(grenade.AbsOrigin!, beam);

                beam.DispatchSpawn();
                */
            });
        });

        RegisterListener<Listeners.OnTick>(() =>
        {
            foreach (var nades in Grenades)
            {
                CBaseCSGrenadeProjectile grenade = nades.Key;
                var track = nades.Value;
                CBeam beam = nades.Value.Beam;
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

                

                CreateBeamBetweenPoints(track.LastPos, currentPos!);
                /*
                beam.Teleport(track.LastPos);
                beam.EndPos.Add(currentPos!);
                */
                Grenades[grenade] = new BeamTrack(currentPos!, track.Beam);
            }
        });

    }

    public void CreateBeamBetweenPoints(Vector start, Vector end)
    {
        CBeam? beam = Utilities.CreateEntityByName<CBeam>("beam");

        if (beam == null)
        {
            return;
        }

        beam.Render = Color.Lime;
        beam.Width = 5;



        //beam.EndWidth = 3.0f;

        beam.Teleport(start);

        beam.EndPos.X = end.X;
        beam.EndPos.Y = end.Y;
        beam.EndPos.Z = end.Z;

        //beam.DispatchSpawn();
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


