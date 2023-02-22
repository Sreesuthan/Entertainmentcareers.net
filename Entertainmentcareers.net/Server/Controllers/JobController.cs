using Dapper;
using Entertainmentcareers.net.Client.Pages;
using Entertainmentcareers.net.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Entertainmentcareers.net.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IConfiguration _config;

        public JobController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("jobtypes")]
        public async Task<ActionResult<List<JobType>>> GetAllJobType()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<JobType> jobTypes = await connection.QueryAsync<JobType>("select * from JobTypes");
            return Ok(jobTypes);
        }

        [HttpGet("countries")]
        public async Task<ActionResult<List<Countries>>> GetAllCountry()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<Countries> countries = await connection.QueryAsync<Countries>("select * from Countries Order by Country");
            return Ok(countries);
        }

        [HttpGet("employmenttypes")]
        public async Task<ActionResult<List<EmploymentTypes>>> GetAllemploymenttype()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<EmploymentTypes> employmentTypes = await connection.QueryAsync<EmploymentTypes>("select * from EmploymentTypes");
            return Ok(employmentTypes);
        }

        [HttpGet("categories")]
        public async Task<ActionResult<List<Categories>>> GetAllCategories()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<Categories> categories = await connection.QueryAsync<Categories>("select * from Categories Order by Category");
            return Ok(categories);
        }

        [HttpGet("companies")]
        public async Task<ActionResult<List<Companies>>> GetAllCompanies()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<Companies> companies = await connection.QueryAsync<Companies>("select * from Companies Order by Company");
            return Ok(companies);
        }

        [HttpGet("categories/legends")]
        public async Task<ActionResult<List<Categories>>> GetAllCategoryLegends()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<Categories> categories = await connection.QueryAsync<Categories>("  select Id, Category from (select c.Id, c.Category, DATEDIFF(MINUTE, j.CreateDate, GETDATE()) as DateDiffMin from Categories as c inner join jobs as j on c.Category=j.Category Where DATEDIFF(MINUTE, j.CreateDate, GETDATE()) < 7200) as new group by id, Category;");
            return Ok(categories);
        }

        [HttpGet("currentlist/{employer}")]
        public async Task<ActionResult<List<Job>>> GetAllCurrentJobs(string employer)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<Job> jobs = await connection.QueryAsync<Job>("select ROW_NUMBER() over(order by (select 1)) as [SrNo], Id, Email, EmploymentType, JobType, CompanyName, JobTitle, Country, State, Active, Category, CreateDate, LastDateToApply from Jobs where Email=@Employer and LastDateToApply > GETDATE() order by Id desc", new { Employer = employer });
            return Ok(jobs);
        }

        [HttpGet("activejobs")]
        public async Task<ActionResult<List<Job>>> GetAllActiveJobs()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<Job> jobs = await connection.QueryAsync<Job>("select ROW_NUMBER() over(order by (select 1)) as [SrNo], Id, JobTitle, EmploymentType, LastDateToApply, CompanyName, Country, State, Category, DATEDIFF(MINUTE, CreateDate, GETDATE()) as DateDiffMin from Jobs where Active='true'");
            return Ok(jobs);
        }

        [HttpGet("pastlist/{employer}")]
        public async Task<ActionResult<List<Job>>> GetAllPastJobs(string employer)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<Job> jobs = await connection.QueryAsync<Job>("select ROW_NUMBER() over(order by (select 1)) as [SrNo], Id, Email, EmploymentType, JobType, CompanyName, JobTitle, Country, State, Active, Category, CreateDate, LastDateToApply from Jobs where Email=@Employer and LastDateToApply < GETDATE() order by Id desc", new { Employer = employer });
            return Ok(jobs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Job>> GetJob(int id)
        {
            try
            {
                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                var job = await connection.QueryFirstAsync<Job>("select * from Jobs where Id=@id",
                    new { id = id });
                return Ok(job);
            }
            catch
            {
                return BadRequest("job details not found...");
            }
        }

        [HttpPost]
        public async Task<ActionResult<List<Job>>> CreateJob(Job job)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("Insert into Jobs values(@Email, @EmploymentType, @JobType, @CompanyName, @ShowCompanyName, @JobTitle, @JobCode, @City, @Country, @State, @Website, @Details, @Instructions, @Salary, @AdditionalComments, @InstructionForUs, @Active, @Employer, @Category, @CreateDate, @LastDateToApply)", job);
            return Ok(await SelectAllJobs(connection));
        }

        [HttpPut]
        public async Task<ActionResult<List<Job>>> UpdateJob(Job job)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update jobs set Email=@Email, EmploymentType=@EmploymentType, JobType=@JobType, CompanyName=@CompanyName, ShowCompanyName=@ShowCompanyName, JobTitle=@JobTitle, JobCode=@JobCode, City=@City, Country=@Country, State=@State, Website=@Website, Details=@Details, Instructions=@Instructions, Salary=@Salary, AdditionalComments=@AdditionalComments, InstructionForUs=@InstructionForUs, Active=@Active, Employer=@Employer, Category=@Category, LastDateToApply=@LastDateToApply where Id = @Id", job);
            return Ok(await SelectAllJobs(connection));
        }

        [HttpDelete("currentlist/{id}/{employer}")]
        public async Task<ActionResult<List<Job>>> DeletecurrentlistJob(int id, string employer)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("delete from Jobs where Id=@Id", new { Id = id });
            return Ok(await connection.QueryAsync<Job>("select ROW_NUMBER() over(order by (select 1)) as [SrNo], Email, EmploymentType, JobType, CompanyName, JobTitle, Country, State, Active, Category, CreateDate, LastDateToApply from Jobs where Email=@Employer and LastDateToApply > GETDATE() order by Id desc", new { Employer = employer }));
        }

        [HttpDelete("pastlist/{id}/{employer}")]
        public async Task<ActionResult<List<Job>>> DeletepastlistJob(int id, string employer)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("delete from Jobs where Id=@Id", new { Id = id });
            return Ok(await connection.QueryAsync<Job>("select ROW_NUMBER() over(order by (select 1)) as [SrNo], Email, EmploymentType, JobType, CompanyName, JobTitle, Country, State, Active, Category, CreateDate, LastDateToApply from Jobs where Email=@Employer and LastDateToApply < GETDATE() order by Id desc", new { Employer = employer }));
        }

        private static async Task<IEnumerable<Job>> SelectAllJobs(SqlConnection connection)
        {
            return await connection.QueryAsync<Job>("select * from Jobs");
        }
    }
}
