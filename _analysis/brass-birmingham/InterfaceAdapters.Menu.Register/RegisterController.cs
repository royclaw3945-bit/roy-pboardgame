using BrassApplication.UseCases.Menu.OnlineGame.Register;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Menu.Register;

public class RegisterController : IController
{
	private readonly IRegisterInput _loginUseCase;

	public RegisterController(IRegisterInput loginUseCase)
	{
		_loginUseCase = loginUseCase;
	}

	public void OnViewCreated()
	{
	}

	public void OnViewDestroyed()
	{
	}

	public void RegisterClicked(string email, string nickname, string password, string confirmPassword)
	{
		_loginUseCase.RegisterClicked(email, nickname, password, confirmPassword);
	}

	public void BackClicked()
	{
		_loginUseCase.BackClicked();
	}
}
