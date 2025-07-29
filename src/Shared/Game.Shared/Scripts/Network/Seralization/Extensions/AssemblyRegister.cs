#nullable enable
using System.Linq.Expressions;
using System.Reflection;
using Game.Shared.Scripts.Network.Seralization.Attributes;
using LiteNetLib.Utils;

namespace Game.Shared.Scripts.Network.Seralization.Extensions;

/// <summary>
/// Extensões para registro automático de tipos no NetPacketProcessor
/// </summary>
public static class NetPacketProcessorExtensions
{
    public static void AutoRegisterFromAssembly(this NetPacketProcessor proc)
        => proc.AutoRegisterFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
    
    /// <summary>
    /// Registra automaticamente todos os tipos marcados com AutoRegisterPacketAttribute na assembly atual
    /// </summary>
    /// <param name="processor">O NetPacketProcessor</param>
    /// <param name="assembly">Assembly para escanear (opcional, usa Assembly.GetCallingAssembly() se não especificado)</param>
    public static void AutoRegisterFromAssembly(this NetPacketProcessor processor, System.Reflection.Assembly? assembly)
    {
        assembly ??= System.Reflection.Assembly.GetCallingAssembly();
        AutoRegisterFromAssemblies(processor, assembly);
    }

    /// <summary>
    /// Registra automaticamente todos os tipos marcados com AutoRegisterPacketAttribute nas assemblies especificadas
    /// </summary>
    /// <param name="processor">O NetPacketProcessor</param>
    /// <param name="assemblies">Assemblies para escanear</param>
    public static void AutoRegisterFromAssemblies(this NetPacketProcessor processor, params System.Reflection.Assembly[] assemblies)
    {
        //var typesToRegister = new List<(Type type, AutoRegisterTypeAttribute attribute)>();
        
        var typesToRegister = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t =>
                !t.IsGenericTypeDefinition &&
                t.GetCustomAttribute<AutoRegisterTypeAttribute>() is not null
            )
            .Select(t => (type: t, attr: t.GetCustomAttribute<AutoRegisterTypeAttribute>()));

