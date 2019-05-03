namespace slimport
{
  public interface ILogger
  {
    void Update(string name, string oldValue, string newValue);
    void Delete(string name);
    void SetContext(string context);
  }
}