namespace slimport
{
  using System;
  using System.Linq;
  using System.Xml;

  public static class AttributesExtensions
  {
    public static void SetOrUpdateAttribute(this XmlNode xmlNode, string attributeName, string value, ILogger logger)
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
        logger?.Delete(attributeName);
      }
      else
      {
        if (attribute.Value != value)
        {
          logger?.Update(attributeName, attribute.Value, value);
          attribute.Value = value;
        }
      }
    }

    public static string GetValueOrDefault(this XmlAttributeCollection xmlAttributeCollection, string attributeName, string defaultValue = default)
    {
      var xmlAttribute = xmlAttributeCollection.OfType<XmlAttribute>().FirstOrDefault(x => string.Equals(attributeName, x.Name, StringComparison.OrdinalIgnoreCase));
      return xmlAttribute != null ? xmlAttribute.Value : defaultValue;
    }
  }
}