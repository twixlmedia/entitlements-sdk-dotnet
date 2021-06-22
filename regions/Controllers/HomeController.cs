using System;
using System.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace regions.Controllers {

    public class HomeController : Controller {

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }


        // The default page which is shown when no action is specified
        public IActionResult Index() {
            ViewData["title"] = "Regions Twixl Entitlements Server";
            return View();
        }

        // Shows the signin form used to ask for the region
        public IActionResult signin_form() {
            ViewData["title"] = "Select your region";
            return View();
        }

        // Check the region from the signin form and return it as the
        // entitlement token. This will allow us to return the correct list
        // of entitlements later on.
        //
        // If the region is invalid, we return an error message.
        //
        // The different parameters are sent as a HTTP POST request.
        public IActionResult signin(string region) {
            try {
                string token = checkRegion(region);
                return okResult(new { token = token });
            } catch (Exception e) {
                return errorResult(e.Message);
            }
        }

        // The signin succeeded, so we are just closing the entitlements
        // popup by calling a specific url
        public IActionResult signin_succeeded(string token) {
            return Redirect("tp-close://self");
        }

        // The signin didn't work, we retrieve the error from the URL and
        // render the error screen.
        public IActionResult signin_error(String error) {
            ViewData["title"] = "An Error Occurred";
            ViewData["error"] = error;
            return View();
        }

        // The entitlements call checks the token to find out what region
        // was selected. Based on the region, it will return a different list
        // of product identifiers which combined with the "hide_unentitled"
        // mode makes the app show or hide different issues.
        //
        // If the token is invalid, we return an error message.
        //
        // The different parameters are sent as a HTTP POST request.
        public IActionResult entitlements(string token) {
            try {

                ArrayList entitledProducts = new ArrayList();

                if (token == "region1") {
                    entitledProducts.Add("com.twixlmedia.demo.region1.issue1");
                    entitledProducts.Add("com.twixlmedia.demo.region1.issue2");
                } else if (token == "region2") {
                    entitledProducts.Add("com.twixlmedia.demo.region2.issue1");
                    entitledProducts.Add("com.twixlmedia.demo.region2.issue2");
                } else {
                    throw new Exception("Invalid region");
                }

                return okResult(new {
                    entitled_products = entitledProducts,
                    entitlement_mode = "hide_unentitled",
                    token = token
                });

            } catch (Exception e) {
                return errorResult(e.Message);
            }
        }

        // This is a helper method to check if the login is correct or not.
        //
        // If the login is correct, we return an entitlement token.
        //
        // If the loign is incorrect, we throw an Exception.
        //
        // This is the place where you can customize the way a username and
        // password are verified. You can for example perform a database call
        // to verify the credentials.
        private string checkRegion(string region) {
            if (region != "region1" && region != "region2") {
                throw new Exception("Invalid region");
            }
            return region;
        }

        // Helper method to return a HTTP 200 response
        private IActionResult okResult(object data) {
            return Json(data);
        }

        // Helper method to return a HTTP 500 response
        private IActionResult errorResult(string error) {
            return Json(new { error = error });
        }

    }
    
}
