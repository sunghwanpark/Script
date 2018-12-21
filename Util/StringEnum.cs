using System;
using System.Reflection;

public static class StringEnum
{
    public static string GetStringValue(this Enum value)
    {
        string output = null;

        Type type = value.GetType();

        FieldInfo fi = type.GetField(value.ToString());
        StringValue[] attrs = fi.GetCustomAttributes(typeof(StringValue), false) as StringValue[];

        if (attrs.Length > 0)
        {
            output = attrs[0].Value;
        }

        return output;
    }
}

public class StringValue : Attribute
{
    public StringValue(string value)
    {
        Value = value;
    }

    public string Value { get; private set; }
}