using System;
using System.Globalization;
using RestSharp.Extensions;

namespace RestSharp.Serializers;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class SerializeAsAttribute : Attribute
{
	public string Name { get; set; }

	public bool Attribute { get; set; }

	public CultureInfo Culture { get; set; }

	public NameStyle NameStyle { get; set; }

	public int Index { get; set; }

	public SerializeAsAttribute()
	{
		NameStyle = NameStyle.AsIs;
		Index = int.MaxValue;
		Culture = CultureInfo.InvariantCulture;
	}

	public string TransformName(string input)
	{
		string text = Name ?? input;
		return NameStyle switch
		{
			NameStyle.CamelCase => text.ToCamelCase(Culture), 
			NameStyle.PascalCase => text.ToPascalCase(Culture), 
			NameStyle.LowerCase => text.ToLower(), 
			_ => input, 
		};
	}
}
