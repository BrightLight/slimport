namespace slimport
{
  using System;

  public class ConsoleLogger : ILogger
  {
    private string context;

    public void Update(string name, string oldValue, string newValue)
    {
      if (string.IsNullOrEmpty(oldValue))
      {
        Console.WriteLine($"{this.context}: Add {name}: [{newValue}]");
      }
      else
      {
        Console.WriteLine($"{this.context}: Update {name} from [{oldValue}] to [{newValue}]");
      }
    }

    public void Delete(string name)
    {
      Console.WriteLine($"{this.context}: Delete {name}");
    }

    public void SetContext(string context)
    {
      this.context = context;
    }
  }

  public class XmlLogger : ILogger
  {
    public void Update(string name, string oldValue, string newValue)
    {
      // TODO_IMPLEMENT_ME();
    }

    public void Delete(string name)
    {
      // TODO_IMPLEMENT_ME();
    }

    public void SetContext(string context)
    {
      // TODO_IMPLEMENT_ME();
    }
  }
}