using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo
{
    public class TodoService
    {
        private readonly TodoRepository _todoRepository = new();

        public List<Todo> GetAll() => _todoRepository.GetAll();
        public Todo Add(string title) => _todoRepository.Add(title);
        public bool Delete(int id) => _todoRepository.Delete(id);
        public bool Update(int id, string title) => _todoRepository.Update(id, title);
        public bool Toggle(int id) => _todoRepository.Toggle(id);
    }
}
