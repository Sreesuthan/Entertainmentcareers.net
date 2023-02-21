using Dapper;
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

        [HttpPost]
        public async Task<ActionResult<List<Job>>> CreateJob(Job job)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("Insert into Jobs values(@Email, @EmploymentType, @JobType, @CompanyName, @ShowCompanyName, @JobTitle, @JobCode, @City, @Country, @State, @Website, @Details, @Instructions, @Salary, @AdditionalComments, @InstructionForUs, @Active, @Employer, @Category, @CreateDate, @LastDateToApply)", job);
            return Ok(await SelectAllJobs(connection));
        }

        private static async Task<IEnumerable<Job>> SelectAllJobs(SqlConnection connection)
        {
            return await connection.QueryAsync<Job>("select * from Jobs");
        }
    }
}
