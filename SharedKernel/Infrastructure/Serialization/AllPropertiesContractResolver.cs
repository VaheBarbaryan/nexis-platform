using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SharedKernel.Infrastructure.Serialization;

public class AllPropertiesContractResolver : DefaultContractResolver
{
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        ArgumentNullException.ThrowIfNull(type);

        #pragma warning disable S3011
        var properties = type.GetProperties(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance)
            .Select(p => this.CreateProperty(p, memberSerialization))
            .ToList();

        properties.ForEach(p =>
        {
            p.Writable = true;
            p.Readable = true;
        });

        return properties;
    }
}