        // Registra cada tipo
        foreach (var (type, attribute) in typesToRegister)
        {
            RegisterType(processor, type, attribute);
        }
    }

    /// <summary>
    /// Registra um tipo específico
    /// </summary>
    /// <param name="proc">O NetPacketProcessor</param>
    /// <param name="type">Tipo para registrar</param>
    /// <param name="attr">Atributo de configuração</param>
    private static void RegisterType(NetPacketProcessor proc, Type type, AutoRegisterTypeAttribute attr)
    {
        if (attr.UseCustomDelegates || HasCustomDelegates(type))
            RegisterWithCustomDelegates(proc, type);
        else if (typeof(INetSerializable).IsAssignableFrom(type))
            if (type.IsValueType) RegisterStructType(proc, type, attr);
            else               RegisterClassType(proc, type, attr);
        else
            throw new InvalidOperationException($"Tipo {type.Name} inválido p/ registro");
    }

    /// <summary>
    /// Registra um tipo struct
    /// </summary>
    /// <param name="proc">O NetPacketProcessor</param>
    /// <param name="type">Tipo para registrar</param>
    /// <param name="attr">Atributo de configuração</param>
    private static void RegisterStructType(NetPacketProcessor proc, Type type, AutoRegisterTypeAttribute attr)
    {
        if (!attr.UseCustomDelegates && typeof(INetSerializable).IsAssignableFrom(type))
        {
            // Chama o RegisterNestedType<T>() genérico via reflexão
            proc.RegisterNestedType(type);
        }
        else if (attr.UseCustomDelegates || HasCustomDelegates(type))
        {
            RegisterWithCustomDelegates(proc, type);
        }
        else
        {
            throw new InvalidOperationException(
                $"Struct {type.Name} necessita implementar INetSerializable ou ter delegates customizados.");
        }
    }

    /// <summary>
    /// Registra um tipo class
    /// </summary>
    /// <param name="proc">O NetPacketProcessor</param>
    /// <param name="type">Tipo para registrar</param>
    /// <param name="attr">Atributo de configuração</param>
    private static void RegisterClassType(NetPacketProcessor proc, Type type, AutoRegisterTypeAttribute attr)
    {
        if ((attr.UseCustomDelegates || HasCustomDelegates(type)))
        {
            RegisterWithCustomDelegates(proc, type);
            return;
        }

        if (typeof(INetSerializable).IsAssignableFrom(type))
        {
            var ctor = FindConstructorDelegate(type)
                       ?? CreateDefaultConstructor(type);
            proc.RegisterNestedType(type, ctor);
        }
        else
        {
            throw new InvalidOperationException($"Classe {type.Name} necessita INetSerializable ou delegates custom.");
        }
    }

    /// <summary>
    /// Verifica se o tipo tem delegates customizados definidos
    /// </summary>
    private static bool HasCustomDelegates(Type type)
    {
        bool hasWriter = type.GetMethods()
            .Any(m => m.GetCustomAttribute<WriteTypeDelegateAttribute>() != null);
        bool hasReader = type.GetMethods()
            .Any(m => m.GetCustomAttribute<ReadTypeDelegateAttribute>() != null);
        return hasWriter || hasReader;
    }
    
    private static void RegisterWithCustomDelegates(NetPacketProcessor proc, Type type)
    {
        var writeDel = FindWriteDelegate(type);
        var readDel  = FindReadDelegate(type);

        if (writeDel is null || readDel is null)
            throw new InvalidOperationException($"Tipo {type.Name} sem delegates válidos.");

        // Chama o overload correto
        proc.RegisterNestedType(type, writeDel, readDel);
    }

    /// <summary>
    /// Encontra o delegate de escrita para um tipo
    /// </summary>
    private static Delegate FindWriteDelegate(Type type)
    {
        var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.GetCustomAttribute<WriteTypeDelegateAttribute>() != null);

        foreach (var method in methods)
        {
            var parameters = method.GetParameters();
            if (parameters.Length == 2 && 
                parameters[0].ParameterType == typeof(NetDataWriter) && 
                parameters[1].ParameterType == type &&
                method.ReturnType == typeof(void))
            {
                return Delegate.CreateDelegate(typeof(Action<,>).MakeGenericType(typeof(NetDataWriter), type), method);
            }
        }

        return null;
    }

    /// <summary>
    /// Encontra o delegate de leitura para um tipo
    /// </summary>
    private static Delegate FindReadDelegate(Type type)
    {
        var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.GetCustomAttribute<ReadTypeDelegateAttribute>() != null);

        foreach (var method in methods)
        {
            var parameters = method.GetParameters();
            if (parameters.Length == 1 && 
                parameters[0].ParameterType == typeof(NetDataReader) && 
                method.ReturnType == type)
            {
                return Delegate.CreateDelegate(typeof(Func<,>).MakeGenericType(typeof(NetDataReader), type), method);
            }
        }

        return null;
    }

    /// <summary>
    /// Encontra o delegate constructor para um tipo
    /// </summary>
    private static Delegate FindConstructorDelegate(Type type)
    {
        // Procura por métodos marcados como factory
        var factoryMethods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.GetCustomAttribute<TypeConstructorAttribute>() != null &&
                        m.GetParameters().Length == 0 &&
                        m.ReturnType == type);

        if (factoryMethods.Any())
        {
            var factoryMethod = factoryMethods.First();
            return Delegate.CreateDelegate(typeof(Func<>).MakeGenericType(type), factoryMethod);
        }

        return null;
    }

    /// <summary>
    /// Cria um delegate constructor padrão para um tipo
    /// </summary>
    private static Delegate CreateDefaultConstructor(Type type)
    {
        // Verifica se há construtor público sem parâmetros
        var ctorInfo = type.GetConstructor(Type.EmptyTypes);
        if (ctorInfo == null)
            throw new InvalidOperationException($"Tipo {type.Name} não possui construtor padrão público");

        // Expression.New para chamar o construtor
        NewExpression newExpr = Expression.New(ctorInfo);

        // Tipo do delegate Func<T>
        Type funcType = typeof(Func<>).MakeGenericType(type);

        // Cria um Lambda<Func<T>> e já compila para um Delegate
        LambdaExpression lambda = Expression.Lambda(funcType, newExpr);
        return lambda.Compile();
    }
    
    /// <summary>
    /// Chama o método genérico RegisterNestedType&lt;T&gt;() onde T : struct, INetSerializable
    /// </summary>
    private static void RegisterNestedType(this NetPacketProcessor proc, Type structType)
    {
        if (!structType.IsValueType)
            throw new ArgumentException($"Tipo {structType.Name} deve ser um struct", nameof(structType));

        if (!typeof(INetSerializable).IsAssignableFrom(structType))
            throw new ArgumentException($"Tipo {structType.Name} deve implementar INetSerializable", nameof(structType));
        
        var m = typeof(NetPacketProcessor)
            .GetMethod(nameof(NetPacketProcessor.RegisterNestedType), Type.EmptyTypes)!;
        // torna genérico
        var gen = m.MakeGenericMethod(structType);
        gen.Invoke(proc, null);
    }

    /// <summary>
    /// Chama o método genérico RegisterNestedType&lt;T&gt;(Func&lt;T&gt; constructor) onde T : class, INetSerializable
    /// </summary>
    private static void RegisterNestedType(this NetPacketProcessor proc, Type classType, Delegate constructor)
    {
        if (classType.IsValueType)
            throw new ArgumentException($"Tipo {classType.Name} deve ser uma classe", nameof(classType));

        if (!typeof(INetSerializable).IsAssignableFrom(classType))
            throw new ArgumentException($"Tipo {classType.Name} deve implementar INetSerializable", nameof(classType));
        
        var m = typeof(NetPacketProcessor)
            .GetMethods()
            .Where(m => m.Name == nameof(NetPacketProcessor.RegisterNestedType))
            .Where(m => m.GetParameters().Length == 1)
            .Where(m => m.GetParameters()[0].ParameterType.IsGenericType)
            .FirstOrDefault(m => m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(Func<>));
        
        if (m == null)
            throw new InvalidOperationException("Método RegisterNestedType<T>(Func<T>) não encontrado");
        var gen = m.MakeGenericMethod(classType);
        // Func<object> não encaixa diretamente em Func<T>, mas Func<T> aceita retorno covariante
        gen.Invoke(proc, new object[] { constructor });
    }

    /// <summary>
    /// Chama o método genérico RegisterNestedType&lt;T&gt;(Action&lt;NetDataWriter, T&gt;, Func&lt;NetDataReader, T&gt;)
    /// </summary>
    private static void RegisterNestedType(this NetPacketProcessor proc, Type type,
        Delegate writeDelegate, Delegate readDelegate)
    {
        var m = typeof(NetPacketProcessor)
            .GetMethods()
            .FirstOrDefault(mi =>
                mi.Name == nameof(NetPacketProcessor.RegisterNestedType) &&
                mi.GetParameters().Length == 2 &&
                mi.GetParameters()[0].ParameterType.IsGenericType &&
                mi.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(Action<,>) &&
                mi.GetParameters()[1].ParameterType.IsGenericType &&
                mi.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>)
            );
        
        if (m == null)
            throw new InvalidOperationException("Método RegisterNestedType<T>(Action<NetDataWriter, T>, Func<NetDataReader, T>) não encontrado");
        
        var gen = m.MakeGenericMethod(type);
        gen.Invoke(proc, new object[] { writeDelegate, readDelegate });
    }
}
