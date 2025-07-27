using Game.Shared.Resources;
using Godot;

namespace Game.Shared.Config;

public static class ResourcePaths
{
    // Diretórios base
    private const string ScenesDir    = "res://Scenes/";
    private const string ResourcesDir = "res://Resources/";
    private const string ScriptsDir   = "res://Scripts/";
    private const string UserCfgDir   = "user://";
    
    // Shared Resource Paths
    private const string SharedScenesDir = "res://SharedScenes/";
    private const string SharedResourcesDir = "res://SharedResources/";
    private const string SharedScriptsDir = "res://SharedScripts/";
    
    // Diretório de Cenas
    public static class Scenes
    {
        public static readonly ResourcePath<PackedScene> Main =
            new(ScenesDir + "main.tscn");

        public static readonly ResourcePath<PackedScene> Menu =
            new(ScenesDir + "menu.tscn");

        public static readonly ResourcePath<PackedScene> Game =
            new(ScenesDir + "game.tscn");
        
        // Diretório de Entidades
        public static class Entities
        {
            public static readonly ResourcePath<PackedScene> GodotBody =
                new(SharedScenesDir + "Entities/GodotBody.tscn");
            
            public static readonly ResourcePath<PackedScene> PlayerBody =
                new(ScenesDir + "Entities/PlayerBody.tscn");
        }
    }
    
    // Diretório de Recursos
    public static class Resources
    {
        public static readonly ResourcePath<SpriteFrames> SpriteFramesMageMale =
            new(ResourcesDir + "Sprites/Mage/Male/spriteframes.tres");
        public static readonly ResourcePath<SpriteFrames> SpriteFramesMageFemale =
            new(ResourcesDir + "Sprites/Mage/Female/spriteframes.tres");
        public static readonly ResourcePath<SpriteFrames> SpriteFramesArcherMale =
            new(ResourcesDir + "Sprites/Archer/Male/spriteframes.tres");
        public static readonly ResourcePath<SpriteFrames> SpriteFramesArcherFemale =
            new(ResourcesDir + "Sprites/Archer/Female/spriteframes.tres");
    }

    // Diretório de Configurações
    public static class Config
    {
        public static readonly ResourcePath<ConfigFile> GameSettings =
            new(UserCfgDir + "game_settings.cfg");
            
        public static readonly ResourcePath<ConfigFile> SaveData =
            new(UserCfgDir + "save_data.dat");
    }
    
    // Diretório de Scripts
    public static class Scripts
    {
        public static readonly ResourcePath<Script> 
            PlayerController = new(ScriptsDir + "PlayerController.cs");
    }
}
