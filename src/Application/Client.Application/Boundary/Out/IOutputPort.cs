namespace Client.Application.Boundary.Out;

public interface IOutputPort<TResponse>
{
    void Present(TResponse response);
}
