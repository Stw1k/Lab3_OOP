using System.Collections.ObjectModel;

namespace Lab3_OOP
{
    public partial class EditPage : ContentPage
    {
        private Student _selectedItem;
        public event EventHandler<Student> DataModified;

        private void FillInputs()
        {
            fullnameInput.Text = _selectedItem.FullName;
            facultyInput.Text = _selectedItem.Faculty;
            groupInput.Text = _selectedItem.Group;
            specialityInput.Text = _selectedItem.Speciality;
            marksInput.Text = _selectedItem.Marks;
        }
        public EditPage(Student selected)
        {
            InitializeComponent();

            _selectedItem = selected;
            FillInputs();
        }

       

        private bool IsEmpty(string value)
        {
            return value == string.Empty;
        }

        private bool ValidateAll()
        {
            return (
                !IsEmpty(fullnameInput.Text) &&
                !IsEmpty(facultyInput.Text) &&
                !IsEmpty(groupInput.Text) &&
                !IsEmpty(specialityInput.Text) &&
                !IsEmpty(marksInput.Text) 
                );
        }

        private void UpdateSelected()
        {
            _selectedItem.FullName = fullnameInput.Text;
            _selectedItem.Faculty = facultyInput.Text;
            _selectedItem.Group = groupInput.Text;
            _selectedItem.Speciality = specialityInput.Text;
            _selectedItem.Marks = marksInput.Text;
        }
        private void SaveButtonClicked(object sender, EventArgs e)
        {
            if (ValidateAll())
            {
                UpdateSelected();
                DataModified?.Invoke(this, _selectedItem);
                Navigation.PopModalAsync();
            }
            else
            {
                DisplayAlert("Помилка", "Деякі введення не правильні.", "ОК");
            }
        }

        private void ReturnButtonClicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}