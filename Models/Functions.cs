using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace Elementary_schools_overview.Models
{
    class Functions
    {
        public static BitmapImage getImageSource(string path)
        {
            BitmapImage ret = new BitmapImage();
            ret.BeginInit();
            ret.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
            ret.EndInit();
            return ret;
        }

        public static Dictionary<string, string> JSON_converter(string stringToConvert)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(stringToConvert.Substring(0, stringToConvert.IndexOf('}') + 1));
        }

        public static bool checkConversionCondition(ref string convertString)
        {
            if (convertString.IndexOf('}') + 2 >= convertString.Length)
            {
                return false;
            }
            convertString = convertString.Substring(convertString.IndexOf('}') + 2);
            return true;
        }

        /*public static async Task ImageSelectionForAddingSchool(int school_id)
        {
            OpenFileDialog fileBrowser = new OpenFileDialog();
            fileBrowser.Filter = "Image Files (*.jpg, *.png)|*.jpg;*.png";
            if(fileBrowser.ShowDialog() == true)
            {
                using (HttpClient client = new HttpClient())
                {
                    using (MultipartFormDataContent dataForm = new MultipartFormDataContent())
                    {
                        byte[] imgBytes = File.ReadAllBytes(fileBrowser.FileName);
                        ByteArrayContent imgContent = new ByteArrayContent(imgBytes);
                        dataForm.Add(imgContent, "image", Path.GetFileName(fileBrowser.FileName));
                        dataForm.Add(new StringContent(school_id.ToString()), "school_id");
                        HttpResponseMessage response = await client.PostAsync("https://projekat.api.muharemovic.com/api/addIcon", dataForm);
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception("error");
                        }
                    }
                }
            }
        }*/

        /*public static async Task ImageSelectionForAddingSchool(int school_id, string img_path)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg";
            fileDialog.FilterIndex = 1;
            if (fileDialog.ShowDialog() == true)
            {
                //--------- PREPARING IMAGE FOR WEB API -----------
                string selectedImagePath = fileDialog.FileName;
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://projekat.api.muharemovic.com/api/addIcon");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "multipart/form-data");
                MultipartFormDataContent fData = new MultipartFormDataContent();
                byte[] fBytes = File.ReadAllBytes(selectedImagePath);
                ByteArrayContent fContent = new ByteArrayContent(fBytes);
                fData.Add(fContent, "image", Path.GetFileName(selectedImagePath));
                fData.Add(new StringContent(school_id.ToString()), "school_id");

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress);
                request.Content = fData;
                HttpResponseMessage response = await client.SendAsync(request);
                var result = response.Content.ReadAsStringAsync().Result;
               // var response = await client.PostAsync(client.BaseAddress, fData);
                //var result = await response.Content.ReadAsStringAsync();
                if (result.Length != 0)
                {
                    result = result.Trim('[', ']');
                    while (true)
                    {
                        var tmp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(result.Substring(0, result.IndexOf('}') + 1));
                        img_path = tmp.ElementAt(0).Value;
                        if (result.IndexOf('}') + 2 >= result.Length)
                        {
                            break;
                        }
                        result = result.Substring(result.IndexOf('}') + 2);
                    }
                }
                else
                {
                    MessageBox.Show("Something went wrong while uploading school to a database.");
                    return;
                }
                if (img_path == "-1")
                {
                    MessageBox.Show("Something went wrong while uploading school to a database.");
                    return;
                }
            }
        }*/
    }
}
