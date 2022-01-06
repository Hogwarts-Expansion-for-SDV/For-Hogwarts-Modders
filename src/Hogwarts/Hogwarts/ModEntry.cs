using System;
using Hogwarts;
using StardewModdingAPI;
using StardewValley;

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    internal static ModEntry Instance;
    internal const string TrainStationName = "Custom_HogsmeadeStation";
    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        Instance = this;
        helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
    }

    private void GameLoop_UpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
    {
        if (Game1.player.currentLocation.Name == TrainStationName)
        {
            if (e.IsMultipleOf((uint)(60 * TrainSprite.Random.Next(30, 60))))
            {
                new TrainSprite(Monitor, Game1.player.currentLocation);
            }
            foreach (TrainSprite ts in TrainSprite.Trains)
            {
                ts.Update();
            }
        }
    }
}