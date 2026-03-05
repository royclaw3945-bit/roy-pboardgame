using BrassApplication.UseCases.Menu.OnlineGame.PasswordRecovery;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Menu.PasswordRecovery;

public class PasswordRecoveryController : IController
{
	private readonly IPasswordRecoveryInput _loginUseCase;

	public PasswordRecoveryController(IPasswordRecoveryInput loginUseCase)
	{
		_loginUseCase = loginUseCase;
	}

	public void OnViewCreated()
	{
	}

	public void OnViewDestroyed()
	{
	}

	public void SendEmailClicked(string email)
	{
		_loginUseCase.SendEmailClicked(email);
	}

	public void BackClicked()
	{
		_loginUseCase.BackClicked();
	}
}
