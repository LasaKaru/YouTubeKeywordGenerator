using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace YouTubeKeywordGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            string keyword = KeywordTextBox.Text;
            if (string.IsNullOrWhiteSpace(keyword))
            {
                MessageBox.Show("Please enter a keyword.");
                return;
            }

            try
            {
                var tags = await FetchTags(keyword);
                if (CommaRadioButton.IsChecked == true)
                {
                    ResultTextBox.Text = string.Join(", ", tags);
                }
                else if (HashRadioButton.IsChecked == true)
                {
                    ResultTextBox.Text = string.Join(" ", tags.Select(tag => "#" + tag));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async Task<List<string>> FetchTags(string keyword)
        {
            var tags = new List<string>();
            string apiUrl = $"https://suggestqueries.google.com/complete/search?client=firefox&ds=yt&q={Uri.EscapeDataString(keyword)}";

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync(apiUrl);

                // Log the raw JSON response for debugging
                Console.WriteLine(response);

                var json = JArray.Parse(response);

                // Ensure the second element (index 1) is an array and parse its contents
                var suggestions = json[1] as JArray;
                if (suggestions != null)
                {
                    tags.AddRange(suggestions.Select(s => s.ToString()));
                }
            }

            return tags;
        }
    }
}
