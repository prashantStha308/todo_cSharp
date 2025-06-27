using System;
using System.Diagnostics;
using System.Text.Json;
using System.IO;

namespace TodoNamespace
{
    public class Task
    {
        private static int count = 0;
        public Task(string name)
        {
            this.Name = name;
            this.IsCompleted = false;
            this.TaskId = ++count;
        }

        public Task() { }

        public string Name { get; set; }
        public bool IsCompleted { get; set; }
        public int TaskId { get; set; }

        public void ToggleCompleteStatus()
        {
            this.IsCompleted = !this.IsCompleted;
            Console.WriteLine($"{this.Name} is {this.IsCompleted}\n");
        }

    }

    public class TaskList
    {
        private static int count = 0;
        public TaskList() 
        {
            Tasks = new List<Task>();
        }

        public TaskList(string name)
        {
            ListName = name;
            Tasks = new List<Task>();
            ListId = ++count;
        }

        public string ListName { get; set; }
        public int ListId { get; set; }
        public List<Task> Tasks { get; set; }

        public void AddTask()
        {
            Console.WriteLine("Enter Task name: ");
            string input = Console.ReadLine();

            if (input != null)
            {
                Tasks.Add(new Task(input));
            }
            else
            {
                Console.WriteLine("\n\tTask's name cannot be null\n");
            }
        }

        public void DeleteTask(int index)
        {
            if (index < 0 || index >= this.Tasks.Count)
            {
                Console.WriteLine("Error in DeleteTask. Input index must be in range of (0 , (taskList.Count - 1) ) . Deletation  failed\n");
                return;
            }
            string taskName = this.Tasks[index].Name;
            Tasks.RemoveAt(index);
            Console.WriteLine($"Task: #{taskName} deleted\n");
        }
    }
    internal class Program
    {
        private List<TaskList> taskLists = new List<TaskList>();

        private const string saveFile = "todolists.json";

