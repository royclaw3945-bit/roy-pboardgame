using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using RestSharp.Serializers;

namespace RestSharp;

public class RestRequest : IRestRequest
{
	private Method _method;

	private DataFormat _requestFormat = DataFormat.Xml;

	private int _attempts;

	public bool AlwaysMultipartFormData { get; set; }

	public ISerializer JsonSerializer { get; set; }

	public ISerializer XmlSerializer { get; set; }

	public Action<Stream> ResponseWriter { get; set; }

	public bool UseDefaultCredentials { get; set; }

	public List<Parameter> Parameters { get; private set; }

	public List<FileParameter> Files { get; private set; }

	public Method Method
	{
		get
		{
			return _method;
		}
		set
		{
			_method = value;
		}
	}

	public string Resource { get; set; }

	public DataFormat RequestFormat
	{
		get
		{
			return _requestFormat;
		}
		set
		{
			_requestFormat = value;
		}
	}

	public string RootElement { get; set; }

	public Action<IRestResponse> OnBeforeDeserialization { get; set; }

	public string DateFormat { get; set; }

	public string XmlNamespace { get; set; }

	public ICredentials Credentials { get; set; }

	public object UserState { get; set; }

	public int Timeout { get; set; }

	public int ReadWriteTimeout { get; set; }

	public int Attempts => _attempts;

	public RestRequest()
	{
		Parameters = new List<Parameter>();
		Files = new List<FileParameter>();
		XmlSerializer = new XmlSerializer();
		JsonSerializer = new JsonSerializer();
		OnBeforeDeserialization = delegate
		{
		};
	}

	public RestRequest(Method method)
		: this()
	{
		Method = method;
	}

	public RestRequest(string resource)
		: this(resource, Method.GET)
	{
	}

	public RestRequest(string resource, Method method)
		: this()
	{
		Resource = resource;
		Method = method;
	}

	public RestRequest(Uri resource)
		: this(resource, Method.GET)
	{
	}

	public RestRequest(Uri resource, Method method)
		: this(resource.IsAbsoluteUri ? (resource.AbsolutePath + resource.Query) : resource.OriginalString, method)
	{
	}

	public IRestRequest AddFile(string name, string path)
	{
		long length = new FileInfo(path).Length;
		return AddFile(new FileParameter
		{
			Name = name,
			FileName = Path.GetFileName(path),
			ContentLength = length,
			Writer = delegate(Stream s)
			{
				using StreamReader streamReader = new StreamReader(path);
				streamReader.BaseStream.CopyTo(s);
			}
		});
	}

	public IRestRequest AddFile(string name, byte[] bytes, string fileName)
	{
		return AddFile(FileParameter.Create(name, bytes, fileName));
	}

	public IRestRequest AddFile(string name, byte[] bytes, string fileName, string contentType)
	{
		return AddFile(FileParameter.Create(name, bytes, fileName, contentType));
	}

	public IRestRequest AddFile(string name, Action<Stream> writer, string fileName)
	{
		return AddFile(name, writer, fileName, null);
	}

	public IRestRequest AddFile(string name, Action<Stream> writer, string fileName, string contentType)
	{
		return AddFile(new FileParameter
		{
			Name = name,
			Writer = writer,
			FileName = fileName,
			ContentType = contentType
		});
	}

	private IRestRequest AddFile(FileParameter file)
	{
		Files.Add(file);
		return this;
	}

	public IRestRequest AddBody(object obj, string xmlNamespace)
	{
		string value;
		string name;
		switch (RequestFormat)
		{
		case DataFormat.Json:
			value = JsonSerializer.Serialize(obj);
			name = JsonSerializer.ContentType;
			break;
		case DataFormat.Xml:
			XmlSerializer.Namespace = xmlNamespace;
			value = XmlSerializer.Serialize(obj);
			name = XmlSerializer.ContentType;
			break;
		default:
			value = "";
			name = "";
			break;
		}
		return AddParameter(name, value, ParameterType.RequestBody);
	}

	public IRestRequest AddBody(object obj)
	{
		return AddBody(obj, "");
	}

	public IRestRequest AddJsonBody(object obj)
	{
		RequestFormat = DataFormat.Json;
		return AddBody(obj, "");
	}

	public IRestRequest AddXmlBody(object obj)
	{
		RequestFormat = DataFormat.Xml;
		return AddBody(obj, "");
	}

	public IRestRequest AddXmlBody(object obj, string xmlNamespace)
	{
		RequestFormat = DataFormat.Xml;
		return AddBody(obj, xmlNamespace);
	}

	public IRestRequest AddObject(object obj, params string[] includedProperties)
	{
		PropertyInfo[] properties = obj.GetType().GetProperties();
		foreach (PropertyInfo propertyInfo in properties)
		{
			if (includedProperties.Length != 0 && (includedProperties.Length == 0 || !includedProperties.Contains(propertyInfo.Name)))
			{
				continue;
			}
			Type propertyType = propertyInfo.PropertyType;
			object obj2 = propertyInfo.GetValue(obj, null);
			if (obj2 == null)
			{
				continue;
			}
			if (propertyType.IsArray)
			{
				Type elementType = propertyType.GetElementType();
				if (((Array)obj2).Length > 0 && (elementType.IsPrimitive || elementType.IsValueType || elementType == typeof(string)))
				{
					string[] value = (from object item in (Array)obj2
						select item.ToString()).ToArray();
					obj2 = string.Join(",", value);
				}
				else
				{
					obj2 = string.Join(",", (string[])obj2);
				}
			}
			AddParameter(propertyInfo.Name, obj2);
		}
		return this;
	}

	public IRestRequest AddObject(object obj)
	{
		AddObject(obj, new string[0]);
		return this;
	}

	public IRestRequest AddParameter(Parameter p)
	{
		Parameters.Add(p);
		return this;
	}

	public IRestRequest AddParameter(string name, object value)
	{
		return AddParameter(new Parameter
		{
			Name = name,
			Value = value,
			Type = ParameterType.GetOrPost
		});
	}

	public IRestRequest AddParameter(string name, object value, ParameterType type)
	{
		return AddParameter(new Parameter
		{
			Name = name,
			Value = value,
			Type = type
		});
	}

	public IRestRequest AddHeader(string name, string value)
	{
		Func<string, bool> func = (string host) => Uri.CheckHostName(Regex.Split(host, ":\\d+")[0]) == UriHostNameType.Unknown;
		if (name == "Host" && func(value))
		{
			throw new ArgumentException("The specified value is not a valid Host header string.", "value");
		}
		return AddParameter(name, value, ParameterType.HttpHeader);
	}

	public IRestRequest AddCookie(string name, string value)
	{
		return AddParameter(name, value, ParameterType.Cookie);
	}

	public IRestRequest AddUrlSegment(string name, string value)
	{
		return AddParameter(name, value, ParameterType.UrlSegment);
	}

	public IRestRequest AddQueryParameter(string name, string value)
	{
		return AddParameter(name, value, ParameterType.QueryString);
	}

	public void IncreaseNumAttempts()
	{
		_attempts++;
	}
}
