namespace Client.Infrastructure.Godot.Resources.Constants;

/// <summary>
/// Constantes para caminhos de recursos do jogo
/// </summary>
public static class ResourcePaths
{
    // Diretórios base
    private const string ResourcesDir = "res://Resources/";
    private const string ScenesDir = "res://Scenes/";
    private const string ScriptsDir = "res://Scripts/";
        
    private const string SpritesDir = ResourcesDir + "Sprites/";
        
    // Sprites de personagens
    public static class Characters
    {
        public const string ArcherFemale = SpritesDir + "Archer/Female/spriteframes.tres";
        public const string ArcherMale = SpritesDir + "Archer/Male/spriteframes.tres";
        public const string MageFemale = SpritesDir + "Mage/Female/spriteframes.tres";
        public const string MageMale = SpritesDir + "Mage/Male/spriteframes.tres";
    }
        
    // Cenas principais
    public static class Scenes
    {
        public const string Main = ScenesDir + "main.tscn";
        public const string Menu = ScenesDir + "menu.tscn";
        public const string Game = ScenesDir + "game.tscn";
    }
        
    // Configurações
    public static class Config
    {
        public const string GameSettings = "user://game_settings.cfg";
        public const string SaveData = "user://save_data.dat";
    }
}