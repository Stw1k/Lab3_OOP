using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;


namespace Lab3_OOP
{
    internal class JsonProgram
    {
        public static void Serialize(string path, ObservableCollection<Student> results)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            using (FileStream fstream = new FileStream(path, FileMode.Create))
            {

                JsonSerializer.Serialize(fstream, results, options);
            }
        }

        public static ObservableCollection<Student> Deserialize(string path)
        {
            ObservableCollection<Student> results = new ObservableCollection<Student>();
            using (FileStream fstream = new FileStream(path, FileMode.Open))
            {
                var studs = JsonSerializer.Deserialize<List<Student>>(fstream);

                foreach (Student stud in studs)
                {
                    results.Add(stud);
                }

                return results;
            }
        }
    }
}