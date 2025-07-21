using Godot;

namespace GodotFloorLevels.Scripts.InterfaceAdapters.Godot.Config;

// TODO: Implement this class to manage game settings.

/// <summary>
/// Configurações globais do jogo que podem ser modificadas
/// </summary>
public static class GameConfig
{
    // Configurações de gameplay
    public static float MasterVolume { get; set; } = 1.0f;
    public static float SfxVolume { get; set; } = 0.8f;
    public static float MusicVolume { get; set; } = 0.6f;
        
    // Configurações de debug
    public static bool DebugMode { get; set; } = false;
    public static bool ShowGridLines { get; set; } = false;
    public static bool ShowEntityInfo { get; set; } = false;
        
    // Configurações de display
    public static Vector2I WindowSize { get; set; } = new Vector2I(1280, 720);
    public static bool Fullscreen { get; set; } = false;
    public static bool VSync { get; set; } = true;
        
    /// <summary>
    /// Salva as configurações no arquivo
    /// </summary>
    public static void SaveSettings()
    {
        var config = new ConfigFile();
            
        // Áudio
        config.SetValue("audio", "master_volume", MasterVolume);
        config.SetValue("audio", "sfx_volume", SfxVolume);
        config.SetValue("audio", "music_volume", MusicVolume);
            
        // Debug
        config.SetValue("debug", "debug_mode", DebugMode);
        config.SetValue("debug", "show_grid_lines", ShowGridLines);
        config.SetValue("debug", "show_entity_info", ShowEntityInfo);
            
        // Display
        config.SetValue("display", "window_size", WindowSize);
        config.SetValue("display", "fullscreen", Fullscreen);
        config.SetValue("display", "vsync", VSync);
            
        config.Save("user://game_settings.cfg");
    }
        
    /// <summary>
    /// Carrega as configurações do arquivo
    /// </summary>
    public static void LoadSettings()
    {
        var config = new ConfigFile();
        if (config.Load("user://game_settings.cfg") != Error.Ok)
            return; // Usar valores padrão se não conseguir carregar
            
        // Áudio
        MasterVolume = (float)config.GetValue("audio", "master_volume", MasterVolume);
        SfxVolume = (float)config.GetValue("audio", "sfx_volume", SfxVolume);
        MusicVolume = (float)config.GetValue("audio", "music_volume", MusicVolume);
            
        // Debug
        DebugMode = (bool)config.GetValue("debug", "debug_mode", DebugMode);
        ShowGridLines = (bool)config.GetValue("debug", "show_grid_lines", ShowGridLines);
        ShowEntityInfo = (bool)config.GetValue("debug", "show_entity_info", ShowEntityInfo);
            
        // Display
        WindowSize = (Vector2I)config.GetValue("display", "window_size", WindowSize);
        Fullscreen = (bool)config.GetValue("display", "fullscreen", Fullscreen);
        VSync = (bool)config.GetValue("display", "vsync", VSync);
    }
}