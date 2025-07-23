namespace Client.Application.Boundary.Out;

/// <summary>
/// Abstração para criar, consultar e manipular entidades ECS.
/// Definida na camada Application para manter a independência de infra.
/// </summary>
public interface IEcsContext
{
    /// <summary>Cria nova entidade vazia.</summary>
    IEntity CreateEntity();

    /// <summary>
    /// Consulta entidades que tenham todos os tipos de componente especificados.
    /// </summary>
    IEnumerable<IEntity> QueryEntities(params Type[] componentTypes);

    /// <summary>
    /// Encontra uma única entidade ou retorna null.
    /// </summary>
    IEntity? QueryEntity(Func<IEntity, bool> predicate);
}
