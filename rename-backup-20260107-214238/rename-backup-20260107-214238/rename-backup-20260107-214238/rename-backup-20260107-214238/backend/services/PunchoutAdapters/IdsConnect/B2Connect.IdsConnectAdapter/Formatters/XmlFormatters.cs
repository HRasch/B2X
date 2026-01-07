using System.Xml;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace B2Connect.IdsConnectAdapter.Formatters;

/// <summary>
/// XML Input Formatter for IDS Connect protocol
/// </summary>
public class XmlSerializerInputFormatter : InputFormatter
{
    public XmlSerializerInputFormatter()
    {
        SupportedMediaTypes.Add("application/xml");
        SupportedMediaTypes.Add("text/xml");
    }

    protected override bool CanReadType(Type type)
    {
        return true; // Support all types for XML deserialization
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
    {
        var request = context.HttpContext.Request;
        using var reader = new StreamReader(request.Body);
        var xmlContent = await reader.ReadToEndAsync();

        try
        {
            var serializer = new XmlSerializer(context.ModelType);
            using var stringReader = new StringReader(xmlContent);
            using var xmlReader = XmlReader.Create(stringReader, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null
            });
            var result = serializer.Deserialize(xmlReader);

            return await InputFormatterResult.SuccessAsync(result);
        }
        catch (Exception ex)
        {
            context.ModelState.AddModelError(string.Empty, $"XML deserialization failed: {ex.Message}");
            return await InputFormatterResult.FailureAsync();
        }
    }
}

/// <summary>
/// XML Output Formatter for IDS Connect protocol
/// </summary>
public class XmlSerializerOutputFormatter : OutputFormatter
{
    public XmlSerializerOutputFormatter()
    {
        SupportedMediaTypes.Add("application/xml");
        SupportedMediaTypes.Add("text/xml");
    }

    protected override bool CanWriteType(Type? type)
    {
        return true; // Support all types for XML serialization
    }

    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
    {
        var response = context.HttpContext.Response;

        if (context.Object == null)
        {
            return;
        }

        try
        {
            var serializer = new XmlSerializer(context.ObjectType!);
            var xmlNamespaces = new XmlSerializerNamespaces();
            xmlNamespaces.Add(string.Empty, string.Empty); // Remove default namespaces

            using var stringWriter = new StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = false,
                Encoding = System.Text.Encoding.UTF8
            });

            serializer.Serialize(xmlWriter, context.Object, xmlNamespaces);
            var xmlContent = stringWriter.ToString();

            await response.WriteAsync(xmlContent);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"XML serialization failed: {ex.Message}", ex);
        }
    }
}
