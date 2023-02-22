using Entertainmentcareers.net.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Entertainmentcareers.net.Client.Services.JobService
{
    public class JobService : IJobService
    {
        private readonly HttpClient _http;
        private readonly NavigationManager _navigationManager;

        public JobService(HttpClient http, NavigationManager navigationManager)
        {
            _http = http;
            _navigationManager = navigationManager;
        }
        public List<Job> jobs { get; set; } = new List<Job>();

        public async Task CreateJob(Job job)
        {
            var result = await _http.PostAsJsonAsync("api/Job", job);
            var response = await result.Content.ReadFromJsonAsync<List<Job>>();
            jobs = response;
            _navigationManager.NavigateTo($"postjob");
        }

        public async Task<Job> GetSingleJob(int id)
        {
            var result = await _http.GetFromJsonAsync<Job>($"api/Job/{id}");
            if (result != null)
            {
                return result;
            }
            else
            {
                throw new Exception("Job not found");
            }
        }


        public async Task UpdateJob(Job job)
        {
            var result = await _http.PutAsJsonAsync("api/Job", job);
            var response = await result.Content.ReadFromJsonAsync<List<Job>>();
            jobs = response;
            _navigationManager.NavigateTo($"Joblist"); 
        }
    }
}
