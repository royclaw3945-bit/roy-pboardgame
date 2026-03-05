namespace Frameworks.Init;

public interface IFrameworksDelegate
{
	void LoggerInit();

	void MainThreadDispatcherInit();

	void RouterInit();

	void LocalizationInit();
}
