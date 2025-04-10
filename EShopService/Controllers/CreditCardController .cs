using EShop.Application;
using EShop.Domain.Enums;
using EShop.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EShopService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditCardController : ControllerBase
    {
        [HttpGet("validate")]
        public IActionResult ValidateCard(String cardNumber)
        {
            try
            {
                CreditCardService.ValidateCard(cardNumber);
                return Ok(new { cardProvider = CreditCardService.GetCardType(cardNumber).ToString() });
            }
            catch (CardNumberTooLongException ex)
            {
                return StatusCode(414, new { message = ex.Message, code = 414 });
            }
            catch (CardNumberTooShortException ex)
            {
                return StatusCode(400, new { message = ex.Message, code = 400 });
            }
            catch (CardNumberInvalidException ex)
            {
                return StatusCode(406, new { message = ex.Message, code = 406 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error.", details = ex.Message });
            }
        }
    }
}
