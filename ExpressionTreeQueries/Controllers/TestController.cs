using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpressionTreeQueries.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ExpressionTreeQueries.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<TestController> _logger;
        private readonly DemoCtx _ctx;

        public TestController(ILogger<TestController> logger, DemoCtx ctx)
        {
            _ctx = ctx;
            _logger = logger;
        }

        [HttpGet("/INIT")]
        public async Task<IActionResult> Get()
        {
            var fake_db = new List<Person>()
            {
                new Person("Henning", 26, Gender.Male, 12),
                new Person("Preben", 42, Gender.Female, 12),
                new Person("Kurt", 42, Gender.Male, 15)
            };

            int result = 0;

            if (!_ctx.Persons.Any())
            {
                foreach (var p in fake_db)
                {
                    _ctx.Persons.Add(p);
                }
                result = await _ctx.SaveChangesAsync();
            }

            return Ok(result);
        }

        [HttpGet("/TEST")]
        public async Task<IActionResult> TEST([FromQuery] string propName, string filterValue, string operand, string likeFilter)
        {

            if (!string.IsNullOrWhiteSpace(likeFilter))
            {
                var query1 = _ctx.Persons.Where(t => EF.Functions.Like(t.Name, $"%{likeFilter}%"));
                var sql = query1.ToSql().Replace("\r\n", " ");
                var result = await query1.ToListAsync();
                return Ok(new { GeneratedSql = sql, result });
            }


            var helper = new ExpressionTreeBuilder<Person>();
            var dn = helper.GetDynamicQueryWithExpressionTrees(propName, filterValue, operand);
            var otherdn = helper.GetDynamicQueryWithExpressionTrees("gender", "1", "==");


            var query = _ctx.Persons as IQueryable<Person>;


            query = query.Where(dn);

            query = query.Where(otherdn);

            query = query.AsNoTracking();

            var enumquer = query.AsQueryable();

            try
            {
                var sql = query.ToSql().Replace("\r\n", " ");
                var result = enumquer.ToList();
                return Ok(new { GeneratedSql = sql, result });
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }


            return NoContent();

        }
    }
}
