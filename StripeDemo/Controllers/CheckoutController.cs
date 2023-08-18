using Microsoft.AspNetCore.Mvc;
using StripeDemo.Models;
using System.Collections.Generic;
using StripeBillingPortal = Stripe.BillingPortal; // Alias for the Stripe.BillingPortal namespace
using Stripe.Checkout;
using Stripe;

namespace StripeDemo.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            List<ProductEntity> productlist1 = new List<ProductEntity>
            {
                  new ProductEntity
                {
                    Product = "Tommy",
                    Rate = 100,
                    Quantity = 1,
                    ImagePath="img/app1.webp"
                },
                new ProductEntity
                {
                    Product = "Jerry",
                    Rate = 200,
                    Quantity = 11,
                    ImagePath = "img/app2.webp"
                }
            };
            return View(productlist1); 
        }
        public IActionResult OrderConfirmation()
        {
            var service = new SessionService();
            Session session = service.Get(TempData["Session"].ToString());

            if(session != null)
            {
                return View("Success");
            }
       
            return View("Login");
        }

        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Checkout(SessionCreateOptions options)
        {
            List<ProductEntity> productlist1 = new List<ProductEntity>
            {
                 new ProductEntity
                {
                    Product = "Tommy",
                    Rate = 1500,
                    Quantity = 2,
                    ImagePath="img/app1.webp"
                },
                new ProductEntity
                {
                    Product = "Jerry",
                    Rate = 1000,
                    Quantity = 1,
                    ImagePath = "img/app2.webp"
                }
            };

            var domain = "https://localhost:7003/";

            options.SuccessUrl = domain + "Checkout/OrderConfirmation";
            options.CancelUrl = domain + "Checkout/Login";
            options.LineItems = new List<SessionLineItemOptions>();
            options.Mode = "payment";

            foreach (var item in productlist1)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Rate * item.Quantity),
                        Currency = "Inr",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.ToString(),
                        }
                    },
                    Quantity = item.Quantity
                };
                options.LineItems.Add(sessionLineItem);
            }

            StripeConfiguration.ApiKey= "sk_test_51NaYqsSBfmSD3Vq0nr0hEhtR5QuD1FLMsAGaDWFBPahK30p8SnmmFFqHFl0hG0Y8ir0VNVWv6CVSn9kcdnXp4K3H00tuw4eMHa";
            
            var service = new SessionService();
            Session session = service.Create(options);

            TempData["Session"] = session.Id;
            Response.Headers.Add("Location",session.Url);

            return new StatusCodeResult(303);  // Pass the session object to the view
        }
    }
}
