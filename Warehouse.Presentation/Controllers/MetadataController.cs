using Microsoft.AspNetCore.Mvc;
using Warehouse.Domain.Exceptions;
using Warehouse.Presentation.Services;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/metadata")]
public class MetadataController : ControllerBase
{
    private readonly ValidationMetadataService _validationMetadataService;

    public MetadataController(ValidationMetadataService validationMetadataService)
    {
        _validationMetadataService = validationMetadataService;
    }

    [HttpGet("validation/{dtoName}")]
    public ActionResult GetValidationMetadata([FromRoute] string dtoName)
    {
        var result = _validationMetadataService.GetValidationMetadata(dtoName);

        if (!result.IsSuccess || result.Value == null)
            throw new NotFoundException("DTO", dtoName);

        return Ok(result.Value);
    }
}
