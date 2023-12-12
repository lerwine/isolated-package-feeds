using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NuGetPuller.UnitTests.Helpers;

public class ResultWithType<T>
{
    public ResultWithType(T? value)
    {
        var reflectedType = typeof(T);
        ReflectedType = reflectedType.FullName ?? reflectedType.Name;
        Type? actualType = value?.GetType();
        if (actualType is not null && !(actualType.Equals(reflectedType) || actualType.IsPublic))
            actualType = actualType.BaseType;
        ActualType = (actualType is null) ? ReflectedType : actualType.FullName ?? actualType.Name;
        Result = value;
    }

    public string ReflectedType { get; set; }
    public string ActualType { get; set; }
    public T? Result { get; set; }
}

public class ResultWithType<TEnumerable, TItem>
    where TEnumerable : IEnumerable<TItem>
{
    public ResultWithType(TEnumerable? value)
    {
        var reflectedType = typeof(TEnumerable);
        ReflectedType = reflectedType.FullName ?? reflectedType.Name;
        Type? actualType = value?.GetType();
        if (actualType is not null && !(actualType.Equals(reflectedType) || actualType.IsPublic))
            actualType = actualType.BaseType;
        ActualType = actualType?.FullName ?? ReflectedType;
        Results = value?.Select(obj => new Item(obj));
        ReflectedItemType = (reflectedType = typeof(TItem)).FullName ?? reflectedType.Name;
    }

    public string ReflectedType { get; set; }
    public string ActualType { get; set; }
    public string ReflectedItemType { get; set; }
    public IEnumerable<Item>? Results { get; set; }

    public class Item
    {
        public string ActualType { get; set; }
        public TItem? Result { get; set; }
        public Item(TItem? value)
        {
            var reflectedType = typeof(TItem);
            Type? actualType = value?.GetType();
            if (actualType is not null && !(actualType.Equals(reflectedType) || actualType.IsPublic))
                actualType = actualType.BaseType;
            ActualType = (actualType is null) ? (reflectedType.FullName ?? reflectedType.Name) : actualType.FullName ?? actualType.Name;
            Result = value;
        }
    }
}
