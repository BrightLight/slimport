namespace slimport
{
  using System;
  using System.Collections.Generic;
  using System.Xml;

  public class SisulizerProject
  {
    private readonly Dictionary<string, LangRow> content = new Dictionary<string, LangRow>();

    public SisulizerProject(string filename, string language)
    {
      var importProject = new XmlDocument();
      importProject.Load(filename);

      // iterate sources
      foreach (XmlElement rowNode in importProject.SelectNodes("//document/source/node/node/row"))
      {
        var context = GetContextOfNode(rowNode);
        foreach (XmlElement langNode in rowNode.SelectNodes($"lang[@id='{language}']"))
        {
          var langRow = new LangRow
          {
            Id = langNode.Attributes.GetValueOrDefault("id"),
            Date = langNode.Attributes.GetValueOrDefault("date"),
            Status = langNode.Attributes.GetValueOrDefault("status"),
            Content = langNode.InnerText,
            Invalidated = langNode.Attributes.GetValueOrDefault("invalidated") == "1",
          };
          this.content.Add(context, langRow);
        }

        ////Console.WriteLine(context);
      }
    }

    public LangRow GetLangRow(string context)
    {
      return this.content.TryGetValue(context, out var langRow) ? langRow : null;
    }

    public static string GetContextOfNode(XmlNode xmlNode)
    {
      string parentContext = "";
      if (xmlNode.ParentNode != null)
      {
        parentContext = GetContextOfNode(xmlNode.ParentNode);
        if (!string.IsNullOrEmpty(parentContext))
        {
          parentContext = parentContext + "/";
        }
      }

      switch (xmlNode.Name)
      {
        case "source":
          return parentContext + xmlNode.Attributes.GetValueOrDefault("name");
        case "row":
          return parentContext + xmlNode.Attributes.GetValueOrDefault("id");
        case "node":
          return parentContext + xmlNode.Attributes.GetValueOrDefault("name");
      }

      return null;
    }
  }
}