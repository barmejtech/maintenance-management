using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Maintenance_management.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<FilesController> _logger;

    // Permitted extensions for uploaded files
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp",
        ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx",
        ".txt", ".csv", ".zip", ".mp4"
    };

    // Maximum file size: 50 MB
    private const long MaxFileSizeBytes = 50L * 1024 * 1024;

    public FilesController(IWebHostEnvironment env, ILogger<FilesController> logger)
    {
        _env = env;
        _logger = logger;
    }

    /// <summary>Upload one or more files. Returns the relative URL for each saved file.</summary>
    [HttpPost("upload")]
    [RequestSizeLimit(50 * 1024 * 1024)]
    public async Task<IActionResult> Upload(List<IFormFile> files)
    {
        if (files is null || files.Count == 0)
            return BadRequest(new { message = "No files provided." });

        var uploadFolder = Path.Combine(_env.WebRootPath, "Files");
        Directory.CreateDirectory(uploadFolder);

        var results = new List<object>();

        foreach (var file in files)
        {
            if (file.Length == 0)
                continue;

            if (file.Length > MaxFileSizeBytes)
                return BadRequest(new { message = $"File '{file.FileName}' exceeds the 50 MB size limit." });

            var ext = Path.GetExtension(file.FileName);
            if (!AllowedExtensions.Contains(ext))
                return BadRequest(new { message = $"Extension '{ext}' is not allowed." });

            // Generate a unique file name to prevent overwrite attacks
            var safeFileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadFolder, safeFileName);

            await using var stream = System.IO.File.Create(filePath);
            await file.CopyToAsync(stream);

            var relativeUrl = $"/Files/{safeFileName}";

            _logger.LogInformation("File '{Original}' saved as '{Saved}'.", file.FileName, safeFileName);

            results.Add(new
            {
                originalName = file.FileName,
                fileName = safeFileName,
                url = relativeUrl,
                contentType = file.ContentType,
                sizeBytes = file.Length
            });
        }

        return Ok(results);
    }

    /// <summary>Download / serve a file by its saved name.</summary>
    [HttpGet("{fileName}")]
    public IActionResult Download(string fileName)
    {
        // Sanitize: only allow the bare file name (no path traversal)
        var safeFileName = Path.GetFileName(fileName);
        var filePath = Path.Combine(_env.WebRootPath, "Files", safeFileName);

        if (!System.IO.File.Exists(filePath))
            return NotFound(new { message = "File not found." });

        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(safeFileName, out var contentType))
            contentType = "application/octet-stream";

        return PhysicalFile(filePath, contentType, safeFileName);
    }

    /// <summary>Delete a file by its saved name (Admin / Manager only).</summary>
    [HttpDelete("{fileName}")]
    [Authorize(Roles = "Admin,Manager")]
    public IActionResult Delete(string fileName)
    {
        var safeFileName = Path.GetFileName(fileName);
        var filePath = Path.Combine(_env.WebRootPath, "Files", safeFileName);

        if (!System.IO.File.Exists(filePath))
            return NotFound(new { message = "File not found." });

        System.IO.File.Delete(filePath);
        _logger.LogInformation("File '{FileName}' deleted.", safeFileName);
        return NoContent();
    }
}
