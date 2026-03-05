using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using RestSharp.Extensions;

namespace RestSharp.Deserializers;

public class XmlDeserializer : IDeserializer
{
	public string RootElement { get; set; }

	public string Namespace { get; set; }

	public string DateFormat { get; set; }

	public CultureInfo Culture { get; set; }

	public XmlDeserializer()
	{
		Culture = CultureInfo.InvariantCulture;
	}

	public virtual T Deserialize<T>(IRestResponse response)
	{
		if (string.IsNullOrEmpty(response.Content))
		{
			return default(T);
		}
		XDocument xDocument = XDocument.Parse(response.Content);
		XElement root = xDocument.Root;
		if (RootElement.HasValue() && xDocument.Root != null)
		{
			root = xDocument.Root.Element(RootElement.AsNamespaced(Namespace));
		}
		if (!Namespace.HasValue())
		{
			RemoveNamespace(xDocument);
		}
		T val = Activator.CreateInstance<T>();
		Type type = val.GetType();
		if (type.IsSubclassOfRawGeneric(typeof(List<>)))
		{
			return (T)HandleListDerivative(val, root, type.Name, type);
		}
		return (T)Map(val, root);
	}

	private void RemoveNamespace(XDocument xdoc)
	{
		foreach (XElement item in xdoc.Root.DescendantsAndSelf())
		{
			if (item.Name.Namespace != XNamespace.None)
			{
				item.Name = XNamespace.None.GetName(item.Name.LocalName);
			}
			if (item.Attributes().Any((XAttribute a) => a.IsNamespaceDeclaration || a.Name.Namespace != XNamespace.None))
			{
				item.ReplaceAttributes(from a in item.Attributes()
					select (!a.IsNamespaceDeclaration) ? ((!(a.Name.Namespace != XNamespace.None)) ? a : new XAttribute(XNamespace.None.GetName(a.Name.LocalName), a.Value)) : null);
			}
		}
	}

