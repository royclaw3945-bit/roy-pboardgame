namespace RestSharp;

public class RestResponse<T> : RestResponseBase, IRestResponse<T>, IRestResponse
{
	public T Data { get; set; }

	public static explicit operator RestResponse<T>(RestResponse response)
	{
		return new RestResponse<T>
		{
			ContentEncoding = response.ContentEncoding,
			ContentLength = response.ContentLength,
			ContentType = response.ContentType,
			Cookies = response.Cookies,
			ErrorMessage = response.ErrorMessage,
			ErrorException = response.ErrorException,
			Headers = response.Headers,
			RawBytes = response.RawBytes,
			ResponseStatus = response.ResponseStatus,
			ResponseUri = response.ResponseUri,
			Server = response.Server,
			StatusCode = response.StatusCode,
			StatusDescription = response.StatusDescription,
			Request = response.Request
		};
	}
}
public class RestResponse : RestResponseBase, IRestResponse
{
}
