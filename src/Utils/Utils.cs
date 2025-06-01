using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

namespace CS2_CustomTrailAndTracers.Utils
{
    public class PluginUtilities(CS2_CustomTrailAndTracers plugin)
    {
        private readonly CS2_CustomTrailAndTracers _plugin = plugin;

        //Thanks K4ryuu https://discord.com/channels/1160907911501991946/1285282792619507815/1342485330934108221
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
            Control Point.Y = 4 - Radius of a particle (If particle supports it. If not, changing it won't do anything (at least for my particles))
            Control Point = 5 - Start
            Control Point = 6 - End
            */

            float LifeTime = 0.5f;
            var particle = Utilities.CreateEntityByName<CParticleSystem>("info_particle_system")!;

            particle.EffectName = _plugin.tracerParticles[_plugin.tracerToUse];
            particle.Teleport(start);
            particle.LifeState = 5;
            
            SetControlPointValue.Invoke(particle, 4, new Vector(0, 5, 0));
            SetControlPointValue.Invoke(particle, 5, start);
            SetControlPointValue.Invoke(particle, 6, endPos);

            particle.TintCP = 1;
            particle.Tint = _plugin.TrailColor;
            particle.StartActive = true;

            particle.DispatchSpawn();

            particle.AcceptInput("Start");

            //Removal since I had problems with lifespan and it pretty much works similar to this:
            _plugin.AddTimer(LifeTime, () =>
            {
                particle.Remove();
            });
        }

        // Also inspired by: 
        // https://github.com/ipsvn/ChaseMod/blob/c2c003598aeb7d950a61872942fff89fcc21f553/NadeManager.cs#L149
        public void CreateNadeTrail(Vector start, CBaseGrenade grenade)
        {
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


        //Exkludera Bullet-Effects: https://github.com/exkludera/cs2-bullet-effects/blob/main/src/utils.cs#L41
        public static Vector GetEyePosition(CCSPlayerController player)
        {
            Vector absorigin = player.PlayerPawn.Value!.AbsOrigin!;
            CPlayer_CameraServices camera = player.PlayerPawn.Value!.CameraServices!;

            //added "-5" cause you are not shooting fukin lasers from your eyes but idk maybe you do

            return new Vector(absorigin.X, absorigin.Y, absorigin.Z + camera.OldPlayerViewOffsetZ - 5);
        }

    }
}
