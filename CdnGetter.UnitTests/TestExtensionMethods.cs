using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace CdnGetter.UnitTests;

public static class TestExtensionMethods
{
    public static JsonObject AddProperty(this JsonObject target, string propertyName, string value)
    {
        target.Add(propertyName, JsonValue.Create(value));
        return target;
    }
    
    public static JsonObject AddProperty(this JsonObject target, string propertyName, int value)
    {
        target.Add(propertyName, JsonValue.Create(value));
        return target;
    }
    
    public static JsonObject AddProperty(this JsonObject target, string propertyName, bool value)
    {
        target.Add(propertyName, JsonValue.Create(value));
        return target;
    }
    
    public static JsonObject AddProperty(this JsonObject target, string propertyName, JsonObject value)
    {
        target.Add(propertyName, value);
        return target;
    }

    public static JsonObject AddNullProperty(this JsonObject target, string propertyName)
    {
        target.Add(propertyName, null);
        return target;
    }

    public static JsonObject AddIntArrayProperty(this JsonObject target, string propertyName, params int[] values)
    {
        JsonArray arr = new();
        foreach (int i in values)
            arr.Add(JsonValue.Create(i));
        target.Add(propertyName, arr);
        return target;
    }

    public static JsonObject AddStringArrayProperty(this JsonObject target, string propertyName, params string[] values)
    {
        JsonArray arr = new();
        foreach (string s in values)
            arr.Add(JsonValue.Create(s));
        target.Add(propertyName, arr);
        return target;
    }

    public static JsonObject AddObjectArrayProperty(this JsonObject target, string propertyName, params JsonObject[] items)
    {
        JsonArray arr = new();
        foreach (JsonObject s in items)
            arr.Add(s);
        target.Add(propertyName, arr);
        return target;
    }

    public static bool TryGetPropertyString(this JsonObject target, string propertyName, [NotNullWhen(true)] out string? result)
    {
        JsonNode? jsonNode = target[propertyName];
        if (jsonNode is null)
        {
            result = null;
            return false;
        }
        result = jsonNode.GetValue<string>();
        return true;
    }

    public static bool TryGetPropertyInt(this JsonObject target, string propertyName, out int result)
    {
        JsonNode? jsonNode = target[propertyName];
        if (jsonNode is null)
        {
            result = default;
            return false;
        }
        result = jsonNode.GetValue<int>();
        return true;
    }

    public static bool TryGetPropertyArray(this JsonObject target, string propertyName, [NotNullWhen(true)] out JsonArray? result)
    {
        result = target[propertyName] as JsonArray;
        return result is not null;
    }
}
