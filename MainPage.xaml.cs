using System.Collections.ObjectModel;
using System.Reflection;

namespace Lab3_OOP
{
    public partial class MainPage : ContentPage
    {
        private string _filePath = "";
        private string _resultsFilePath = "";
        private bool _isError = false;
        private Dictionary<string, List<string>> _pickersData = new Dictionary<string, List<string>>
        {
            { "FullName", new List<string>() },
            { "Faculty", new List<string>() },
            { "Group", new List<string>() },
            { "Speciality", new List<string>() },
            { "Marks", new List<string>() },
        };
        private ObservableCollection<Student> _deserializedData = new ObservableCollection<Student>();
        private ObservableCollection<Student> _dataToShow = new ObservableCollection<Student>();
        private LinqSearch _analizator = new LinqSearch();
        Student argument = new Student();

        public MainPage()
        {
            InitializeComponent();
            ResultsListView.ItemsSource = _dataToShow;
        }

        private void AddPickerValue(Student stud)
        {
            string[] selectedProperties = { "FullName", "Faculty", "Group", "Speciality" };

            foreach (string propertyName in selectedProperties)
            {
                PropertyInfo property = stud.GetType().GetProperty(propertyName);
                var pickerList = _pickersData[propertyName];

                if (property != null)
                {
                    string propertyValue = property.GetValue(stud) as string;
                    if (!string.IsNullOrEmpty(propertyValue) && !pickerList.Contains(propertyValue))
                    {
                        pickerList.Add(propertyValue);
                    }
                }
            }
        }

        private void ClearCriterias()
        {
            foreach (var list in _pickersData.Values)
            {
                list.Clear();
            }
        }

        private void ClearPickersValues()
        {
            fullnamePicker.ItemsSource = null;
            facultyPicker.ItemsSource = null;
            groupPicker.ItemsSource = null;
            specialityPicker.ItemsSource = null;
            
        }

        private void SortPickersValues()
        {
            foreach (var list in _pickersData.Values)
            {
                list.Sort();
            }
        }

        private void AddItemSourses()
        {
            SortPickersValues();
            fullnamePicker.ItemsSource = _pickersData["FullName"];
            facultyPicker.ItemsSource = _pickersData["Faculty"];
            groupPicker.ItemsSource = _pickersData["Group"];
            specialityPicker.ItemsSource = _pickersData["Speciality"];
        }

        private void FillPickers()
        {
            ClearCriterias();

            foreach (var stud in _deserializedData)
            {
                AddPickerValue(stud);
            }

            ClearPickersValues();
            AddItemSourses();
        }

        private void UpdateFilters()
        {
            fullnamePicker.SelectedItem = null;
            facultyPicker.SelectedItem = null;
            groupPicker.SelectedItem = null;
            specialityPicker.SelectedItem = null;
            _dataToShow.Clear();
            notFoundLabel.IsVisible = false;
        }

