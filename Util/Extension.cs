using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static partial class Extension
{
    private static Dictionary<Type, List<MemberInfo>> typeMembers = new Dictionary<Type, List<MemberInfo>>();

    public static void SetFields(this MonoBehaviour behaviour)
    {
        var bType = behaviour.GetType();
        var cType = typeof(SetField);
        List<MemberInfo> members;

        if (typeMembers.ContainsKey(bType))
        {
            members = typeMembers[bType];
        }
        else
        {
            members = new List<MemberInfo>();

            bType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m =>
                   (m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property)
                   && m.GetCustomAttributes(cType, true).Length == 1)
                   .ToList()
                   .ForEach((FieldInfo fieldInfo)
                   =>
                   {
                       members.Add(fieldInfo as MemberInfo);
                   });

            bType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttributes(cType, true).Length == 1)
                .ToList()
                .ForEach(propertyInfo =>
                {
                    members.Add(propertyInfo as MemberInfo);
                });

            members.OrderBy(m => m.MemberType).ThenBy(m => m.Name);
            typeMembers.Add(bType, members);
        }

        Action<MemberInfo, object> setValue = (MemberInfo info, object value) =>
        {
            if (info is FieldInfo)
            {
                FieldInfo fieldInfo = info as FieldInfo;
                fieldInfo.SetValue(behaviour, value);
            }
            else if (info is PropertyInfo)
            {
                PropertyInfo propertyInfo = info as PropertyInfo;
                propertyInfo.SetValue(behaviour, value, null);
            }
        };

        foreach (MemberInfo item in members)
        {
            var attribute = item.GetCustomAttributes(cType, true)[0] as SetField;
            if(attribute.type.Equals(typeof(GameObject)))
            {
                var obj = behaviour.transform.Find(attribute.name);
                setValue(item, obj.gameObject);
                continue;
            }
            
            var childs = behaviour.transform.GetComponentsInChildren(attribute.type, true);
            if (childs != null)
            {
                try
                {
                    Component com = childs.Where(c => (c.name == attribute.name)).FirstOrDefault();
                    if (com != null)
                        setValue(item, com);
                }
                catch (Exception e)
                {
                    Debug.LogError(string.Format("check SetField hierachy name : {0}", behaviour.ToString()));
                }
            }
            else
            {
                Debug.LogError("check SetField type");
            }
        }
    }
}

/// <summary>
/// use [SetField(typeof(TYPE), "Hierarchy Name"))]
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class SetField : Attribute
{
    public readonly string name;
    public readonly Type type;

    public SetField(Type type, string name)
    {
        this.type = type;
        this.name = name;
    }
}