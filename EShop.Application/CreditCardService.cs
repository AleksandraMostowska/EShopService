using EShop.Domain.Enums;
using EShop.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace EShop.Application

{
    public class CreditCardService
    {

        public static string ValidateCard(String cardNumber)
        {
            var cardNumLen = cardNumber.Length;
            if (cardNumLen < 13) throw new CardNumberTooShortException("Card number too short.");
            if (cardNumLen > 19) throw new CardNumberTooLongException("Card number too long.");

            cardNumber = cardNumber.Replace(" ", "").Replace("-","");
            if (!cardNumber.All(char.IsDigit))
                throw new CardNumberInvalidException("Invalid chars.");

            int sum = 0;
            bool alternate = false;

            for (int i = cardNumber.Length - 1; i >= 0; i--)
            {
                int digit = cardNumber[i] - '0';

                if (alternate)
                {
                    digit *= 2;
                    if (digit > 9)
                        digit -= 9;
                }

                sum += digit;
                alternate = !alternate;
            }

            if (sum % 10 != 0) throw new CardNumberInvalidException("Invalid number.");

            return cardNumber;
        }

        public static CreditCardProvider GetCardType(string cardNumber)
        {
            cardNumber = cardNumber.Replace(" ", "").Replace("-", "");

            if (Regex.IsMatch(cardNumber, @"^4(\d{12}|\d{15}|\d{18})$"))
                return CreditCardProvider.Visa;
            if (Regex.IsMatch(cardNumber, @"^(5[1-5]\d{14}|2(2[2-9][1-9]|2[3-9]\d{2}|[3-6]\d{3}|7([01]\d{2}|20\d))\d{10})$"))
                return CreditCardProvider.MasterCard;
            if (Regex.IsMatch(cardNumber, @"^3[47]\d{13}$"))
                return CreditCardProvider.AmericanExpress;
            else throw new CardNumberInvalidException("Invalid card type");
        }

    }
}
