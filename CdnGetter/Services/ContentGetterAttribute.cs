using System.Collections.ObjectModel;

namespace CdnGetter.Services;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
/// <summary>
/// Marks a class as a service for retrieving content from a remote CDN.
/// </summary>
sealed class ContentGetterAttribute : Attribute
{
    private readonly Guid? _id;
    private readonly string _name;

    /// <summary>
    /// Gets the <see cref="ContentGetterService" /> types that are marked with the <c>ContentGetterAttribute</c> attribute, which should be added to the <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
    /// </summary>
    /// <remarks>The key is the unique identifier of the service for the corresponding <see cref="Model.UpstreamCdn" /> database entity.</remarks>
    public static ReadOnlyDictionary<Guid, (Type Type, string Name, string Description)> UpstreamCdnServices { get; }

    /// <summary>
    /// Creates an attribute that identifies a class as a service for retrieving content from a remote CDN.
    /// </summary>
    /// <param name="guid">The <see cref="Guid" /> string which is the unique identifier for the corresponding <see cref="Model.UpstreamCdn" /> database entity.</param>
    /// <param name="name">The display name that identifies the remote CDN.</param>
    public ContentGetterAttribute(string guid, string name)
    {
        if ((guid = guid.ToTrimmedOrNullIfEmpty()!) is not null && Guid.TryParse(guid, out Guid id))
            _id = id;
        _name = name.ToWsNormalizedOrEmptyIfNull();
    }

    /// <summary>
    /// Gets the unique identifier for the  corresponding <see cref="Model.UpstreamCdn" /> database entity.
    /// </summary>
    public Guid? Id { get { return _id; } }

    /// <summary>
    /// Gets the display name that identifies the remote CDN.
    /// </summary>
    public string Name { get { return _name; } }

    /// <summary>
    /// Gets or sets the verbose description of the remote CDN.
    /// </summary>
    public string? Description { get; set; }

    static ContentGetterAttribute()
    {
        Type it = typeof(ContentGetterService);
        UpstreamCdnServices = new(it.Assembly.GetTypes().SelectMany<Type, (Type Type, ContentGetterAttribute Attribute)>(t =>
        {
            if (!(t.IsPublic && t.IsClass && it.IsAssignableFrom(t)) || t.IsAbstract || t == it)
                return Enumerable.Empty<(Type, ContentGetterAttribute)>();
            return t.GetCustomAttributes(typeof(ContentGetterAttribute), false).OfType<ContentGetterAttribute>().Where(a => a.Id.HasValue).Select(a => (t, a));
        }).GroupBy(t => t.Attribute.Id!.Value).Select<IGrouping<Guid, (Type Type, ContentGetterAttribute Attribute)>, (Guid Id, Type Type, string Name, string Description)>(g =>
        {
            using IEnumerator<(Type Type, ContentGetterAttribute Attribute)> enumerator = g.GetEnumerator();
            enumerator.MoveNext();
            (Type Type, ContentGetterAttribute Attribute) result = enumerator.Current;
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
