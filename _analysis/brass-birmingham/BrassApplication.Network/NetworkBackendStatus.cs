namespace BrassApplication.Network;

public enum NetworkBackendStatus
{
	Success,
	Error,
	Timeout,
	ErrorParsingResponse,
	RegisterEmailIsAlreadyTaken,
	ProvidedEmailIsNotValidEmailAddress,
	RegisterPasswordHasToBeAtLeast6Characters,
	UnauthorizedAndNeedToLoginAgain,
	NotFound,
	EmailHasToBeProvided,
	PasswordHasToBeProvided,
	NameHasToBeProvided
}
