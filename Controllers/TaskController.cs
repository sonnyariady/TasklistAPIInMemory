using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TasklistAPI.Interface;
using TasklistAPI.Model.Request;

namespace TasklistAPI.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskServices _taskServices;
        public TaskController(ITaskServices taskServices)
        {
            _taskServices = taskServices;
        }

        [HttpGet]
        [Route("/api/task/all")]
        public async Task<IActionResult> GetAll()
        {
            var tasks = await _taskServices.GetAll();
            return Ok(tasks);
        }

        [HttpPost]
        [Route("/api/task/create")]
        public async Task<IActionResult> Create([FromBody] TaskRequest input)
        {
            var tasks = await _taskServices.Create(input);
            return Ok(tasks);
        }

        [HttpPut]
        [Route("/api/task/edit/{id}")]
        public async Task<IActionResult> Edit(int id, TaskRequest input)
        {
            var result = await _taskServices.Edit(id, input);
            return Ok(result);
        }

        [HttpDelete]
        [Route("/api/task/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var tasks = await _taskServices.Delete(id);
            return Ok(tasks);
        }

    }
}
