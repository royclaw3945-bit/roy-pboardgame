namespace BrassApplication.Utils;

public interface ISettingStorage
{
	void SetFloat(string key, float value);

	void SetInt(string key, int value);

	void SetString(string key, string value);

	float GetFloat(string key, float defaultValue = 0f);

	int GetInt(string key, int defaultValue = 0);

	string GetString(string key, string defaultValue = "");

	bool HasKey(string key);

	void DeleteKey(string key);

	void Save();
}