        public void SaveData()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(taskLists, options);
                File.WriteAllText(saveFile, json);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving data: {e.Message}");
            }
        }

        public void LoadData()
        {
            try
            {
                if (File.Exists(saveFile))
                {
                    string json = File.ReadAllText(saveFile);
                    taskLists = JsonSerializer.Deserialize<List<TaskList>>(json);
                    if (taskLists == null)
                        taskLists = new List<TaskList>();
                }
                else
                {
                    taskLists = new List<TaskList>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error loading data: {e.Message}");
                taskLists = new List<TaskList>();
            }
        }
        public void AddNewList(string listName)
        {
            taskLists.Add(new TaskList(listName));
            SaveData();
        }

        public static int ParseInt(string input)
        {
            if (int.TryParse(input, out int selected))
            {
                return selected;
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number.\n");
                return -1;
            }
        }

        public static int GetIntInput()
        {
            string input = Console.ReadLine();
            int selected = input.Length == 0 || input == null ? -1 : ParseInt(input);

            return selected;
        }

        public static void SleepClear()
        {
            Thread.Sleep(500);
            Console.Clear();
        }

        public void DeleteTaskList(TaskList taskList, int index)
        {
            // Delete all tasks from the list
            taskList.Tasks.Clear();

            taskLists.RemoveAt(index);
            SaveData();
        }

        public void HandleTask(Task task, int index, TaskList taskList, int listIndex)
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine($"\t\t\t\t\t{task.Name}\t\t---Is Completed?: {task.IsCompleted}");
                Console.WriteLine("Options:\n\t1.Delete Task\t\t 2.Toggle Complete Status\t\t 3.Previous Menu\t\t 4.Exit\n");
                int input = GetIntInput();

                switch (input)
                {
                    case 1:
                        taskList.Tasks.RemoveAt(index);
                        SaveData();
                        return;
                    case 2:
                        task.ToggleCompleteStatus();
                        SaveData();
                        return;
                    case 3:
                        HandleSelectedTaskList(taskList, listIndex);
                        return;
                    case 4:
                        EndProgram();
                        break;
                    default:
                        Console.WriteLine("\nPlease enter a valid option.");
                        SleepClear();
                        break;
                }
            }
        }

        public void HandleSelectedTaskList(TaskList taskList, int index)
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine($"\t\t\t\t\t{taskList.ListName}\n");
                int len = taskList.Tasks.Count;
                Console.WriteLine("Select from give options below.");
                Console.WriteLine(" Options:");

                Console.WriteLine("'p': Previous Menu\t 'd': Delete This List\t\t a: To Add a task");
                Console.WriteLine("\nTracks:\n ");
                if (len == 0)
                {
                    Console.WriteLine("No tasks.\nPress 1.To Add a task");
                    string selected = Console.ReadLine();

                    switch (selected)
                    {
                        case "1":
                            taskList.AddTask();
                            SaveData();
                            return;
                        case "p":
                            ChooseList();
                            return;
                        case "d":
                            DeleteTaskList(taskList, index);
                            SaveData();
                            return;
                        default:
                            Console.WriteLine("Invalid option. Please select either '1','p' or 'd'\n");
                            SleepClear();
                            break;
                    }
                }
                else
                {
                    for (int i = 0; i < len; i++)
                    {
                        string completeStatus = taskList.Tasks[i].IsCompleted ? "Complete" : "InComplete";
                        Console.WriteLine($"{i + 1}. {taskList.Tasks[i].Name}----{completeStatus}");
                    }

                    Console.WriteLine("\nChoose a task by number, or enter 'p' / 'd': ");
                    string input = Console.ReadLine();

                    if (input == "p")
                    {
                        ChooseList();
                        return;
                    }
                    else if (input == "d")
                    {
                        DeleteTaskList(taskList, index);
                        SaveData();
                        return;
                    }
                    else if (input == "a")
                    {
                        taskList.AddTask();
                        SaveData();
                    }
                    else if (int.TryParse(input, out int taskIndex))
                    {
                        if (taskIndex >= 1 && taskIndex <= len)
                        {
                            HandleTask(taskList.Tasks[taskIndex - 1], taskIndex - 1, taskList, index);
                        }
                        else
                        {
                            Console.WriteLine("Invalid task number.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Try again.");
                    }
                }
            }

        }

        public void ChooseList()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("\t\t\t\t\t Task Lists:");
                Console.WriteLine("Options:");
                Console.WriteLine("'p': Go To Previous Menu\t\t'e': Exit program");

                Console.WriteLine("\n Track Lists:\n");
                if (taskLists.Count != 0)
                {
                    for (int i = 0; i < taskLists.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {taskLists[i].ListName}-----Includes {taskLists[i].Tasks.Count} Tasks");
                    }
                }
                else
                {
                    Console.WriteLine("No Task lists present. Press 'a' to create a Task List:");
                    string readInput = Console.ReadLine();
                    if (readInput == "a")
                    {
                        CreateNewList();
                        return;
                    }
                }

                Console.WriteLine("\nChoose a List: ");
                string input = Console.ReadLine();
                if (input == "p")
                {
                    InitialScreen();
                    return;
                }
                else if (input == "e")
                {
                    EndProgram();
                    return;
                }
                int selected = input.Length == 0 || input == null ? -1 : ParseInt(input);

                if (selected > 0 && selected <= taskLists.Count)
                {
                    HandleSelectedTaskList(taskLists[selected - 1], selected - 1);
                    return;
                }
            }
        }

        public void CreateNewList()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("\t\t\t\t\t Task List Creation:");
                Console.WriteLine("Options:");
                Console.WriteLine("'p': To go to previous Screen\t\t 'e': To exit the program");
                Console.WriteLine("Name the new TaskList: ");
                string input = Console.ReadLine();
                if (input == null || input.Length == 0)
                {
                    Console.WriteLine("Heyy!! NO Cheating, a name MUST be given");
                }
                else if (input == "p")
                {
                    InitialScreen();
                }
                else if (input == "e")
                {
                    EndProgram();
                }
                else
                {
                    TaskList newTaskList = new TaskList(input);
                    this.taskLists.Add(newTaskList);

                    Console.WriteLine($"New TaskList: {input} created successfully.");
                    Thread.Sleep(700);

                    HandleSelectedTaskList(newTaskList, taskLists.Count - 1);
                    break;
                }
            }

        }

        public void InitialScreen()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("\t\t\t\t\tTodo App");
                Console.WriteLine("\tOptions:");
                Console.WriteLine("\t 1.Create New List\t\t 2.Choose a List\t\t 3. Exit Program");
                int input = GetIntInput();

                switch (input)
                {
                    case 1:
                        CreateNewList();
                        break;
                    case 2:
                        ChooseList();
                        break;
                    case 3:
                        EndProgram();
                        return;
                    default:
                        Console.WriteLine("Please select a valid option");
                        SleepClear();
                        break;
                }
            }
        }

        private static void EndProgram()
        {
            Environment.Exit(0);
        }

        private void StartTodo()
        {
            Console.Clear();
            LoadData();
            InitialScreen();
        }

        static void Main(string[] args)
        {
            Program app = new Program();
            app.StartTodo();
        }
    }
}