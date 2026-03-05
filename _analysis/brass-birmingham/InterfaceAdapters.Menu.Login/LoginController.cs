using BrassApplication.UseCases.Menu.OnlineGame.Login;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Menu.Login;

public class LoginController : IController
{
	private readonly ILoginInput _loginUseCase;

	public LoginController(ILoginInput loginUseCase)
	{
		_loginUseCase = loginUseCase;
	}

	public void OnViewCreated()
	{
	}

	public void OnViewDestroyed()
	{
	}

	public void LoginClicked(string email, string password)
	{
		_loginUseCase.LoginClicked(email, password);
	}

	public void CreateClicked()
	{
		_loginUseCase.RegisterClicked();
	}

	public void ForgotPasswordClicked()
	{
		_loginUseCase.ForgotPasswordClicked();
	}

	public void BackClicked()
	{
		_loginUseCase.BackClicked();
	}
}
