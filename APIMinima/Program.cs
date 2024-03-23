using APIMinima;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ToDoDb>(opt => opt.UseInMemoryDatabase("ToDoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

RouteGroupBuilder toDoItems = app.MapGroup("/todoitems");

toDoItems.MapGet("/", GetAllToDos);
toDoItems.MapGet("/complete", GetCompleteToDos);
toDoItems.MapGet("/{id}", GetToDo);
toDoItems.MapPost("/", CreatToDo);
toDoItems.MapPut("/{id}", UpdateToDo);
toDoItems.MapDelete("/{id}", DeleteToDo);

static async Task<IResult> GetAllToDos(ToDoDb db)
{
    return TypedResults.Ok(await db.ToDos.Select(x => new ToDoItemDTO(x)).ToArrayAsync());
}

static async Task<IResult> GetCompleteToDos(ToDoDb db)
{
    return TypedResults.Ok(await db.ToDos.Where(q => q.IsComplete).Select(x => new ToDoItemDTO(x)).ToArrayAsync());
}

static async Task<IResult> GetToDo(int id, ToDoDb db)
{
    //ToDo? toDo = await db.ToDos.FindAsync(id);
    //if (toDo == null)
    //{
    //    return TypedResults.NotFound();
    //}
    //else
    //{
    //    return TypedResults.Ok(toDo);
    //}

    return await db.ToDos.FindAsync(id) is ToDo toDo ? TypedResults.Ok(new ToDoItemDTO(toDo)) : TypedResults.NotFound();
}

static async Task<IResult> CreatToDo(ToDoItemDTO toDoItem, ToDoDb db)
{
    ToDo toDo = new ToDo()
    {
        Name = toDoItem.Name,
        IsComplete = toDoItem.IsComplete
    };

    db.ToDos.Add(toDo);
    await db.SaveChangesAsync();

    toDoItem = new ToDoItemDTO(toDo);

    return TypedResults.Created($"/todoitems/{toDoItem.ToDoId}", toDoItem);
}

static async Task<IResult> UpdateToDo(int id, ToDoItemDTO inputToDo, ToDoDb db)
{
    ToDo? toDo = await db.ToDos.FindAsync(id);

    if (toDo == null) return TypedResults.NotFound();

    toDo.Name = inputToDo.Name;
    toDo.IsComplete = inputToDo.IsComplete;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static async Task<IResult> DeleteToDo(int id, ToDoDb db)
{
    ToDo? toDo = await db.ToDos.FindAsync(id);

    if (toDo == null) return TypedResults.NotFound();

    db.ToDos.Remove(toDo);
    await db.SaveChangesAsync();
    ToDoItemDTO toDoItem = new ToDoItemDTO(toDo);
    return TypedResults.Ok(toDoItem);
}


//app.MapGet("/", () => "Hello World!");
//toDoItems.MapGet("/", async(ToDoDb db) =>
//    await db.ToDos.ToListAsync());

//toDoItems.MapGet("/complete", async (ToDoDb db) =>
//    await db.ToDos.Where(q => q.IsComplete).ToListAsync());

//toDoItems.MapGet("/{id}", async (int id, ToDoDb db) =>
//    await db.ToDos.FindAsync(id)
//        is ToDo toDo
//            ? Results.Ok(toDo) : Results.NotFound());

//toDoItems.MapPost("/", async (ToDo toDo, ToDoDb db) =>
//    {
//        db.ToDos.Add(toDo);
//        await db.SaveChangesAsync();
//        return Results.Created($"/todoitems/{toDo.ToDoId}", toDo);
//    });

//toDoItems.MapPut("/{id}", async (int id, ToDo inputToDo, ToDoDb db) =>
//    {
//        ToDo? toDo = await db.ToDos.FindAsync(id);
//        if (toDo is null)
//        {
//            return Results.NotFound();
//        }

//        toDo.Name = inputToDo.Name;
//        toDo.IsComplete = inputToDo.IsComplete;
//        db.Entry(toDo).State = EntityState.Modified;

//        await db.SaveChangesAsync();

//        return Results.NoContent();
//    });

//toDoItems.MapDelete("/{id}", async(int id, ToDoDb db) =>
//    {
//        if (await db.ToDos.FindAsync(id) is ToDo toDo)
//        {
//            db.ToDos.Remove(toDo);
//            await db.SaveChangesAsync();
//            return Results.Ok(toDo);
//        }
//        return Results.NotFound();
//    });


app.Run();
