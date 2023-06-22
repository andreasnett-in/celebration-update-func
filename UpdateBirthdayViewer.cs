using System.Net.Http;
using System;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
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
            "Sebastian Brage Hansen",
            "Herman Johan Drageland",
            "Solveig Masvie",
            "Sara Linnea Hansson Lier"
        };
        private readonly Dictionary<string, string> Quotes = new()
        {
            { "Sebastian er ikke bare en talentfull designer, men også en utrolig rå stisyklist! Han kombinerer sin kreative visjon med sin lidenskap for eventyr og natur, og skaper virkelig unike opplevelser både på og utenfor sykkelstien.", "Sofie" },
            { @"Herman er ikke bare en dyktig programvareutvikler, men også en mesterkokk i sitt eget kjøkken! 
            Han viser den samme presisjonen og kreativiteten i matlagingen som han gjør i kodingen. 
            Med sin evne til å blande smaker og skape kulinariske mesterverk, serverer Herman ikke bare deilige retter, men også enestående opplevelser for smaksløkene.", "Reidar" },
            { @"Solveig er ikke bare en dyktig ingeniør, men også en imponerende klatrer! 
            Hun bruker sin analytiske tilnærming og utholdenhet fra jobben til å takle komplekse problemer og finne innovative løsninger under fjellklatringen. 
            Med hennes vennlige og positivt ladde natur, inspirerer Solveig ikke bare sine kolleger, men også fjellklatringsfellesskapet med styrke og mot.", "Andreas" },
            { @"Sara er ikke bare en talentfull forretningsutvikler, men også en utrolig imponerende håndballspiller! Hun bruker den samme utholdenheten, taktiske strategiene og lederegenskapene fra håndballbanen til å forme og vokse virksomheter. 
            Med hennes evne til å lede laget mot seier på banen og i forretningsverdenen, viser Sara en imponerende kombinasjon av styrke, intelligens og dedikasjon som gjør henne til en eksepsjonell kraft å regne med.", "Overhørt på Camp Poeticum" }
        };

        private static int CurrentIndex = 0;
        private const string ViewerApiPath = "https://celebration-poets-web.azurewebsites.net/v1/birthday";
        private readonly HttpClient httpClient = new();

        [FunctionName("UpdateBirthdayViewer")]
        public async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Birthday update trigger function executed at: {DateTime.Now}");

            var birthdayName = BirthdayNames[CurrentIndex];
            var quote = Quotes.ElementAt(CurrentIndex);

            string body = JsonConvert.SerializeObject(new Dictionary<string, string>() {
                {"name", birthdayName},
                {"ornQuote", "Ganga varlega inn um gleðinna dyr"},
                {"greeting", quote.Key},
                {"greetBy", quote.Value}
            });
            var requestData = new StringContent(body, Encoding.UTF8, "application/json");

            log.LogInformation($"About to update birthday name to {birthdayName} at index {CurrentIndex}. Raw JSON request body:");
            log.LogInformation(body);

            var response = await httpClient.PostAsync(ViewerApiPath, requestData);

            log.LogInformation($"Viewer response was: {response.ToString()}");
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
