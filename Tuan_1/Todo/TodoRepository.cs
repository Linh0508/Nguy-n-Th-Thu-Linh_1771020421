using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Todo
{
    public class TodoRepository
    {
        private readonly List<Todo> _todos = new();
        private int _nextId;
        private readonly string _filePath = "todo.txt";

        public TodoRepository()
        {
            loadFromFile();
        }

        private void loadFromFile()
        {
            if (!File.Exists(_filePath)) return;
            foreach (var line in File.ReadAllLines(_filePath))
            {
                var item = Todo.fromFileString(line);
                _todos.Add(item);
                if (item.Id >= _nextId)
                {
                    _nextId = item.Id + 1;
                }
            }
        }

        public void SaveChanges()
        {
            var lines = _todos.Select(t => t.toFileString()).ToArray();
            File.WriteAllLines(_filePath, lines);
        }

        public List<Todo> GetAll()
        {
            return _todos;
        }

        public Todo Add(string title)
        {
            var item = new Todo
            {
                Id = _nextId++,
                Title = title,
                IsSuccess = false
            };
            _todos.Add(item);
            SaveChanges();
            return item;
        }

        public bool Delete(int id)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            if (todo != null)
            {
                _todos.Remove(todo);
                SaveChanges();
                return true;
            }
            return false;
        }

        public bool Update(int id, string title)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            if (todo != null)
            {
                todo.Title = title;
                SaveChanges();
                return true;
            }
            return false;
        }

        public bool Toggle(int id)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            if (todo != null)
            {
                todo.IsSuccess = !todo.IsSuccess;
                SaveChanges();
                return true;
            }
            return false;
        }
    }
}
