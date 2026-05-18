using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo
{
    public class TodoUI
    {
        private readonly TodoService _todoService = new();

        public void ShowTodos()
        {
            Console.WriteLine("===DANH SÁCH CÔNG VIỆC===");
            foreach (var item in _todoService.GetAll())
            {
                Console.WriteLine(item.ToString());
            }
        }

        public void ShowMenu()
        {
            Console.WriteLine("===MENU===");
            Console.WriteLine("1. Thêm công việc");
            Console.WriteLine("2. Đánh dấu công việc");
            Console.WriteLine("3. Sửa công việc");
            Console.WriteLine("4. Xóa công việc");
            Console.WriteLine("0. Thoát");
            Console.WriteLine("Chọn: ");
        }

        public void Add()
        {
            Console.WriteLine("Nhập nội dung công việc: ");
            String input = Console.ReadLine();
            _todoService.Add(input);
        }

        public void Delete()
        {
            Console.WriteLine("Nhập id muốn xóa: ");
            int id = int.Parse(Console.ReadLine());
            _todoService.Delete(id);
        }

        public void Toggle()
        {
            Console.WriteLine("Nhập Id: ");
            int id = int.Parse(Console.ReadLine());
            _todoService.Toggle(id);
        }

        public void Update()
        {
            Console.WriteLine("Nhập id muốn sửa: ");
            int id = int.Parse(Console.ReadLine());
            Console.WriteLine("Nhập nội dung mới: ");
            String title = Console.ReadLine();
            _todoService.Update(id, title);
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                ShowTodos();
                ShowMenu();
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        Add(); break;
                    case "2":
                        Toggle(); break;
                    case "3":
                        Update(); break;
                    case "4":
                        Delete(); break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ!");
                        break;
                }
            }
            Console.WriteLine("Nhấn nút bất kỳ để tiếp tục");
            Console.ReadLine();
        }
    }
}
