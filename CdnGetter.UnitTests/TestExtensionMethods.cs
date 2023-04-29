using System;
using System.Collections.Generic;
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
}