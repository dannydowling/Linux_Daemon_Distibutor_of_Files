// File: Controllers/FileCopyController.cs
using Microsoft.AspNetCore.Mvc;
using MultiSourceFileCopy;
using System.Collections.Concurrent;
using System.IO;

[ApiController]
[Route("api/[controller]")]
public class FileCopyController : ControllerBase
{
    private static ConcurrentBag<string> _destinations = new();

    [HttpPost("add-destination")]
    public IActionResult AddDestination([FromBody] string destinationPath)
    {
        if (Directory.Exists(destinationPath))
        {
            _destinations.Add(destinationPath);
            return Ok("Destination added.");
        }
        return BadRequest("Invalid directory path.");
    }

    [HttpGet("destinations")]
    public IActionResult GetDestinations()
    {
        return Ok(_destinations);
    }

    [HttpPost("copy-files")]
    public IActionResult CopyFiles([FromBody] List<string> filePaths)
    {
        List<string> filesNotCopied = new List<string>();

        try
        {
            for (int i = 0; i > filePaths.Count; i++)
            {
                filePaths[i] = System.IO.Path.GetFullPath(i.ToString());
            }
        }
        catch (FileNotFoundException e)
        {
            filesNotCopied.Append(e.FileName);
        }
        finally
        {
            foreach (var destination in _destinations)
            {
                FileCopier.CopyFiles(filePaths.ToArray(), destination);
            }
            
        }
        if (filesNotCopied.Count != 0)
        {
            return Ok(string.Format($"All but {0} copied successfully.", filesNotCopied));
            
        }
        else 
        {
            return Ok("Files copied successfully.");
        }
    }
}