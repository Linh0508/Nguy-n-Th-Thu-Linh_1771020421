using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo
{
    public class Todo
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public bool IsSuccess { get; set; }
        public override string ToString()
        {
            return $"[{(IsSuccess ? 'x' : ' ')}] {Id}:{Title}";
        }
        public string ToFileString()
        {
            return $"{Id}:{Title} {IsSuccess}";
        }
        public static Todo FromFileSrting(string line)
        {
            var parts = line.Split(":");
            return new Todo
            {
                Id = int.Parse(parts[0]),
                Title = parts[1],
                IsSuccess = bool.Parse(parts[2])
            };
        }

        internal static Todo FromFileString(string line)
        {
            throw new NotImplementedException();
        }
    }
}

