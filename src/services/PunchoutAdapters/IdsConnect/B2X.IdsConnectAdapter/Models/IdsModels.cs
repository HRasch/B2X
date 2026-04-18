using System.Xml.Serialization;

namespace B2X.IdsConnectAdapter.Models;

/// <summary>
/// IDS Connect 2.5 Warenkorb Senden Request Model
/// </summary>
[XmlRoot("WarenkorbSenden", Namespace = "")]
public class IdsWarenkorbSenden
{
    [XmlElement("Version")]
    public string Version { get; set; } = "2.5";

    [XmlElement("Kunde")]
    public IdsKunde Kunde { get; set; } = new();

    [XmlArray("Positionen")]
    [XmlArrayItem("Position")]
    public List<IdsWarenkorbPosition> Positionen { get; set; } = new();
}

/// <summary>
/// IDS Connect 2.5 Warenkorb Empfangen Response Model
/// </summary>
[XmlRoot("WarenkorbEmpfangen", Namespace = "")]
public class IdsWarenkorbEmpfangen
{
    [XmlElement("Version")]
    public string Version { get; set; } = "2.5";

    [XmlElement("Bestellnummer")]
    public string Bestellnummer { get; set; } = string.Empty;

    [XmlElement("Status")]
    public string Status { get; set; } = "OK";
}

/// <summary>
/// IDS Connect 2.5 Warenkorb Position Model
/// </summary>
[XmlRoot("Position", Namespace = "")]
public class IdsWarenkorbPosition
{
    [XmlElement("Artikelnummer")]
    public string Artikelnummer { get; set; } = string.Empty;

    [XmlElement("Bezeichnung")]
    public string Bezeichnung { get; set; } = string.Empty;

    [XmlElement("Menge")]
    public decimal Menge { get; set; }

    [XmlElement("Einheit")]
    public string Einheit { get; set; } = "Stk";

    [XmlElement("Preis")]
    public decimal Preis { get; set; }

    [XmlElement("Waehrung")]
    public string Waehrung { get; set; } = "EUR";
}

/// <summary>
/// IDS Connect 2.5 Kunde Model
/// </summary>
[XmlRoot("Kunde", Namespace = "")]
public class IdsKunde
{
    [XmlElement("Id")]
    public string Id { get; set; } = string.Empty;

    [XmlElement("Name")]
    public string Name { get; set; } = string.Empty;

    [XmlElement("Adresse")]
    public string Adresse { get; set; } = string.Empty;

    [XmlElement("PLZ")]
    public string PLZ { get; set; } = string.Empty;

    [XmlElement("Ort")]
    public string Ort { get; set; } = string.Empty;
}

/// <summary>
/// IDS Connect 2.5 Deep-Link Request Model
/// </summary>
[XmlRoot("ArtikelDeepLink", Namespace = "")]
public class IdsDeepLinkRequest
{
    [XmlElement("Version")]
    public string Version { get; set; } = "2.5";

    [XmlElement("Kunde")]
    public IdsKunde Kunde { get; set; } = new();

    [XmlElement("Artikelnummer")]
    public string Artikelnummer { get; set; } = string.Empty;
}

/// <summary>
/// IDS Connect 2.5 Deep-Link Response Model
/// </summary>
[XmlRoot("ArtikelDeepLinkResponse", Namespace = "")]
public class IdsDeepLinkResponse
{
    [XmlElement("Version")]
    public string Version { get; set; } = "2.5";

    [XmlElement("DeepLink")]
    public string DeepLink { get; set; } = string.Empty;

    [XmlElement("Status")]
    public string Status { get; set; } = "OK";

    [XmlElement("ArtikelInfo")]
    public IdsArtikelInfo ArtikelInfo { get; set; } = new();
}

/// <summary>
/// IDS Connect 2.5 Artikel Info Model
/// </summary>
[XmlRoot("ArtikelInfo", Namespace = "")]
public class IdsArtikelInfo
{
    [XmlElement("Artikelnummer")]
    public string Artikelnummer { get; set; } = string.Empty;

    [XmlElement("Bezeichnung")]
    public string Bezeichnung { get; set; } = string.Empty;

    [XmlElement("Preis")]
    public decimal Preis { get; set; }

    [XmlElement("Waehrung")]
    public string Waehrung { get; set; } = "EUR";

    [XmlElement("Verfuegbar")]
    public bool Verfuegbar { get; set; } = true;
}
