using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

namespace CS2_GrenadeTrail.Utils
{
    public class PluginUtilities(CS2_GrenadeTrail plugin)
    {
        private readonly CS2_GrenadeTrail _plugin = plugin;
        public static MemoryFunctionWithReturn<CParticleSystem, int, Vector, bool> SetControlPointValue { get; }
        = new("55 48 89 E5 41 57 41 56 41 55 49 89 D5 31 D2 41 54 41 89 F4");

        public void CreatePathTrail(Vector start, CCSPlayerController player)
        {
            // Also inspired:
            // https://github.com/ipsvn/ChaseMod/blob/c2c003598aeb7d950a61872942fff89fcc21f553/NadeManager.cs#L149

            CParticleSystem particle = Utilities.CreateEntityByName<CParticleSystem>("info_particle_system")!;

            particle.EffectName = _plugin.modelParticles[_plugin.modelToUse];
            particle.Teleport(start, QAngle.Zero, Vector.Zero);

            particle.TintCP = 1;
            particle.Tint = _plugin.TrailColor;
            particle.StartActive = true;
            particle.DispatchSpawn();

            particle.AcceptInput("Start");
            particle.AcceptInput("SetParent", player.PlayerPawn.Value, particle, "!activator");

            _plugin.AddTimer(3f, () =>
            {

                CreatePathTrail(start, player);
            });

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

            particle.EffectName = _plugin.tracerParticles[_plugin.tracerToUse];
            particle.Teleport(start);

            SetControlPointValue.Invoke(particle, 5, start);
            SetControlPointValue.Invoke(particle, 6, endPos);

            /*
            particle.DataCP = 2;
            particle.DataCPValue.X = direction.X; particle.DataCPValue.Y = direction.Y; particle.DataCPValue.Z = direction.Z;
            */
            particle.TintCP = 1;
            particle.Tint = _plugin.TrailColor;
            particle.StartActive = true;

            particle.DispatchSpawn();

            particle.AcceptInput("Start");
        }

        public void CreateNadeTrail(Vector start, CBaseGrenade grenade)
        {
            // Also inspired by: 
            // https://github.com/ipsvn/ChaseMod/blob/c2c003598aeb7d950a61872942fff89fcc21f553/NadeManager.cs#L149

            CParticleSystem particle = Utilities.CreateEntityByName<CParticleSystem>("info_particle_system")!;

            particle.EffectName = _plugin.modelParticles[_plugin.modelToUse];
            particle.Teleport(start, QAngle.Zero, Vector.Zero);

            particle.TintCP = 1;
            particle.Tint = _plugin.TrailColor;
            particle.StartActive = true;
            particle.DispatchSpawn();

            particle.AcceptInput("SetParent", grenade, particle, "!activator");
            particle.AcceptInput("Start");
        }


        //From Exkludera bullet impact:
        public static Vector GetEyePosition(CCSPlayerController player)
        {
            Vector absorigin = player.PlayerPawn.Value!.AbsOrigin!;
            CPlayer_CameraServices camera = player.PlayerPawn.Value!.CameraServices!;

            return new Vector(absorigin.X, absorigin.Y, absorigin.Z + camera.OldPlayerViewOffsetZ);
        }

    }
}
