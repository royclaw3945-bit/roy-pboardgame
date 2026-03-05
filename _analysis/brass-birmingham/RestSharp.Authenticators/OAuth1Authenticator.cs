using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp.Authenticators.OAuth;
using RestSharp.Authenticators.OAuth.Extensions;
using RestSharp.Contrib;

namespace RestSharp.Authenticators;

public class OAuth1Authenticator : IAuthenticator
{
	public virtual string Realm { get; set; }

	public virtual OAuthParameterHandling ParameterHandling { get; set; }

	public virtual OAuthSignatureMethod SignatureMethod { get; set; }

	public virtual OAuthSignatureTreatment SignatureTreatment { get; set; }

	internal virtual OAuthType Type { get; set; }

	internal virtual string ConsumerKey { get; set; }

	internal virtual string ConsumerSecret { get; set; }

	internal virtual string Token { get; set; }

	internal virtual string TokenSecret { get; set; }

	internal virtual string Verifier { get; set; }

	internal virtual string Version { get; set; }

	internal virtual string CallbackUrl { get; set; }

	internal virtual string SessionHandle { get; set; }

	internal virtual string ClientUsername { get; set; }

	internal virtual string ClientPassword { get; set; }

	public static OAuth1Authenticator ForRequestToken(string consumerKey, string consumerSecret)
	{
		return new OAuth1Authenticator
		{
			ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
			SignatureMethod = OAuthSignatureMethod.HmacSha1,
			SignatureTreatment = OAuthSignatureTreatment.Escaped,
			ConsumerKey = consumerKey,
			ConsumerSecret = consumerSecret,
			Type = OAuthType.RequestToken
		};
	}

	public static OAuth1Authenticator ForRequestToken(string consumerKey, string consumerSecret, string callbackUrl)
	{
		OAuth1Authenticator oAuth1Authenticator = ForRequestToken(consumerKey, consumerSecret);
		oAuth1Authenticator.CallbackUrl = callbackUrl;
		return oAuth1Authenticator;
	}

	public static OAuth1Authenticator ForAccessToken(string consumerKey, string consumerSecret, string token, string tokenSecret)
	{
		return new OAuth1Authenticator
		{
			ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
			SignatureMethod = OAuthSignatureMethod.HmacSha1,
			SignatureTreatment = OAuthSignatureTreatment.Escaped,
			ConsumerKey = consumerKey,
			ConsumerSecret = consumerSecret,
			Token = token,
			TokenSecret = tokenSecret,
			Type = OAuthType.AccessToken
		};
	}

	public static OAuth1Authenticator ForAccessToken(string consumerKey, string consumerSecret, string token, string tokenSecret, string verifier)
	{
		OAuth1Authenticator oAuth1Authenticator = ForAccessToken(consumerKey, consumerSecret, token, tokenSecret);
		oAuth1Authenticator.Verifier = verifier;
		return oAuth1Authenticator;
	}

	public static OAuth1Authenticator ForAccessTokenRefresh(string consumerKey, string consumerSecret, string token, string tokenSecret, string sessionHandle)
	{
		OAuth1Authenticator oAuth1Authenticator = ForAccessToken(consumerKey, consumerSecret, token, tokenSecret);
		oAuth1Authenticator.SessionHandle = sessionHandle;
		return oAuth1Authenticator;
	}

	public static OAuth1Authenticator ForAccessTokenRefresh(string consumerKey, string consumerSecret, string token, string tokenSecret, string verifier, string sessionHandle)
	{
		OAuth1Authenticator oAuth1Authenticator = ForAccessToken(consumerKey, consumerSecret, token, tokenSecret);
		oAuth1Authenticator.SessionHandle = sessionHandle;
		oAuth1Authenticator.Verifier = verifier;
		return oAuth1Authenticator;
	}

	public static OAuth1Authenticator ForClientAuthentication(string consumerKey, string consumerSecret, string username, string password)
	{
		return new OAuth1Authenticator
		{
			ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
			SignatureMethod = OAuthSignatureMethod.HmacSha1,
			SignatureTreatment = OAuthSignatureTreatment.Escaped,
			ConsumerKey = consumerKey,
			ConsumerSecret = consumerSecret,
			ClientUsername = username,
			ClientPassword = password,
			Type = OAuthType.ClientAuthentication
		};
	}

