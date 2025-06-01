using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API;
using System.Drawing;


namespace CS2_GrenadeTrail.Managers
{
    public class CommandsManager(CS2_GrenadeTrail plugin)
    {
        private readonly CS2_GrenadeTrail _plugin = plugin;
        public void RegisterCommands()
        {
            _plugin.AddCommand("css_randomTrailColor", "Sets random color of trail", OnRandomColor);
            _plugin.AddCommand("css_particle", "Set trail model to argument (int)", OnChangeParticle);
            _plugin.AddCommand("css_tracers", "Set bullet tracer model to argument (int)", OnChangeTracer);
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

            _plugin.TrailColor = Color.FromArgb(r, g, b, 255);
            Server.PrintToChatAll($"Changed color to: {_plugin.TrailColor}");
        }

        [CommandHelper(minArgs: 1, usage: "id", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
        private void OnChangeParticle(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null) return;
            var pawn = player.PlayerPawn.Value;
            if (pawn == null || !pawn.IsValid) return;
            var id = Convert.ToInt32(commandInfo.GetArg(1));
            _plugin.modelToUse = id;
            player.PrintToChat($"Changed trail to: {_plugin.modelParticles[id]}");
        }

        [CommandHelper(minArgs: 1, usage: "id", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
        private void OnChangeTracer(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null) return;
            var pawn = player.PlayerPawn.Value;
            if (pawn == null || !pawn.IsValid) return;
            var id = Convert.ToInt32(commandInfo.GetArg(1));
            _plugin.tracerToUse = id;
            player.PrintToChat($"Changed tracer to: {_plugin.tracerParticles[id]}");
        }

    }
}
