using System.Xml.Serialization;

namespace Demoulas.ProfitSharing.Data.Cli.DiagramEntities;


[XmlRoot(ElementName = "Node")]
public class Node
{

    [XmlAttribute(AttributeName = "Id")]
    public string? Id { get; set; }

    [XmlAttribute(AttributeName = "Label")]
    public string? Label { get; set; }

    [XmlAttribute(AttributeName = "ChangeTrackingStrategy")]
    public string? ChangeTrackingStrategy { get; set; }

    [XmlAttribute(AttributeName = "PropertyAccessMode")]
    public string? PropertyAccessMode { get; set; }

    [XmlAttribute(AttributeName = "ProductVersion")]
    public string? ProductVersion { get; set; }

    [XmlAttribute(AttributeName = "Annotations")]
    public string? Annotations { get; set; }

    [XmlAttribute(AttributeName = "Category")]
    public string? Category { get; set; }

    [XmlAttribute(AttributeName = "Group")]
    public string? Group { get; set; }

    [XmlAttribute(AttributeName = "Name")]
    public string? Name { get; set; }

    [XmlAttribute(AttributeName = "BaseClass")]
    public string? BaseClass { get; set; }

    [XmlAttribute(AttributeName = "IsAbstract")]
    public bool IsAbstract { get; set; }

    [XmlAttribute(AttributeName = "Type")]
    public string? Type { get; set; }

    [XmlAttribute(AttributeName = "MaxLength")]
    public string? MaxLength { get; set; }

    [XmlAttribute(AttributeName = "Field")]
    public string? Field { get; set; }

    [XmlAttribute(AttributeName = "BeforeSaveBehavior")]
    public string? BeforeSaveBehavior { get; set; }

    [XmlAttribute(AttributeName = "AfterSaveBehavior")]
    public string? AfterSaveBehavior { get; set; }

    [XmlAttribute(AttributeName = "IsPrimaryKey")]
    public bool IsPrimaryKey { get; set; }

    [XmlAttribute(AttributeName = "IsForeignKey")]
    public bool IsForeignKey { get; set; }

    [XmlAttribute(AttributeName = "IsRequired")]
    public bool IsRequired { get; set; }

    [XmlAttribute(AttributeName = "IsIndexed")]
    public bool IsIndexed { get; set; }

    [XmlAttribute(AttributeName = "IndexName")]
    public string? IndexName { get; set; }

    [XmlAttribute(AttributeName = "IsShadow")]
    public bool IsShadow { get; set; }

    [XmlAttribute(AttributeName = "IsAlternateKey")]
    public bool IsAlternateKey { get; set; }

    [XmlAttribute(AttributeName = "IsConcurrencyToken")]
    public bool IsConcurrencyToken { get; set; }

    [XmlAttribute(AttributeName = "IsUnicode")]
    public bool IsUnicode { get; set; }

    [XmlAttribute(AttributeName = "ValueGenerated")]
    public string? ValueGenerated { get; set; }

    [XmlAttribute(AttributeName = "Dependent")]
    public string? Dependent { get; set; }

    [XmlAttribute(AttributeName = "Principal")]
    public string? Principal { get; set; }

    [XmlAttribute(AttributeName = "Inverse")]
    public string? Inverse { get; set; }
}

[XmlRoot(ElementName = "Nodes")]
public class Nodes
{

    [XmlElement(ElementName = "Node")] public List<Node> Node { get; set; } = new List<Node>();
}

[XmlRoot(ElementName = "Link")]
public class Link
{

    [XmlAttribute(AttributeName = "Source")]
    public string? Source { get; set; }

    [XmlAttribute(AttributeName = "Target")]
    public string? Target { get; set; }

    [XmlAttribute(AttributeName = "Category")]
    public string? Category { get; set; }

    [XmlAttribute(AttributeName = "From")]
    public string? From { get; set; }

    [XmlAttribute(AttributeName = "To")]
    public string? To { get; set; }

    [XmlAttribute(AttributeName = "Name")]
    public string? Name { get; set; }

    [XmlAttribute(AttributeName = "Annotations")]
    public string? Annotations { get; set; }

    [XmlAttribute(AttributeName = "IsUnique")]
    public bool IsUnique { get; set; }

    [XmlAttribute(AttributeName = "Label")]
    public string? Label { get; set; }
}

[XmlRoot(ElementName = "Links")]
public class Links
{

    [XmlElement(ElementName = "Link")] public List<Link> Link { get; set; } = new List<Link>();
}

[XmlRoot(ElementName = "Condition")]
public class Condition
{

    [XmlAttribute(AttributeName = "Expression")]
    public string? Expression { get; set; }
}

[XmlRoot(ElementName = "Setter")]
public class Setter
{

    [XmlAttribute(AttributeName = "Property")]
    public string? Property { get; set; }

    [XmlAttribute(AttributeName = "Value")]
    public string? Value { get; set; }
}

[XmlRoot(ElementName = "Style")]
public class Style
{

    [XmlElement(ElementName = "Condition")]
    public Condition? Condition { get; set; }

    [XmlElement(ElementName = "Setter")]
    public Setter? Setter { get; set; }

    [XmlAttribute(AttributeName = "TargetType")]
    public string? TargetType { get; set; }

    [XmlAttribute(AttributeName = "GroupLabel")]
    public string? GroupLabel { get; set; }

    [XmlAttribute(AttributeName = "ValueLabel")]
    public bool ValueLabel { get; set; }
}

[XmlRoot(ElementName = "Styles")]
public class Styles
{

    [XmlElement(ElementName = "Style")] public List<Style> Style { get; set; } = new List<Style>();
}

[XmlRoot(ElementName = "Property")]
public class Property
{

    [XmlAttribute(AttributeName = "Id")]
    public string? Id { get; set; }

    [XmlAttribute(AttributeName = "Group")]
    public string? Group { get; set; }

    [XmlAttribute(AttributeName = "DataType")]
    public string? DataType { get; set; }

    [XmlAttribute(AttributeName = "Description")]
    public string? Description { get; set; }

    [XmlAttribute(AttributeName = "Label")]
    public string? Label { get; set; }
}

[XmlRoot(ElementName = "Properties")]
public class Properties
{

    [XmlElement(ElementName = "Property")] public List<Property> Property { get; set; } = new List<Property>();
}

[XmlRoot(ElementName = "DirectedGraph")]
public class DirectedGraph
{

    [XmlElement(ElementName = "Nodes")]
    public Nodes? Nodes { get; set; }

    [XmlElement(ElementName = "Links")]
    public Links? Links { get; set; }

    [XmlElement(ElementName = "Styles")]
    public Styles? Styles { get; set; }

    [XmlElement(ElementName = "Properties")]
    public Properties? Properties { get; set; }

    [XmlAttribute(AttributeName = "GraphDirection")]
    public string? GraphDirection { get; set; }

    [XmlAttribute(AttributeName = "xmlns")]
    public string? Xmlns { get; set; }
}


