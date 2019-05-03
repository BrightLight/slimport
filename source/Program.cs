namespace slimport
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;
  using System.Text;
  using System.Xml;

  class Program
  {
    static void Main(string[] args)
    {
      if (args.Length < 2)
      {
        Console.WriteLine("Usage:");
        Console.WriteLine("slimport <mainprojectfile> <lang>:<languageimportfile>");
        Environment.Exit(1);
      }

      var logger = new ConsoleLogger();
      var languageArgument = args[1];
      var importProjects = new Dictionary<string, SisulizerProject>();
      foreach (var languageCommandLineArgument in args.Where(x => x.Contains("=")))
      {
        var languageArgumentParts = languageCommandLineArgument.Split("=");
        var language = languageArgumentParts[0];
        var languageImportFile = languageArgumentParts[1];
        var importProject = new SisulizerProject(languageImportFile, language);
        importProjects.Add(language, importProject);
      }

      var mainProjectFileName = args[0];
      var mainProject = new XmlDocument();
      mainProject.Load(mainProjectFileName);
      foreach (XmlElement rowNode in mainProject.SelectNodes("//document/source/node/node/row"))
      {
        var context = SisulizerProject.GetContextOfNode(rowNode);
        logger.SetContext(context);
        foreach (var (language, importProject) in importProjects)
        {
          var langRow = importProject.GetLangRow(context);
          if (langRow != null)
          {
            var existingLangNode = rowNode.ChildNodes.Cast<XmlNode>().FirstOrDefault(x => x.Name == "lang" && x.Attributes["id"].Value == language);
            if (existingLangNode != null)
            {
              // update
              UpdateNode(existingLangNode, langRow, logger);
            }
            else
            {
              // append
              if (rowNode.ChildNodes.OfType<XmlNode>().Any(x => x.NodeType == XmlNodeType.Text))
              {
                var nativeText = rowNode.InnerText;
                rowNode.InnerText = string.Empty;
                rowNode.AppendChild(mainProject.CreateWhitespace(Environment.NewLine));
                var nativeElement = mainProject.CreateElement("native");
                nativeElement.InnerText = nativeText;
                rowNode.AppendChild(nativeElement);
              }

              rowNode.AppendChild(mainProject.CreateWhitespace(Environment.NewLine));
              var newLangNode = mainProject.CreateElement("lang");
              newLangNode.SetOrUpdateAttribute("id", language, logger);
              UpdateNode(newLangNode, langRow, logger);
              rowNode.AppendChild(newLangNode);
              rowNode.AppendChild(mainProject.CreateWhitespace(Environment.NewLine));
            }
          }
        }
      }

      var xmlWriterSettings = new XmlWriterSettings();
      xmlWriterSettings.Encoding = Encoding.UTF8;
      xmlWriterSettings.Indent = true;
      xmlWriterSettings.IndentChars = "  ";
      xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;
      xmlWriterSettings.CloseOutput = true;
      xmlWriterSettings.NewLineChars = Environment.NewLine;
      var xmlWriter = XmlWriter.Create(@"c:\externalprojects\slimport\demos\newimport.slp", xmlWriterSettings);

      mainProject.Save(xmlWriter);
      xmlWriter.Flush();
    }

    private static void UpdateNode(XmlNode xmlNode, LangRow langRow, ILogger logger)
    {
      ////var langRowDate = DateTime.ParseExact(langRow.Date, "yyyyMMddhhmmss", CultureInfo.InvariantCulture);
      ////var sourceDate = DateTime.ParseExact(xmlNode.Attributes["date"].Value, "yyyyMMddhhmmss", CultureInfo.InvariantCulture);

      xmlNode.SetOrUpdateAttribute("date", langRow.Date, logger);
      xmlNode.SetOrUpdateAttribute("status", langRow.Status, logger);
      xmlNode.SetOrUpdateAttribute("invalidated", langRow.Invalidated ? "1" : "", logger);

      if (xmlNode.InnerText != langRow.Content)
      {
        logger?.Update("content", xmlNode.InnerText, langRow.Content);
        xmlNode.InnerText = langRow.Content;
      }
    }
  }
}
