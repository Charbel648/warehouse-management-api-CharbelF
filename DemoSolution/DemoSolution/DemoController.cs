using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace DemoSolution;

[ApiController]
[Route("[controller]")]
public class DemoController : ControllerBase
{
    private readonly IDistributedCache _cache;

    private List<StudentViewModel> _students =
    [
        new StudentViewModel
        {
            Id = 1,
            Name = "Student1",
            Email = "email1@email.com"
        },
        new StudentViewModel
        {
            Id = 2,
            Name = "Student2",
            Email = "email2@email.com"
        },
        new StudentViewModel
        {
            Id = 3,
            Name = "Student3",
            Email = "email3@email.com"
        },
    ];
    
    public DemoController(IDistributedCache cache)
    {
        _cache = cache;
    }

    [HttpPost]
    public async Task<StudentViewModel> Get([FromBody] int id)
    {
        var cacheKey = $"Student:{id}";
        var cacheValue = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cacheValue)) return JsonSerializer.Deserialize<StudentViewModel>(cacheValue);
        
        
        var student = _students.FirstOrDefault(s => s.Id == id);
        var serializedStudent = JsonSerializer.Serialize(student);
        await _cache.SetStringAsync(
            cacheKey,
            serializedStudent,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
        return student;
    }

}