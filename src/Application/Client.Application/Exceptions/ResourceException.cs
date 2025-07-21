using System;

namespace GodotFloorLevels.Scripts.Application.Exceptions;

/// <summary>
/// Exceção específica para erros no ResourceLoader.
/// </summary>
public class ResourceLoaderException : Exception
{
    public ResourceLoaderException(string message) : base(message) { }
    public ResourceLoaderException(string message, Exception inner) : base(message, inner) { }
}