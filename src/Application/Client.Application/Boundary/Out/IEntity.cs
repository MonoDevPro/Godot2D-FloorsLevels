namespace Client.Application.Boundary.Out;

/// <summary>
/// Interface de entidade ECS genérica, capacidade de armazenar componentes.
/// Definida na camada Application como um *outbound port* para manter independência de infra.
/// </summary>
public interface IEntity
{
    Guid Id { get; }
    bool Has<T>() where T : struct;
    T Get<T>() where T : struct;
    void Set<T>(T component) where T : struct;
    void Remove<T>() where T : struct;
}
