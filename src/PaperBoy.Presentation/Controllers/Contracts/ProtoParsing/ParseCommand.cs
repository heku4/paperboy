using Microsoft.AspNetCore.Http;

namespace PaperBoy.Presentation.Controllers.Contracts.ProtoParsing;

public record ParseCommand(IFormFile FormFile, string FileName);