namespace APIMinima
{
    public class ToDoItemDTO
    {
        public int ToDoId { get; set; }
        public string? Name { get; set; }
        public bool IsComplete { get; set; }

        public ToDoItemDTO() { }

        public ToDoItemDTO(ToDo todoItem) => (ToDoId, Name, IsComplete) = (todoItem.ToDoId, todoItem.Name, todoItem.IsComplete);
    }
}
