using System.Net.Http;
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace birthday_update
{
    public class UpdateBirthdayViewer
    {
        private readonly List<string> BirthdayNames = new()
        {
            "Reidar",
            "Andreas",
            "Sofie"
        };

        private int CurrentIndex = 0;
        private const string ViewerApiPath = "https://celebration-poets-web.azurewebsites.net/v1/birthday";
        private readonly HttpClient httpClient = new();

        [FunctionName("UpdateBirthdayViewer")]
        public async Task Run([TimerTrigger("0 */5 * * * *"
        #if DEBUG
        ,RunOnStartup = true
        #endif
        )]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Birthday update trigger function executed at: {DateTime.Now}");

            var birthdayName = BirthdayNames[CurrentIndex];

            string body = JsonConvert.SerializeObject(new Dictionary<string, string>() {
                {"name", birthdayName},
                {"greeting", "Gratulerer med dagen!"}
            });
            var requestData = new StringContent(body, Encoding.UTF8, "application/json");


            await httpClient.PostAsync(ViewerApiPath, requestData);

            IncrementIndex();
        }

        private void IncrementIndex()
        {
            if (CurrentIndex == BirthdayNames.Count - 1)
            {
                CurrentIndex = 0;
            }
            else
            {
                CurrentIndex++;
            }
        }
    }
}
