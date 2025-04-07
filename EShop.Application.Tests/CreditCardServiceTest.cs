using EShop.Domain;
using EShop.Domain.Enums;
using EShop.Domain.Exceptions;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace EShop.Application.Tests
{
    public class CreditCardServiceTest
    {

        [Fact]
        public void Validate_NumberTooShort_ThrowsException()
        {
            var ex = Assert.Throws<CardNumberTooShortException>(() => CreditCardService.ValidateCard("123"));
            Assert.Equal("Card number too short.", ex.Message);
        }

        [Fact]
        public void Validate_NumberTooLong_ThrowsException()
        {
            var ex = Assert.Throws<CardNumberTooLongException>(() => CreditCardService.ValidateCard("123456789123456789123456789"));
            Assert.Equal("Card number too long.", ex.Message);
        }

        [Fact]
        public void Validate_NumberWithinRange_ReturnsCardNumber()
        {
            var cardNumber = "378282246310005";
            Assert.Equal(cardNumber, CreditCardService.ValidateCard(cardNumber));
        }

        [Fact]
        public void Validate_LeftBorder_ReturnsCardNumber()
        {
            var cardNumber = "1488470602414";
            Assert.Equal(cardNumber, CreditCardService.ValidateCard(cardNumber));
        }

        [Fact]
        public void Validate_RightBorder_ReturnsCardNumberr()
        {
            var cardNumber = "4656463574039062384";
            Assert.Equal(cardNumber, CreditCardService.ValidateCard(cardNumber));
        }

        [Theory]
        [InlineData("1234567812??3456")]
        [InlineData("1234567812ab3456")]
        [InlineData("1234567812AS3456")]
        [InlineData("AAAAAAAAAAAAAAAA")]
        public void Validate_InvalidChars_ThrowsException(string data)
        {
            var ex = Assert.Throws<CardNumberInvalidException>(() => CreditCardService.ValidateCard(data));
            Assert.Equal("Invalid chars.", ex.Message);
        }

        [Theory]
        [InlineData("1488 470602414", "1488470602414")]
        [InlineData("1488 47060-2414", "1488470602414")]
        public void Validate_ValidChars_ReturnsCardNumber(string data, string expected)
        {
            Assert.Equal(expected, CreditCardService.ValidateCard(data));
        }

        [Fact]
        public void Validate_InvalidLuhnAlg_ThrowsException()
        {
            var ex = Assert.Throws<CardNumberInvalidException>(() => CreditCardService.ValidateCard("11111111111111"));
            Assert.Equal("Invalid number.", ex.Message);
        }

        [Fact]
        public void Validate_ValidLuhnAlg_ReturnsCardNumber()
        {
            var cardNumber = "49927398716222";
            Assert.Equal(cardNumber, CreditCardService.ValidateCard(cardNumber));
        }

        [Theory]
        [InlineData("3497 7965 8312 797", CreditCardProvider.AmericanExpress)]
        [InlineData("345-470-784-783-010", CreditCardProvider.AmericanExpress)]
        [InlineData("378523393817437", CreditCardProvider.AmericanExpress)]
        [InlineData("4024-0071-6540-1778", CreditCardProvider.Visa)]
        [InlineData("4532 2080 2150 4434", CreditCardProvider.Visa)]
        [InlineData("4532289052809181", CreditCardProvider.Visa)]
        [InlineData("5530016454538418", CreditCardProvider.MasterCard)]
        [InlineData("5551561443896215", CreditCardProvider.MasterCard)]
        [InlineData("5131208517986691", CreditCardProvider.MasterCard)]
        public void GetCardType_ValidNumbers_ReturnsCorrectType(string cardNumber, CreditCardProvider expectedType)
        {
            Assert.Equal(CreditCardService.GetCardType(cardNumber), expectedType);
        }


        [Theory]
        [InlineData("1234567890123456")]  
        [InlineData("9999999999999999")] 
        [InlineData("")] 
        [InlineData("abcdefghijklmno")] 
        [InlineData("4111-1111-aaaa-1111")]  
        public void GetCardType_InvalidNumbers_ThrowsException(string cardNumber)
        {
            var ex = Assert.Throws<CardNumberInvalidException>(() => CreditCardService.GetCardType(cardNumber));
            Assert.Equal("Invalid card type", ex.Message);
        }
    }
}