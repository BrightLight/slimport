namespace slimport
{
  using System.Xml;

  public static class AttributesExtensions
  {
    public static void SetOrUpdateAttribute(this XmlNode xmlNode, string attributeName, string value)
    {
      var attribute = xmlNode.Attributes[attributeName];
      if (attribute == null)
      {
        if (string.IsNullOrEmpty(value))
        {
          return;
        }

        attribute = xmlNode.OwnerDocument.CreateAttribute(attributeName);
        xmlNode.Attributes.Append(attribute);
      }

      if (string.IsNullOrEmpty(value))
      {
        xmlNode.Attributes.Remove(attribute);
      }
      else
      {
        attribute.Value = value;
      }
    }

    public static string GetValueOrDefault(this XmlAttribute xmlAttribute, string defaultValue = default)
    {
      return xmlAttribute != null ? xmlAttribute.Value : defaultValue;
    }
  }
}