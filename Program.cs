using Microsoft.AspNetCore.Mvc.Diagnostics;
using System.Threading.Tasks;
using System.Linq;


var builder = WebApplication.CreateBuilder(args);
var AllowCors = "_AllowCors";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowCors, builder => {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseCors(AllowCors);

// Define a list to store ToDoTask objects
var tasks = new List<TaskData>();

// Define a route to handle GET requests at the root ("/api/")
app.MapGet("/api/", () => new { Message = ("To do list") });

// Define a route to handle GET requests for all tasks ("/api/tasks")
app.MapGet("/api/tasks", () => tasks);

// Define a route to handle GET requests for a specific task by ID ("/api/tasks/{id}")
app.MapGet("/api/tasks/{id}", (int id) => { tasks.Select(t => t.Id == id); });

// Define a route to handle PUT requests to update a task by ID ("/api/tasks/{id}")
app.MapPut("/api/tasks/{id}", (int id, TaskData updatedTask) =>
{
    // Find the task with the specified ID
    var taskToUpdate = tasks.FirstOrDefault(t => t.Id == id);

    if (taskToUpdate != null)
    {
        // Update the task properties with data from the request body
        taskToUpdate.Text = updatedTask.Text;
        taskToUpdate.Done = updatedTask.Done;

        // Return an OK response with the updated task
        return Results.Ok(taskToUpdate);
    }

    // Return a Not Found response if the task with the specified ID is not found
    return Results.NotFound();
});

// Define a route to handle DELETE requests to remove a task by ID ("/api/tasks/{id}")
app.MapDelete("api/tasks/{id}", (int id) =>
{
    // Find the task with the specified ID
    var findTask = tasks.FirstOrDefault(t => t.Id == id);

    if (findTask != null)
    {
        // Remove the task from the tasks list
        tasks.Remove(findTask);
    }
});

// Initialize a variable to keep track of the highest task ID
int highestId = 0;

// Define a route to handle POST requests to create a new task ("/api/tasks/")
app.MapPost("/api/tasks/", (TaskData task) =>
{
    // Assign a new unique ID to the task
    task.Id = highestId + 1;

    // Add the task to the tasks list
    tasks.Add(task);

    // Update the highestId variable to keep it unique
    highestId = task.Id;
});


app.UseCors(AllowCors);
app.Run();

public class TaskData
{
    public string Text { get; set; }
    public int Id { get; set; }
    public bool Done { get; set; }
}