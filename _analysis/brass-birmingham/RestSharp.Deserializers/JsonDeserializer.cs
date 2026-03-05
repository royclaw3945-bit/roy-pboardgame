using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using RestSharp.Extensions;

namespace RestSharp.Deserializers;

public class JsonDeserializer : IDeserializer
{
	public string RootElement { get; set; }

	public string Namespace { get; set; }

	public string DateFormat { get; set; }

	public CultureInfo Culture { get; set; }

	public JsonDeserializer()
	{
		Culture = CultureInfo.InvariantCulture;
	}

	public T Deserialize<T>(IRestResponse response)
	{
		T val = Activator.CreateInstance<T>();
		if (val is IList)
		{
			Type type = val.GetType();
			if (RootElement.HasValue())
			{
				object parent = FindRoot(response.Content);
				return (T)BuildList(type, parent);
			}
			object parent2 = SimpleJson.DeserializeObject(response.Content);
			return (T)BuildList(type, parent2);
		}
		if (val is IDictionary)
		{
			object parent3 = FindRoot(response.Content);
			return (T)BuildDictionary(val.GetType(), parent3);
		}
		object obj = FindRoot(response.Content);
		return (T)Map(val, (IDictionary<string, object>)obj);
	}

	private object FindRoot(string content)
	{
		IDictionary<string, object> dictionary = (IDictionary<string, object>)SimpleJson.DeserializeObject(content);
		if (RootElement.HasValue() && dictionary.ContainsKey(RootElement))
		{
			return dictionary[RootElement];
		}
		return dictionary;
	}

	private object Map(object target, IDictionary<string, object> data)
	{
		foreach (PropertyInfo item in (from p in target.GetType().GetProperties()
			where p.CanWrite
			select p).ToList())
		{
			Type propertyType = item.PropertyType;
			object[] customAttributes = item.GetCustomAttributes(typeof(DeserializeAsAttribute), inherit: false);
			string text = ((customAttributes.Length == 0) ? item.Name : ((DeserializeAsAttribute)customAttributes[0]).Name);
			string[] array = text.Split('.');
			IDictionary<string, object> dictionary = data;
			object obj = null;
			for (int num = 0; num < array.Length; num++)
			{
				string text2 = array[num].GetNameVariants(Culture).FirstOrDefault(dictionary.ContainsKey);
				if (text2 == null)
				{
					break;
				}
				if (num == array.Length - 1)
				{
					obj = dictionary[text2];
				}
				else
				{
					dictionary = (IDictionary<string, object>)dictionary[text2];
				}
			}
			if (obj != null)
			{
				item.SetValue(target, ConvertValue(propertyType, obj), null);
			}
		}
		return target;
	}

	private IDictionary BuildDictionary(Type type, object parent)
	{
		IDictionary dictionary = (IDictionary)Activator.CreateInstance(type);
		Type type2 = type.GetGenericArguments()[1];
		foreach (KeyValuePair<string, object> item in (IDictionary<string, object>)parent)
		{
			string key = item.Key;
			object obj = null;
			obj = ((!type2.IsGenericType || !(type2.GetGenericTypeDefinition() == typeof(List<>))) ? ConvertValue(type2, item.Value) : BuildList(type2, item.Value));
			dictionary.Add(key, obj);
		}
		return dictionary;
	}

	private IList BuildList(Type type, object parent)
	{
		IList list = (IList)Activator.CreateInstance(type);
		Type type2 = type.GetInterfaces().First((Type x) => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>)).GetGenericArguments()[0];
		if (parent is IList)
		{
			foreach (object item in (IList)parent)
			{
				if (type2.IsPrimitive)
				{
					object value = ConvertValue(type2, item);
					list.Add(value);
				}
				else if (type2 == typeof(string))
				{
					if (item == null)
					{
						list.Add(null);
					}
					else
					{
						list.Add(item.ToString());
					}
				}
				else if (item == null)
				{
					list.Add(null);
				}
				else
				{
					object value2 = ConvertValue(type2, item);
					list.Add(value2);
				}
			}
		}
		else
		{
			list.Add(ConvertValue(type2, parent));
		}
		return list;
	}

	private object ConvertValue(Type type, object value)
	{
		string text = Convert.ToString(value, Culture);
		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
		{
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			type = type.GetGenericArguments()[0];
		}
		if (type == typeof(object) && value != null)
		{
			type = value.GetType();
		}
		if (type.IsPrimitive)
		{
			return value.ChangeType(type, Culture);
		}
		if (type.IsEnum)
		{
			return type.FindEnumValue(text, Culture);
		}
		if (type == typeof(Uri))
		{
			return new Uri(text, UriKind.RelativeOrAbsolute);
		}
		if (type == typeof(string))
		{
			return text;
		}
		if (type == typeof(DateTime) || type == typeof(DateTimeOffset))
		{
			DateTime dateTime = ((!DateFormat.HasValue()) ? text.ParseJsonDate(Culture) : DateTime.ParseExact(text, DateFormat, Culture));
			if (type == typeof(DateTime))
			{
				return dateTime;
			}
			if (type == typeof(DateTimeOffset))
			{
				return (DateTimeOffset)dateTime;
			}
		}
		else
		{
			if (type == typeof(decimal))
			{
				if (value is double)
				{
					return (decimal)(double)value;
				}
				return decimal.Parse(text, Culture);
			}
			if (type == typeof(Guid))
			{
				return string.IsNullOrEmpty(text) ? Guid.Empty : new Guid(text);
			}
			if (type == typeof(TimeSpan))
			{
				return TimeSpan.Parse(text);
			}
			if (!type.IsGenericType)
			{
				if (type.IsSubclassOfRawGeneric(typeof(List<>)))
				{
					return BuildList(type, value);
				}
				if (type == typeof(JsonObject))
				{
					return BuildDictionary(typeof(Dictionary<string, object>), value);
				}
				return CreateAndMap(type, value);
			}
			Type genericTypeDefinition = type.GetGenericTypeDefinition();
			if (genericTypeDefinition == typeof(List<>))
			{
				return BuildList(type, value);
			}
			if (!(genericTypeDefinition == typeof(Dictionary<, >)))
			{
				return CreateAndMap(type, value);
			}
			if (type.GetGenericArguments()[0] == typeof(string))
			{
				return BuildDictionary(type, value);
			}
		}
		return null;
	}

	private object CreateAndMap(Type type, object element)
	{
		object obj = Activator.CreateInstance(type);
		Map(obj, (IDictionary<string, object>)element);
		return obj;
	}
}
