using System;

namespace BrassApplication.Utils;

public static class ExceptionToTranslationKey
{
	public static string ConvertExceptionToTranslationKey(Exception exception)
	{
		return exception.Message switch
		{
			"The operation has timed out." => "Timeout", 
			"WrongGamePassword" => "WrongGamePassword", 
			"Attempted to perform an unauthorized operation." => "UnauthorizedAccess", 
			"Path not found" => "ServerError", 
			"Bad Gateway" => "ServerError", 
			"Unknown Error" => "UnknownError", 
			_ => "UnknownError", 
		};
	}
}
