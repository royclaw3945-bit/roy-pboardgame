using System;
using System.Collections.Generic;

namespace BoardGameRules.Entities.EventSourcing.Events;

[Serializable]
public class EventMetadata : IEventMetadata
{
	private readonly string REVISION = "Revision";

	private Dictionary<string, object> Properties { get; set; }

	public int Revision
	{
		get
		{
			return Convert.ToInt32(Properties[REVISION]);
		}
		set
		{
			Properties[REVISION] = value;
		}
	}

	public EventMetadata()
	{
		Properties = new Dictionary<string, object>();
		Revision = -1;
	}
}
