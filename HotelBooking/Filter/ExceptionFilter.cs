﻿namespace HotelBooking.Filter
{
    using System.Net;
    using HotelBooking.Domain.Exceptions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class ExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<ExceptionFilter> _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            // Handle the exception here
            if (context.Exception is BookingBusinessException)
            {
                context.ModelState.AddModelError(nameof(BookingBusinessException), context.Exception.Message);
                context.Result = new BadRequestObjectResult(context.ModelState);

                return;
            }

            _logger.LogInformation(context.Exception.Message);
            var result = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            context.Result = result;
        }
    }
}