	public static OAuth1Authenticator ForProtectedResource(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
	{
		return new OAuth1Authenticator
		{
			Type = OAuthType.ProtectedResource,
			ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
			SignatureMethod = OAuthSignatureMethod.HmacSha1,
			SignatureTreatment = OAuthSignatureTreatment.Escaped,
			ConsumerKey = consumerKey,
			ConsumerSecret = consumerSecret,
			Token = accessToken,
			TokenSecret = accessTokenSecret
		};
	}

	public void Authenticate(IRestClient client, IRestRequest request)
	{
		OAuthWorkflow workflow = new OAuthWorkflow
		{
			ConsumerKey = ConsumerKey,
			ConsumerSecret = ConsumerSecret,
			ParameterHandling = ParameterHandling,
			SignatureMethod = SignatureMethod,
			SignatureTreatment = SignatureTreatment,
			Verifier = Verifier,
			Version = Version,
			CallbackUrl = CallbackUrl,
			SessionHandle = SessionHandle,
			Token = Token,
			TokenSecret = TokenSecret,
			ClientUsername = ClientUsername,
			ClientPassword = ClientPassword
		};
		AddOAuthData(client, request, workflow);
	}

	private void AddOAuthData(IRestClient client, IRestRequest request, OAuthWorkflow workflow)
	{
		string text = client.BuildUri(request).ToString();
		int num = text.IndexOf('?');
		if (num != -1)
		{
			text = text.Substring(0, num);
		}
		string method = request.Method.ToString().ToUpperInvariant();
		WebParameterCollection webParameterCollection = new WebParameterCollection();
		if (!request.AlwaysMultipartFormData && !request.Files.Any())
		{
			foreach (Parameter item in client.DefaultParameters.Where((Parameter p) => p.Type == ParameterType.GetOrPost))
			{
				webParameterCollection.Add(new WebPair(item.Name, item.Value.ToString()));
			}
			foreach (Parameter item2 in request.Parameters.Where((Parameter p) => p.Type == ParameterType.GetOrPost))
			{
				webParameterCollection.Add(new WebPair(item2.Name, item2.Value.ToString()));
			}
		}
		else
		{
			foreach (Parameter item3 in client.DefaultParameters.Where((Parameter p) => p.Type == ParameterType.GetOrPost && p.Name.StartsWith("oauth_")))
			{
				webParameterCollection.Add(new WebPair(item3.Name, item3.Value.ToString()));
			}
			foreach (Parameter item4 in request.Parameters.Where((Parameter p) => p.Type == ParameterType.GetOrPost && p.Name.StartsWith("oauth_")))
			{
				webParameterCollection.Add(new WebPair(item4.Name, item4.Value.ToString()));
			}
		}
		OAuthWebQueryInfo oAuthWebQueryInfo;
		switch (Type)
		{
		case OAuthType.RequestToken:
			workflow.RequestTokenUrl = text;
			oAuthWebQueryInfo = workflow.BuildRequestTokenInfo(method, webParameterCollection);
			break;
		case OAuthType.AccessToken:
			workflow.AccessTokenUrl = text;
			oAuthWebQueryInfo = workflow.BuildAccessTokenInfo(method, webParameterCollection);
			break;
		case OAuthType.ClientAuthentication:
			workflow.AccessTokenUrl = text;
			oAuthWebQueryInfo = workflow.BuildClientAuthAccessTokenInfo(method, webParameterCollection);
			break;
		case OAuthType.ProtectedResource:
			oAuthWebQueryInfo = workflow.BuildProtectedResourceInfo(method, webParameterCollection, text);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		switch (ParameterHandling)
		{
		case OAuthParameterHandling.HttpAuthorizationHeader:
			webParameterCollection.Add("oauth_signature", oAuthWebQueryInfo.Signature);
			request.AddHeader("Authorization", GetAuthorizationHeader(webParameterCollection));
			break;
		case OAuthParameterHandling.UrlOrPostParameters:
			webParameterCollection.Add("oauth_signature", oAuthWebQueryInfo.Signature);
			{
				foreach (WebPair item5 in webParameterCollection.Where((WebPair parameter) => !parameter.Name.IsNullOrBlank() && (parameter.Name.StartsWith("oauth_") || parameter.Name.StartsWith("x_auth_"))))
				{
					request.AddParameter(item5.Name, HttpUtility.UrlDecode(item5.Value));
				}
				break;
			}
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private string GetAuthorizationHeader(WebPairCollection parameters)
	{
		StringBuilder stringBuilder = new StringBuilder("OAuth ");
		if (!Realm.IsNullOrBlank())
		{
			stringBuilder.Append("realm=\"{0}\",".FormatWith(OAuthTools.UrlEncodeRelaxed(Realm)));
		}
		parameters.Sort((WebPair l, WebPair r) => l.Name.CompareTo(r.Name));
		int num = 0;
		List<WebPair> list = parameters.Where((WebPair parameter) => !parameter.Name.IsNullOrBlank() && !parameter.Value.IsNullOrBlank() && (parameter.Name.StartsWith("oauth_") || parameter.Name.StartsWith("x_auth_"))).ToList();
		foreach (WebPair item in list)
		{
			num++;
			string format = ((num < list.Count) ? "{0}=\"{1}\"," : "{0}=\"{1}\"");
			stringBuilder.Append(format.FormatWith(item.Name, item.Value));
		}
		return stringBuilder.ToString();
	}
}
