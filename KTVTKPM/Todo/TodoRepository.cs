using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo
{
    internal class TodoRepository
    {
        private readonly List<Todo> _todos = new();  // _todo → _todos (dùng nhất quán)
        private int _nextId;
        private readonly string _filePath = "todo.txt";

        public TodoRepository()  // Bỏ class lồng nhau, sửa tên constructor
        {
            LoadFromFile();
        }

        private void LoadFromFile()
        {
            if (!File.Exists(_filePath)) return;
            foreach (var line in File.ReadAllLines(_filePath))
            {
                var item = Todo.FromFileString(line);
                _todos.Add(item);  // _todo → _todos
                if (item.Id >= _nextId)
                {
                    _nextId = item.Id + 1;
                }
            }
        }

        public void SaveChange()
        {
            File.WriteAllLines(_filePath, _todos.Select(x => x.ToFileString()));
        }

        public List<Todo> GetAll() => _todos;  // todos → _todos

        public Todo Add(string title)
        {
            var item = new Todo
            {
                Id = _nextId++,
                Title = title,
                IsSuccess = false
            };
            _todos.Add(item);  // _todo → _todos
            SaveChange();
            return item;  // todo → item
        }

        public bool Delete(int id)
        {
            var todo = _todos.FirstOrDefault(x => x.Id == id);  // _todo → _todos
            if (todo != null)
            {
                _todos.Remove(todo);  // _todo → _todos
                SaveChange();
                return true;
            }
            return false;
        }
        public bool Updata (int id, string title)
        {
            var todo = _todos.FirstOrDefault(x => x.Id == id);  // _todo → _todos
            if (todo != null)
            {
                todo.Title = title; 
                SaveChange();
                return true;
            }
            return false;
        }
        public bool Toggle (int id)
        {
            var todo = _todos.FirstOrDefault(x => x.Id == id);  // _todo → _todos
            if (todo != null)
            {
                todo.IsSuccess = !todo.IsSuccess;
                SaveChange();
                return true;
            }
            return false;
        }
    }
}