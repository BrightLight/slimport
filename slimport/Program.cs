namespace slimport
{
  using System;
  using System.Linq;
  using System.Xml;

  class Program
  {
    static void Main(string[] args)
    {
      if (args.Length < 2)
      {
        Environment.Exit(1);
      }

      var importProject = new SisulizerProject(args[1], "cs");

      var mainProject = new XmlDocument();
      mainProject.Load(args[0]);
      foreach (XmlElement rowNode in mainProject.SelectNodes("//document/source/node/node/row"))
      {
        var context = SisulizerProject.GetContextOfNode(rowNode);
        var langRow = importProject.GetLangRow(context);
        if (langRow != null)
        {
          var existingLangNode = rowNode.ChildNodes.Cast<XmlNode>().FirstOrDefault(x => x.Name == "lang" && x.Attributes["id"].Value == "cs");
          if (existingLangNode != null)
          {
            // update
            existingLangNode.SetOrUpdateAttribute("date", langRow.Date);
            existingLangNode.SetOrUpdateAttribute("status", langRow.Status);
          }
          else
          {
            // append
            var newLangNode = mainProject.CreateTextNode(langRow.Content);
            mainProject.SetOrUpdateAttribute("id", langRow.Id);
            mainProject.SetOrUpdateAttribute("date", langRow.Date);
            mainProject.SetOrUpdateAttribute("status", langRow.Status);
            rowNode.AppendChild(newLangNode);
          }
        }
      }

      mainProject.Save(@"c:\projects\slimport\demos\newimport.slp");
    }
  }
}
