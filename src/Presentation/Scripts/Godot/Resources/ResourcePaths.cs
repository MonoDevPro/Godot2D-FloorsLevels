namespace GodotFloorLevels.Scripts.Godot.Resources;

/// <summary>
/// Constantes para caminhos de recursos do jogo
/// </summary>
public static class ResourcePaths
{
    // Diretórios base
    private const string ResourcesDir = "res://Resources/";
    private const string ScriptsDir = "res://Scripts/";
    
    // Sprites de personagens
    public static class CharactersSprites
    {
        // Common
        private const string SpritesDir = ResourcesDir + "Sprites/";
        
        public const string ArcherFemale = SpritesDir + "Archer/Female/spriteframes.tres";
        public const string ArcherMale = SpritesDir + "Archer/Male/spriteframes.tres";
        public const string MageFemale = SpritesDir + "Mage/Female/spriteframes.tres";
        public const string MageMale = SpritesDir + "Mage/Male/spriteframes.tres";
    }
        
    // Cenas principais
    public static class Scenes
    {
        // Common
        private const string ScenesDir = "res://Scenes/";
        
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
