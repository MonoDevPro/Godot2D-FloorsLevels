using Client.Domain.Enums;
using Client.Domain.ValueObjects;
using Client.Infrastructure.Godot.Resources.Constants;
using Client.Infrastructure.Godot.Resources.Loader;
using Godot;

namespace Client.Infrastructure.Godot.Resources;

// Um VO que representa “qual sprite eu quero” e já carrega o SpriteFrames correspondente.
[GlobalClass]
public partial class GodotSpriteResource : Resource
{
    [Export] public GodotResourceLoader Loader = new();
    [Export] public Vocation Vocation;
    [Export] public Gender Gender;
    
    private readonly AnimatedSprite2D _animatedSprite2D;
    private readonly Sprite _sprite;
    
    // Construtor vazio necessário para o Godot reconhecer como um Resource
    public GodotSpriteResource() : this(new Sprite(Vocation.Mage, Gender.Male)) { }
    public GodotSpriteResource(Sprite definition)
    {
        _sprite = definition;
        _animatedSprite2D = CreateSprite();
        Vocation = definition.Vocation;
        Gender = definition.Gender;
    }
    
    public Sprite ToDefinition()
        => _sprite;

    public AnimatedSprite2D ToAnimatedSprite2D()
        => _animatedSprite2D;

    private SpriteFrames LoadFrames()
    {
        var path = (Vocation, Gender) switch
        {
            (Vocation.Mage,   Gender.Male)   => ResourcePaths.Characters.MageMale,
            (Vocation.Mage,   Gender.Female) => ResourcePaths.Characters.MageFemale,
            (Vocation.Archer, Gender.Male)   => ResourcePaths.Characters.ArcherMale,
            (Vocation.Archer, Gender.Female) => ResourcePaths.Characters.ArcherFemale,
            _ => throw new KeyNotFoundException(
                $"No SpriteFrames for vocation '{Vocation}' with gender '{Gender}'"
            )
        };
        
        var resource = Loader.LoadResource<SpriteFrames>(path);
        
        if (resource == null)
            throw new KeyNotFoundException($"SpriteFrames not found at path: {path}");
        
        return resource;
    }

    private AnimatedSprite2D CreateSprite()
    {
        var sprite = new AnimatedSprite2D()
        {
            SpriteFrames = LoadFrames(),
            // você pode já configurar pivô, escala, etc:
        };
        return sprite;
    }

    // Implicit conversion pra facilitar o uso direto onde se espera um Node:
    public static implicit operator AnimatedSprite2D(GodotSpriteResource vo) => vo._animatedSprite2D;
}