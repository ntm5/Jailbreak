﻿using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using Jailbreak.Public.Extensions;
using Jailbreak.Public.Mod.SpecialDays;
using Timer = CounterStrikeSharp.API.Modules.Timers.Timer;

namespace Jailbreak.SpecialDay.SpecialDays;

public class FreeForAllDay : ISpecialDay
{
    public string Name => "Warday";
    public string Description => "Everyone for themselves. Your goal is to be the last man standing!";

    private int timer = 0;
    private Timer? timer1;
    private BasePlugin _plugin;
    private bool _hasStarted = false;

    public FreeForAllDay(BasePlugin plugin)
    {
        _plugin = plugin;
        VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Hook(_ => _hasStarted ? HookResult.Continue : HookResult.Stop, HookMode.Pre);    }
    
    public void OnStart()
    {
        var spawn = Utilities.FindAllEntitiesByDesignerName<SpawnPoint>("info_player_counterterrorist").ToList();

        foreach (var player in Utilities.GetPlayers()
                     .Where(player => player.IsReal()))
        {
            var max = spawn.Count;
            
            var index = new Random().Next(0, max);
            
            player.Teleport(spawn[index].AbsOrigin);
            player.Freeze();
        }
        
        var friendlyFire = ConVar.Find("mp_friendlyfire");
        if (friendlyFire == null) return;
        
        var friendlyFireValue = friendlyFire.GetPrimitiveValue<bool>(); //assume false in this example, use GetNativeValue for vectors, Qangles, etc
        
        if (!friendlyFireValue) {
            friendlyFire.SetValue<>(true);
        }        
        AddTimers();
    }

    private void AddTimers()
    {
        timer1 = _plugin.AddTimer(1f, () =>
        {
            timer++;
            foreach (var player in Utilities.GetPlayers()
                         .Where(player => player.IsReal()))
            {
                if (timer != 5) return;
                
                player.UnFreeze();
                _hasStarted = true;
            }
            timer1.Kill();
        }, TimerFlags.REPEAT);
        
    }

    public void OnEnd()
    {
        
        var friendlyFire = ConVar.Find("mp_friendlyfire");
        if (friendlyFire == null) return;
        
        var friendlyFireValue = friendlyFire.GetPrimitiveValue<bool>(); //assume false in this example, use GetNativeValue for vectors, Qangles, etc
        
        if (friendlyFireValue) {
            friendlyFire.SetValue<>(false);
        }        
    }

}