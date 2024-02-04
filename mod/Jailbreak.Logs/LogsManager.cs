﻿using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Jailbreak.Public.Behaviors;
using Jailbreak.Public.Extensions;
using Jailbreak.Public.Mod.Logs;
using Jailbreak.Public.Mod.Rebel;
using Jailbreak.Public.Mod.Warden;
using Microsoft.Extensions.DependencyInjection;

namespace Jailbreak.Logs;

public class LogsManager : IPluginBehavior, ILogService
{
    private readonly List<string> _logMessages = new();
    private long startTime;
    private IWardenService wardenService;
    private IRebelService rebelService;

    private IServiceProvider _serviceProvider;
    public LogsManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Start(BasePlugin parent)
    {
        parent.RegisterEventHandler<EventRoundStart>(OnRoundStart);
        wardenService = _serviceProvider.GetRequiredService<IWardenService>();
        rebelService = _serviceProvider.GetRequiredService<IRebelService>();
    }

    private HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        startTime = DateTimeOffset.Now.ToUnixTimeSeconds();
        ClearLogMessages();
        return HookResult.Continue;
    }

    public void AddLogMessage(string message)
    {
        // format to [MM:SS] message
        string prefix = $"[{TimeSpan.FromSeconds(DateTimeOffset.Now.ToUnixTimeSeconds() - startTime):mm\\:ss}] ";
        _logMessages.Add(prefix + message);
    }

    public ICollection<string> GetLogMessages()
    {
        return _logMessages;
    }

    public void ClearLogMessages()
    {
        _logMessages.Clear();
    }

    public string FormatPlayer(CCSPlayerController player)
    {
        if (wardenService.IsWarden(player))
            return $"{player.PlayerName} (WARDEN)";
        if (player.GetTeam() == CsTeam.CounterTerrorist)
            return $"{player.PlayerName} (CT)";
        if (rebelService.IsRebel(player))
            return $"{player.PlayerName} (REBEL)";
        return $"{player.PlayerName} (Prisoner)";
    }
}