	protected virtual object Map(object x, XElement root)
	{
		PropertyInfo[] properties = x.GetType().GetProperties();
		foreach (PropertyInfo propertyInfo in properties)
		{
			Type type = propertyInfo.PropertyType;
			if ((!type.IsPublic && !type.IsNestedPublic) || !propertyInfo.CanWrite)
			{
				continue;
			}
			object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(DeserializeAsAttribute), inherit: false);
			XName name = ((customAttributes.Length == 0) ? propertyInfo.Name.AsNamespaced(Namespace) : ((DeserializeAsAttribute)customAttributes[0]).Name.AsNamespaced(Namespace));
			object valueFromXml = GetValueFromXml(root, name, propertyInfo);
			if (valueFromXml == null)
			{
				if (type.IsGenericType)
				{
					Type type2 = type.GetGenericArguments()[0];
					XElement elementByName = GetElementByName(root, type2.Name);
					IList list = (IList)Activator.CreateInstance(type);
					if (elementByName != null)
					{
						IEnumerable<XElement> elements = root.Elements(elementByName.Name);
						PopulateListFromElements(type2, elements, list);
					}
					propertyInfo.SetValue(x, list, null);
				}
				continue;
			}
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				if (valueFromXml == null || string.IsNullOrEmpty(valueFromXml.ToString()))
				{
					propertyInfo.SetValue(x, null, null);
					continue;
				}
				type = type.GetGenericArguments()[0];
			}
			object result2;
			if (type == typeof(bool))
			{
				string s = valueFromXml.ToString().ToLower();
				propertyInfo.SetValue(x, XmlConvert.ToBoolean(s), null);
			}
			else if (type.IsPrimitive)
			{
				propertyInfo.SetValue(x, valueFromXml.ChangeType(type, Culture), null);
			}
			else if (type.IsEnum)
			{
				object value = type.FindEnumValue(valueFromXml.ToString(), Culture);
				propertyInfo.SetValue(x, value, null);
			}
			else if (type == typeof(Uri))
			{
				Uri value2 = new Uri(valueFromXml.ToString(), UriKind.RelativeOrAbsolute);
				propertyInfo.SetValue(x, value2, null);
			}
			else if (type == typeof(string))
			{
				propertyInfo.SetValue(x, valueFromXml, null);
			}
			else if (type == typeof(DateTime))
			{
				valueFromXml = ((!DateFormat.HasValue()) ? ((object)DateTime.Parse(valueFromXml.ToString(), Culture)) : ((object)DateTime.ParseExact(valueFromXml.ToString(), DateFormat, Culture)));
				propertyInfo.SetValue(x, valueFromXml, null);
			}
			else if (type == typeof(DateTimeOffset))
			{
				string text = valueFromXml.ToString();
				if (string.IsNullOrEmpty(text))
				{
					continue;
				}
				try
				{
					DateTimeOffset dateTimeOffset = XmlConvert.ToDateTimeOffset(text);
					propertyInfo.SetValue(x, dateTimeOffset, null);
				}
				catch (Exception)
				{
					if (TryGetFromString(text, out var result, type))
					{
						propertyInfo.SetValue(x, result, null);
						continue;
					}
					DateTimeOffset dateTimeOffset = DateTimeOffset.Parse(text);
					propertyInfo.SetValue(x, dateTimeOffset, null);
				}
			}
			else if (type == typeof(decimal))
			{
				valueFromXml = decimal.Parse(valueFromXml.ToString(), Culture);
				propertyInfo.SetValue(x, valueFromXml, null);
			}
			else if (type == typeof(Guid))
			{
				valueFromXml = (string.IsNullOrEmpty(valueFromXml.ToString()) ? Guid.Empty : new Guid(valueFromXml.ToString()));
				propertyInfo.SetValue(x, valueFromXml, null);
			}
			else if (type == typeof(TimeSpan))
			{
				TimeSpan timeSpan = XmlConvert.ToTimeSpan(valueFromXml.ToString());
				propertyInfo.SetValue(x, timeSpan, null);
			}
			else if (type.IsGenericType)
			{
				Type t = type.GetGenericArguments()[0];
				IList list2 = (IList)Activator.CreateInstance(type);
				XElement elementByName2 = GetElementByName(root, propertyInfo.Name.AsNamespaced(Namespace));
				if (elementByName2.HasElements)
				{
					XElement xElement = elementByName2.Elements().FirstOrDefault();
					IEnumerable<XElement> elements2 = elementByName2.Elements(xElement.Name);
					PopulateListFromElements(t, elements2, list2);
				}
				propertyInfo.SetValue(x, list2, null);
			}
			else if (type.IsSubclassOfRawGeneric(typeof(List<>)))
			{
				object value3 = HandleListDerivative(x, root, propertyInfo.Name, type);
				propertyInfo.SetValue(x, value3, null);
			}
			else if (TryGetFromString(valueFromXml.ToString(), out result2, type))
			{
				propertyInfo.SetValue(x, result2, null);
			}
			else if (root != null)
			{
				XElement elementByName3 = GetElementByName(root, name);
				if (elementByName3 != null)
				{
					object value4 = CreateAndMap(type, elementByName3);
					propertyInfo.SetValue(x, value4, null);
				}
			}
		}
		return x;
	}

	private static bool TryGetFromString(string inputString, out object result, Type type)
	{
		TypeConverter converter = TypeDescriptor.GetConverter(type);
		if (converter.CanConvertFrom(typeof(string)))
		{
			result = converter.ConvertFromInvariantString(inputString);
			return true;
		}
		result = null;
		return false;
	}

	private void PopulateListFromElements(Type t, IEnumerable<XElement> elements, IList list)
	{
		foreach (XElement element in elements)
		{
			object value = CreateAndMap(t, element);
			list.Add(value);
		}
	}

	private object HandleListDerivative(object x, XElement root, string propName, Type type)
	{
		Type type2 = ((!type.IsGenericType) ? type.BaseType.GetGenericArguments()[0] : type.GetGenericArguments()[0]);
		IList list = (IList)Activator.CreateInstance(type);
		IEnumerable<XElement> enumerable = root.Descendants(type2.Name.AsNamespaced(Namespace));
		string name = type2.Name;
		if (!enumerable.Any())
		{
			XName name2 = name.ToLower().AsNamespaced(Namespace);
			enumerable = root.Descendants(name2);
		}
		if (!enumerable.Any())
		{
			XName name3 = name.ToCamelCase(Culture).AsNamespaced(Namespace);
			enumerable = root.Descendants(name3);
		}
		if (!enumerable.Any())
		{
			enumerable = from e in root.Descendants()
				where e.Name.LocalName.RemoveUnderscoresAndDashes() == name
				select e;
		}
		if (!enumerable.Any())
		{
			XName lowerName = name.ToLower().AsNamespaced(Namespace);
			enumerable = from e in root.Descendants()
				where e.Name.LocalName.RemoveUnderscoresAndDashes() == lowerName
				select e;
		}
		PopulateListFromElements(type2, enumerable, list);
		if (!type.IsGenericType)
		{
			Map(list, root.Element(propName.AsNamespaced(Namespace)) ?? root);
		}
		return list;
	}

	protected virtual object CreateAndMap(Type t, XElement element)
	{
		object obj;
		if (t == typeof(string))
		{
			obj = element.Value;
		}
		else if (t.IsPrimitive)
		{
			obj = element.Value.ChangeType(t, Culture);
		}
		else
		{
			obj = Activator.CreateInstance(t);
			Map(obj, element);
		}
		return obj;
	}

	protected virtual object GetValueFromXml(XElement root, XName name, PropertyInfo prop)
	{
		object result = null;
		if (root != null)
		{
			XElement elementByName = GetElementByName(root, name);
			if (elementByName == null)
			{
				XAttribute attributeByName = GetAttributeByName(root, name);
				if (attributeByName != null)
				{
					result = attributeByName.Value;
				}
			}
			else if (!elementByName.IsEmpty || elementByName.HasElements || elementByName.HasAttributes)
			{
				result = elementByName.Value;
			}
		}
		return result;
	}

	protected virtual XElement GetElementByName(XElement root, XName name)
	{
		XName name2 = name.LocalName.ToLower().AsNamespaced(name.NamespaceName);
		XName name3 = name.LocalName.ToCamelCase(Culture).AsNamespaced(name.NamespaceName);
		if (root.Element(name) != null)
		{
			return root.Element(name);
		}
		if (root.Element(name2) != null)
		{
			return root.Element(name2);
		}
		if (root.Element(name3) != null)
		{
			return root.Element(name3);
		}
		if (name == "Value".AsNamespaced(name.NamespaceName))
		{
			return root;
		}
		XElement xElement = (from d in root.Descendants()
			orderby d.Ancestors().Count()
			select d).FirstOrDefault((XElement d) => d.Name.LocalName.RemoveUnderscoresAndDashes() == name.LocalName) ?? (from d in root.Descendants()
			orderby d.Ancestors().Count()
			select d).FirstOrDefault((XElement d) => d.Name.LocalName.RemoveUnderscoresAndDashes() == name.LocalName.ToLower());
		if (xElement != null)
		{
			return xElement;
		}
		return null;
	}

	protected virtual XAttribute GetAttributeByName(XElement root, XName name)
	{
		XName name2 = name.LocalName.ToLower().AsNamespaced(name.NamespaceName);
		XName name3 = name.LocalName.ToCamelCase(Culture).AsNamespaced(name.NamespaceName);
		if (root.Attribute(name) != null)
		{
			return root.Attribute(name);
		}
		if (root.Attribute(name2) != null)
		{
			return root.Attribute(name2);
		}
		if (root.Attribute(name3) != null)
		{
			return root.Attribute(name3);
		}
		XAttribute xAttribute = root.Attributes().FirstOrDefault((XAttribute d) => d.Name.LocalName.RemoveUnderscoresAndDashes() == name.LocalName);
		if (xAttribute != null)
		{
			return xAttribute;
		}
		return null;
	}
}
