using API.Exceptions;
using API.Models;
using API.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IStockService _stockService;
        private readonly IOrderService _orderService;
        private readonly IEmailService _emailService;
        private readonly IPaymentService _paymentService;

        public OrderController(IStockService stockService, IOrderService orderService, IEmailService emailService, IPaymentService paymentService)
        {
            _stockService = stockService;
            _orderService = orderService;
            _emailService = emailService;
            _paymentService = paymentService;
        }
        [HttpPost("submit")]
        public IResult Submit(SubmitOrderRequest request)
        {
            if (!request.LineItems.Any())
                return Results.BadRequest("Line Items are required");

            if (!_stockService.IsEnoughStock(request.LineItems))
                return Results.BadRequest("Not enough stock for order");

            string transactionId;
            try
            {
                // 🚩🚩 Look like Innocent but can throw exception 🚀🚀
                transactionId = _paymentService.ChargeCreditCard(request.CreditCardId, request.TotalAmount);
            }
            catch (PaymentException ex)
            {

                return Results.BadRequest("Cannot process Payment");
            }

            var orderId = _orderService.Submit(transactionId, request.LineItems);

            // 🚩🚩 Product Side Effect
            _stockService.UpdateInventory(request.LineItems);

            // 🚩🚩 Product Side Effect
            _emailService.SendOrderConfirmation(request.CustomerId, orderId);

            return Results.Ok(new OrderCreatedResponse(orderId, transactionId));
        }

        public IResult Submit_Refactor(SubmitOrderRequest request)
        {
            var result = ValidateLineItems(request)
                .Bind(_ => ValidateStock(request))
                .TryCatch(_ => _paymentService.ChargeCreditCard(request.CreditCardId, request.TotalAmount), Error.PaymentFailed)
                .Bind(transactionId => SubmitOrder(transactionId, request.LineItems))
                .Tap(_ => _stockService.UpdateInventory(request.LineItems))
                .Tap(a => _emailService.SendOrderConfirmation(request.CustomerId, a.OrderId))
                .Match
                (
                a => Results.Ok(new OrderCreatedResponse(a.OrderId, a.TransactionId)),
                    error => error.Type switch
                    {
                        ErrorType.Validation => Results.BadRequest(error.Description),
                        _ => Results.StatusCode(StatusCodes.Status500InternalServerError)
                    }
                );

            return result;

        }

        #region Validation Methods
        public Result<SubmitOrderRequest> ValidateLineItems(SubmitOrderRequest request)
        {
            return request.LineItems.Any() 
                ? Result<SubmitOrderRequest>.Success(request)
                : Result<SubmitOrderRequest>.Failure(Error.NoLineItems);
        }
        public Result<SubmitOrderRequest> ValidateStock(SubmitOrderRequest request)
        {
            return _stockService.IsEnoughStock(request.LineItems)
                ? Result<SubmitOrderRequest>.Success(request)
                : Result<SubmitOrderRequest>.Failure(Error.NotEnoughStock);
        }
        public Result<(int OrderId, string TransactionId)> SubmitOrder(string transactionId, List<LineItem> lineItems)
        {
            var orderId = _orderService.Submit(transactionId, lineItems);
            return Result<(int OrderId, string TransactionId)>.Success((orderId, transactionId));
        }
        #endregion
    }
}
