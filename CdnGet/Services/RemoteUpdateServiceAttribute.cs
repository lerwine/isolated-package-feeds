using System.Collections.ObjectModel;

namespace CdnGet.Services;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
sealed class RemoteUpdateServiceAttribute : Attribute
{
    private readonly Guid? _id;
    private readonly string _name;
    public static ReadOnlyDictionary<Guid, (Type Type, string Name, string Description)> RemoteUpdateServices { get; }

    public RemoteUpdateServiceAttribute(string guid, string name)
    {
        if ((guid = guid.ToTrimmedOrNullIfEmpty()!) is not null && Guid.TryParse(guid, out Guid id))
            _id = id;
        _name = name.ToWsNormalizedOrEmptyIfNull();
    }
    public Guid? Id { get { return _id; } }

    public string Name { get { return _name; } }

    public string? Description { get; set; }

    static RemoteUpdateServiceAttribute()
    {
        Type it = typeof(RemoteUpdateService);
        RemoteUpdateServices = new(it.Assembly.GetTypes().SelectMany<Type, (Type Type, RemoteUpdateServiceAttribute Attribute)>(t =>
        {
            if (!(t.IsPublic && t.IsClass && it.IsAssignableFrom(t)) || t.IsAbstract || t == it)
                return Enumerable.Empty<(Type, RemoteUpdateServiceAttribute)>();
            return t.GetCustomAttributes(typeof(RemoteUpdateServiceAttribute), false).OfType<RemoteUpdateServiceAttribute>().Where(a => a.Id.HasValue).Select(a => (t, a));
        }).GroupBy(t => t.Attribute.Id!.Value).Select<IGrouping<Guid, (Type Type, RemoteUpdateServiceAttribute Attribute)>, (Guid Id, Type Type, string Name, string Description)>(g =>
        {
            using IEnumerator<(Type Type, RemoteUpdateServiceAttribute Attribute)> enumerator = g.GetEnumerator();
            enumerator.MoveNext();
            (Type Type, RemoteUpdateServiceAttribute Attribute) result = enumerator.Current;
            if (result.Attribute.Name.Length == 0)
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Attribute.Name.Length > 0)
                    {
                        result = enumerator.Current;
                        break;
                    }
                }
            return (g.Key, result.Type, result.Attribute.Name, result.Attribute.Description.ToTrimmedOrEmptyIfNull());
        }).ToDictionary(t => t.Id, t => (t.Type, t.Name, t.Description)));
    }
}
