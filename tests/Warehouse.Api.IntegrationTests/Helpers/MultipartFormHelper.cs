using System.Net.Http.Headers;

namespace Warehouse.Api.IntegrationTests.Helpers;

public static class MultipartFormHelper
{
    public static MultipartFormDataContent CreateFileForm(
        string fieldName,
        string fileName,
        string contentType,
        byte[] content)
    {
        var form = new MultipartFormDataContent();

        var fileContent = new ByteArrayContent(content);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        form.Add(fileContent, fieldName, fileName);

        return form;
    }
}
