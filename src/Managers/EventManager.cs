using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CS2_GrenadeTrail.Utils;

namespace CS2_GrenadeTrail.Managers
{
    public class EventManager(CS2_GrenadeTrail plugin)
    {
        private readonly CS2_GrenadeTrail _plugin = plugin;
        public void RegisterEvents()
        {
            //Listeners:
            _plugin.RegisterListener<Listeners.OnServerPrecacheResources>(OnServerPrecache);
            _plugin.RegisterListener<Listeners.OnEntityCreated>(OnEntityCreated);
            //Events:
            _plugin.RegisterEventHandler<EventBulletImpact>(OnBulletImpact);
            _plugin.RegisterEventHandler<EventPlayerSpawn>(OnPlayerSpawn);
        }

        private void OnServerPrecache(ResourceManifest manifest)
        {
            foreach (var particles in _plugin.modelParticles)
            {
                manifest.AddResource(particles);
            }
            foreach (var particles in _plugin.tracerParticles)
            {
                manifest.AddResource(particles);
            }
            manifest.AddResource("models/chicken/chicken.vmdl");
        }

        private void OnEntityCreated(CEntityInstance entity)
        {
            // As far as I remember this listener was inspired (probably copied 1:1 lol) from: 
            // github.com/schwarper/cs2-store/blob/main/Store/src/item/items/grenadetrail.cs

            if (!_plugin.projectiles.Contains(entity.DesignerName)) return;
            CBaseGrenade grenade = new(entity.Handle);
            if (grenade.Handle == IntPtr.Zero) return;
            Server.NextFrame(() =>
            {
                var player = grenade.Thrower.Value?.Controller.Value;
                if (player == null) return;

                _plugin.PluginUtilties!.CreateNadeTrail(grenade.AbsOrigin!, grenade);
            });
        }

        private HookResult OnBulletImpact(EventBulletImpact @event, GameEventInfo info)
        {
            var bullet = @event;
            var player = @event.Userid;
            if (player == null) return HookResult.Continue;
            var pawn = player.PlayerPawn.Value;
            if (pawn == null || !pawn.IsValid) return HookResult.Continue;

            Vector startPos = PluginUtilities.GetEyePosition(player);
            Vector endPos = new Vector(bullet.X, bullet.Y, bullet.Z);

            _plugin.PluginUtilties!.CreateParticleBullet(endPos, startPos);

            return HookResult.Continue;
        }

        private HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
        {   
            var player = @event.Userid;
            if (player == null) return HookResult.Continue;
            var pawn = player.PlayerPawn.Value;
            if (pawn == null || !pawn.IsValid) return HookResult.Continue;
            var startPos = pawn.AbsOrigin;
            _plugin.PluginUtilties!.CreatePathTrail(startPos!, player);
            return HookResult.Continue;
        }

    }
}
