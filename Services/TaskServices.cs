using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using TasklistAPI.Helper;
using TasklistAPI.Interface;
using TasklistAPI.Model;
using TasklistAPI.Model.Entity;
using TasklistAPI.Model.Enum;
using TasklistAPI.Model.Request;
using TasklistAPI.Model.Response;

namespace TasklistAPI.Services
{
    public class TaskServices : ITaskServices
    {
        private readonly AppDbContext _context;

        public TaskServices(AppDbContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }

        public async Task<GlobalResponse> GetAll()
        {
            List<TaskItem> list = new List<TaskItem>(); 
            GlobalResponse response = new GlobalResponse();
            try
            {
                list = await _context.TaskItems.ToListAsync();
                response.status_code = HttpResponseCode.ResponseOK;
            }
            catch (Exception ex)
            {
                response.status_code = HttpResponseCode.ResponseError;
                response.message = ex.Message;
            }
            response.data = list;   
            return response;
        }

        public async Task<GlobalResponse> Edit(int id, TaskRequest input)
        {
            GlobalResponse response = new GlobalResponse();
            ErrorFieldSet errorFieldSet = new ErrorFieldSet();
            try
            {
                var itemcheck = await _context.TaskItems.Where(a => a.Id == id).FirstOrDefaultAsync();
                if (itemcheck != null)
                {
                    errorFieldSet = GetErrorValidation(input);
                    if (errorFieldSet.IsValid)
                    {
                        itemcheck.Title = input.Title;
                        itemcheck.Description = input.Description;
                        itemcheck.DueDate = Convert.ToDateTime(input.DueDate);
                        itemcheck.Priority = Convert.ToInt32(input.Priority);
                        _context.Entry(itemcheck).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        response.data = errorFieldSet;
                        response.message = "This following field is invalid";
                        response.status_code = HttpResponseCode.ResponseError;
                    }
                }
                else
                {
                    response.message = "Edit validation failed";
                    errorFieldSet.AddError("Id", "Id is not registered");
                    response.data = errorFieldSet;
                    response.status_code = HttpResponseCode.ResponseError;
                }
            }
            catch (Exception ex)
            {
                response.message = ex.Message;
                response.status_code = HttpResponseCode.ResponseError;
            }
            return response;
        }

        private ErrorFieldSet GetErrorValidation(TaskRequest input)
        {
            ErrorFieldSet errorFieldSet = new ErrorFieldSet();
            if (string.IsNullOrWhiteSpace(input.Title))
            {
                errorFieldSet.AddError("Title", "Title is required");
            }

            if (string.IsNullOrWhiteSpace(input.Description))
            {
                errorFieldSet.AddError("Description", "Description is required");
            }

            if (input.Priority == null)
            {
                errorFieldSet.AddError("Priority", "Priority is required");
            }

            if (input.DueDate == null)
            {
                errorFieldSet.AddError("DueDate", "DueDate is required");
            }
            return errorFieldSet;
        }

        public async Task<GlobalResponse> Create(TaskRequest input)
        {
            GlobalResponse response = new GlobalResponse();
            ErrorFieldSet errorFieldSet = new ErrorFieldSet();

            try
            {
                errorFieldSet = GetErrorValidation(input);

                if (!errorFieldSet.IsValid)
                {
                    response.data = errorFieldSet;
                    response.message = "This following field is invalid";
                    response.status_code = HttpResponseCode.ResponseError;
                }
                else
                {
                    TaskItem taskItem = new TaskItem();
                    var lastitem = await _context.TaskItems.OrderByDescending(u => u.Id).FirstOrDefaultAsync();
                    int MaxId = lastitem == null ? 0 : lastitem.Id;
                    taskItem.Id = MaxId+1;
                    taskItem.Title = input.Title;
                    taskItem.Description = input.Description;
                    taskItem.DueDate = Convert.ToDateTime(input.DueDate);
                    taskItem.Priority = Convert.ToInt32(input.Priority);
                    _context.TaskItems.Add(taskItem);
                    _context.SaveChanges();
                    response.message = "Success";
                    response.data = taskItem;
                    response.status_code = HttpResponseCode.ResponseOK;
                }

                

            }
            catch (Exception ex)
            {
                response.message = ex.Message;
                response.status_code = HttpResponseCode.ResponseError;

            }

            return response;
        }

        public async Task<GlobalResponse> Delete(int id)
        {
            GlobalResponse response = new GlobalResponse();
            ErrorFieldSet errorFieldSet = new ErrorFieldSet();
            try
            {
                var itemcheck = await _context.TaskItems.Where(a=> a.Id == id).FirstOrDefaultAsync();
                if (itemcheck != null)
                {
                    _context.TaskItems.Remove(itemcheck);
                    await _context.SaveChangesAsync();
                    response.message = "Success";
                    response.status_code = HttpResponseCode.ResponseOK;
                }
                else
                {
                    response.message = "Delete validation failed";
                    errorFieldSet.AddError("Id", "Id is not registered");
                    response.data = errorFieldSet;
                    response.status_code = HttpResponseCode.ResponseError;
                }
            }
            catch (Exception ex)
            {
                response.message = ex.Message;
                response.status_code = HttpResponseCode.ResponseError;
            }
            return response;
        }
    }
}
