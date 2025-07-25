using Client.Domain.Enums;
using Godot;
using GodotFloorLevels.Scripts.Godot.Resources.Loader;

namespace GodotFloorLevels.Scripts.Godot.Resources.Utils;

// Um VO que representa “qual sprite eu quero” e já carrega o SpriteFrames correspondente.
[GlobalClass]
public partial class GodotSpriteResource : Resource
{
    [Export] public GodotResourceLoader Loader;
    [Export] public VocationEnum Vocation = VocationEnum.None;
    [Export] public GenderEnum Gender = GenderEnum.None;
    
    private AnimatedSprite2D _animatedSprite;
    public AnimatedSprite2D AnimatedSprite { get => _animatedSprite ??= CreateSprite();  }
    
    private AnimatedSprite2D CreateSprite()
    {
        _animatedSprite = new AnimatedSprite2D()
        {
            SpriteFrames = LoadFrames(),
            // você pode já configurar pivô, escala, etc:
        };
        return _animatedSprite;
    }

    private SpriteFrames LoadFrames()
    {
        var path = (Vocation, Gender) switch
        {
            (VocationEnum.Mage,   GenderEnum.Male)   => ResourcePaths.CharactersSprites.MageMale,
            (VocationEnum.Mage,   GenderEnum.Female) => ResourcePaths.CharactersSprites.MageFemale,
            (VocationEnum.Archer, GenderEnum.Male)   => ResourcePaths.CharactersSprites.ArcherMale,
            (VocationEnum.Archer, GenderEnum.Female) => ResourcePaths.CharactersSprites.ArcherFemale,
            _ => throw new KeyNotFoundException(
                $"No SpriteFrames for vocation '{Vocation}' with gender '{Gender}'"
            )
        };
        
        var resource = Loader.LoadResource<SpriteFrames>(path);
        
        if (resource == null)
            throw new KeyNotFoundException($"SpriteFrames not found at path: {path}");
        
        return resource;
    }

    // Implicit conversion pra facilitar o uso direto onde se espera um Node:
    public static implicit operator AnimatedSprite2D(GodotSpriteResource vo) => vo.AnimatedSprite;
}
