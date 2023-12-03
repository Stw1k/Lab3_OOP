using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3_OOP
{
    internal class LinqSearch
    {
        private bool IsValidPickerValue(string studValue, string argumentValue)
        {
            return string.IsNullOrEmpty(argumentValue) || studValue.Equals(argumentValue);
        }

        public void Search(SearchArgument argument, ObservableCollection<Student> data, ObservableCollection<Student> results)
        {
            var studs = (from stud in data
                         where (
                          IsValidPickerValue(stud.FullName, argument.FullName) &&
                          IsValidPickerValue(stud.Faculty, argument.Faculty) &&
                          IsValidPickerValue(stud.Group, argument.Group) &&
                          IsValidPickerValue(stud.Speciality, argument.Speciality)
                         )
                         select stud).ToList();

            results.Clear();
            foreach (Student stud in studs)
            {
                results.Add(stud);
            }
        }
    }
}