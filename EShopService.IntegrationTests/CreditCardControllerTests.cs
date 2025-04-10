using EShopService.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShopService.IntegrationTests
{
    public class CreditCardControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public CreditCardControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task ValidateCard_ValidCard_ReturnsOk()
        {
            string cardNumber = "4111111111111111"; 

            var response = await _client.GetAsync($"/api/creditcard/validate?cardNumber={cardNumber}");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("cardProvider", responseString);
            Assert.Contains("Visa", responseString);
        }

        [Fact]
        public async Task ValidateCard_ValidMasterCardCard_ReturnsOk()
        {
            string cardNumber = "5530016454538418";

            var response = await _client.GetAsync($"/api/creditcard/validate?cardNumber={cardNumber}");

            response.EnsureSuccessStatusCode(); 
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("cardProvider", responseString);
            Assert.Contains("MasterCard", responseString);
        }

        [Fact]
        public async Task ValidateCard_ValidAmericanExpressCard_ReturnsOk()
        {
            string cardNumber = "378523393817437"; 

            var response = await _client.GetAsync($"/api/creditcard/validate?cardNumber={cardNumber}");

            response.EnsureSuccessStatusCode(); 
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("cardProvider", responseString); 
            Assert.Contains("AmericanExpress", responseString); 
        }


        [Fact]
        public async Task ValidateCard_CardNumberTooLong_ReturnsBadRequest()
        {
            string cardNumber = "411111111111111111111111111111111";

            var response = await _client.GetAsync($"/api/creditcard/validate?cardNumber={cardNumber}");

            Assert.Equal(414, (int)response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("Card number too long", responseString);
        }

        [Fact]
        public async Task ValidateCard_CardNumberTooShort_ReturnsBadRequest()
        {
            string cardNumber = "411111"; 

            var response = await _client.GetAsync($"/api/creditcard/validate?cardNumber={cardNumber}");

            Assert.Equal(400, (int)response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("Card number too short.", responseString);
        }

        [Fact]
        public async Task ValidateCard_InvalidChars_ReturnsBadRequest()
        {
            string cardNumber = "4111-1111-1111-111X"; 

            var response = await _client.GetAsync($"/api/creditcard/validate?cardNumber={cardNumber}");

            Assert.Equal(406, (int)response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("Invalid chars", responseString);
        }

        [Fact]
        public async Task ValidateCard_InvalidCard_ReturnsBadRequest()
        {
            string cardNumber = "1234567890123456"; 

            var response = await _client.GetAsync($"/api/creditcard/validate?cardNumber={cardNumber}");

            Assert.Equal(406, (int)response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("Invalid number.", responseString); 
        }

    }
}