        private async Task<string> PickFile()
        {
            _isError = false;
            try
            {
                var result = await FilePicker.PickAsync();
                if (result != null)
                {
                    string resultPath = result.FullPath;

                    if (File.Exists(resultPath))
                    {
                        string extension = Path.GetExtension(resultPath);
                        if (extension.Equals(".json", StringComparison.OrdinalIgnoreCase))
                        {
                            return resultPath;
                        }
                        else
                        {
                            _isError = true;
                            await DisplayAlert("Помилка", "Обраний файл не є JSON-файлом.", "ОК");
                        }
                    }
                    else
                    {
                        _isError = true;
                        await DisplayAlert("Помилка", "Обраного файлу не існує.", "ОК");
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                _isError = true;
                await DisplayAlert("Помилка", $"{ex.Message}", "ОК");
                return string.Empty;
            }
        }

        private async void OnPickFileClicked(object sender, EventArgs e)
        {
            _filePath = await PickFile();

            if (!string.IsNullOrEmpty(_filePath) && !_isError)
            {
                FileInfo fileInfo = new FileInfo(_filePath);

                if (fileInfo.Length > 0)
                {
                    try
                    {
                        _deserializedData = JsonProgram.Deserialize(_filePath);
                        foreach (var elem  in _deserializedData)
                        {
                            _pickersData["FullName"].Add(elem.FullName);
                            _pickersData["Faculty"].Add(elem.Faculty);
                            _pickersData["Group"].Add(elem.Group);
                            _pickersData["Speciality"].Add(elem.Speciality);
                            _pickersData["Marks"].Add(elem.Marks);
                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Помилка", ex.Message, "ОК");
                    }

                    UpdateFilters();
                    FillPickers();
                    filters.IsVisible = true;
                }
                else
                {
                    await DisplayAlert("Помилка", "Файл пустий.", "ОК");
                }
            }
        }

        private async void SaveJsonBtnClicked(object sender, EventArgs e)
        {
            _resultsFilePath = await PickFile();

            if (!string.IsNullOrEmpty(_resultsFilePath) && !_isError)
            {
                try
                {
                    JsonProgram.Serialize(_resultsFilePath, _dataToShow);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Помилка", ex.Message, "ОК");
                }

                await DisplayAlert("Інформація", "Результати збережені!", "ОК");
            }
        }

        private void OnCleanBtnClicked(object sender, EventArgs e)
        {
            UpdateFilters();
        }

        private SearchArgument FormCriteria()
        {
            SearchArgument newCriteria = new SearchArgument();

            newCriteria.FullName = fullnamePicker.SelectedItem != null ? fullnamePicker.SelectedItem as string : string.Empty;
            newCriteria.Faculty = facultyPicker.SelectedItem != null ? facultyPicker.SelectedItem as string : string.Empty;
            newCriteria.Group = groupPicker.SelectedItem != null ? groupPicker.SelectedItem as string : string.Empty;
            newCriteria.Speciality = specialityPicker.SelectedItem != null ? specialityPicker.SelectedItem as string : string.Empty;

            return newCriteria;
        }

        private async void OnSearchBtnClicked(object sender, EventArgs e)
        {
            var search = FormCriteria();
            try
            {
               
                argument.FullName = search.FullName;
                argument.Faculty = search.Faculty;
                argument.Group = search.Group;
                argument.Speciality = search.Speciality;

            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", $"Error in FormCriteria(): {ex.Message}", "ОК");
                return;
            }

            try
            {
                
                _analizator.Search(search, _deserializedData, _dataToShow);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", $"{ex.Message}", "ОК");
            }

            if (_dataToShow.Count > 0 && !string.IsNullOrEmpty(_filePath))
            {
                ResultsContainer.IsVisible = true;
                notFoundLabel.IsVisible = false;
            }
            else
            {
                ResultsContainer.IsVisible = false;
                if (!string.IsNullOrEmpty(_filePath))
                {
                    notFoundLabel.IsVisible = true;
                }
            }

        }

        async private void OnHelpBtnClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Про програму",
                "Програма розроблена для серіалізації/десеріалізації файлів JSON, відображення їх вмісту за допомогою .NET MAUI. Оберіть файл з вашого комп'ютера формату JSON. " +
                "Наявна також можливість пошуку у списку, що складає вміст файлу, за певними критеріями, додавання елементів до списку, редагування елементів та їх видалення. " +
                "Можна зберегти результати пошуку у списку в JSON-форматі в окремий файл.",
                "ОК");
        }

        private void DeleteButtonClicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            Student workToDel = (Student)button.BindingContext;

            _dataToShow.Remove(workToDel);
            _deserializedData.Remove(workToDel);

            FillPickers();

            try
            {
                JsonProgram.Serialize(_filePath, _deserializedData);
            }
            catch (Exception ex)
            {
                DisplayAlert("Помилка", ex.Message, "ОК");
            }

        }

        private async void OnChangeBtnClicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            Student selectedItem = (Student)button.BindingContext;
            if (selectedItem != null)
            {
                var secondPage = new EditPage(selectedItem);
                secondPage.DataModified += (s, modifiedData) =>
                {
                    if (modifiedData != null)
                    {
                        ResultsListView.ItemsSource = null;

                        int idxInShowData = _dataToShow.IndexOf(selectedItem);
                        int idxInAllData = _dataToShow.IndexOf(selectedItem);

                        _dataToShow[idxInShowData] = modifiedData;
                        _deserializedData[idxInAllData] = modifiedData;

                        ResultsListView.ItemsSource = _dataToShow;
                        DisplayAlert("Інформація", "Зміни успішно внесені!", "ОК");
                        FillPickers();

                        try
                        {
                            JsonProgram.Serialize(_filePath, _deserializedData);
                        }
                        catch (Exception ex)
                        {
                            DisplayAlert("Помилка", ex.Message, "ОК");
                        }
                    }
                    else
                    {
                        DisplayAlert("Помилка", "Внести зміни не вдалося.", "ОК");
                    }
                };

                await Navigation.PushModalAsync(secondPage);
            }
            else
            {
                await DisplayAlert("Помилка", "Сталася помилка!.", "ОК");
            }
        }

        private async void OnAddElemBtnClicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            Student newStud = new Student();

            var secondPage = new EditPage(newStud);
            secondPage.DataModified += (s, modifiedData) =>
            {
                if (modifiedData != null)
                {
                    _deserializedData.Insert(0, modifiedData);
                    _analizator.Search(argument, _deserializedData, _dataToShow);

                    DisplayAlert("Інформація", "Студент додан!", "ОК");
                    FillPickers();

                    try
                    {
                        JsonProgram.Serialize(_filePath, _deserializedData);
                    }
                    catch (Exception ex)
                    {
                        DisplayAlert("Помилка", ex.Message, "ОК");
                    }
                }
                else
                {
                    DisplayAlert("Помилка", "Внести зміни не вдалося.", "ОК");
                }
            };

            await Navigation.PushModalAsync(secondPage);
        }
    }
}