
using System.Net.Http;
using System.Net.Http.Json;


namespace FileCopyApp
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private List<string> selectedFiles = new();
        private List<string> destinations = new();

        private async void OnSelectFilesClicked(object sender, EventArgs e)
        {
            var result = await FilePicker.PickMultipleAsync();
            if (result != null)
            {
                selectedFiles.AddRange(result.Select(f => f.FullPath));
                fileList.ItemsSource = null;
                fileList.ItemsSource = selectedFiles;
            }
        }

        private async void OnAddDestinationClicked(object sender, EventArgs e)
        {
            string destination = await DisplayPromptAsync("Destination", "Enter destination path:");
            if (!string.IsNullOrEmpty(destination))
            {
                using (var httpClient = new HttpClient())
                {
                    // Add destination to API
                    var response = await httpClient.PostAsJsonAsync("http://localhost:5000/api/FileCopy/add-destination", destination);
                    if (response.IsSuccessStatusCode)
                    {
                        destinations.Add(destination);
                        destinationList.ItemsSource = null;
                        destinationList.ItemsSource = destinations;
                    }
                }
            }
        }

        private async void OnCopyFilesClicked(object sender, EventArgs e)
        {
            if (selectedFiles.Any())
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.PostAsJsonAsync("http://localhost:5000/api/FileCopy/copy-files", selectedFiles);
                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Success", "Files copied to all destinations", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Error", "Failed to copy files", "OK");
                    }
                }
            }
        }
    }
}
