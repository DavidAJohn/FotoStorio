using FotoStorio.Client.Shared;
using FotoStorio.Shared.DTOs;
using FotoStorio.Shared.Entities;
using FotoStorio.Shared.Models.Orders;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FotoStorio.Client.Pages.Checkout;

public partial class OrderSummary
{
    [CascadingParameter]
    public Error Error { get; set; }

    [CascadingParameter]
    public BasketState basketState { get; set; }

    private List<BasketItem> basketItems { get; set; }

    private AddressDTO addressDTO = new();
    private bool ShowErrors;
    private string ErrorMessage;
    private bool ShowPaymentOptions;
    private string SubmitSpinnerHidden = "hidden";
    private string AddressFormID = "address-form";
    private bool ShowAddressExplanationText = false;
    private string ImagePath = "http://localhost/images"; // fallback image path

    protected override async Task OnParametersSetAsync()
    {
        await Task.Run(() => System.Threading.Thread.Sleep(1)); // forces the page to wait before getting the basket
        basketItems = basketState.Basket.BasketItems;
        ShowPaymentOptions = true;

        ImagePath = config["ImageAssetsBaseURI"];

        // populate the address form with the user's default address (if they have one)
        addressDTO = await accountService.GetUserAddressAsync();

        if (!string.IsNullOrWhiteSpace(addressDTO.Street))
        {
            ShowAddressExplanationText = true;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            ShowPaymentOptions = true;

            try
            {
                // initialise the stripe js file 
                await JsRuntime.InvokeVoidAsync("setupStripe", config["Stripe_PublishableKey"]); // wwwroot/scripts/stripe.js

                // get a payment intent from stripe
                await GetPaymentIntentResultAsync();
            }
            catch (JSException ex)
            {
                Error.ProcessError(ex, "Pages/Checkout/OrderSummary.OnAfterRenderAsync()");
                ErrorMessage = "Could not set up payment options. Try reloading the page and/or logging in again";
            }

            firstRender = false;

            if (basketItems == null)
            {
                ShowPaymentOptions = false;
            }
        }
    }

    public async Task GetPaymentIntentResultAsync()
    {
        var basket = basketState.Basket;
        var result = await paymentService.CreateOrUpdatePaymentIntent(basket);

        if (result != null)
        {
            // add created payment intent details to the basket state
            basket.PaymentIntentId = result.PaymentIntentId;
            basket.ClientSecret = result.ClientSecret;

            await basketState.SaveChangesAsync(); // save to local storage
        }
    }

    private async Task DeleteItem(BasketItem item)
    {
        basketState.Basket.BasketItems.Remove(basketState.Basket.BasketItems.FirstOrDefault(
                i => i.Product.Name == item.Product.Name &&
                i.Quantity == item.Quantity
            ));

        await GetPaymentIntentResultAsync();

        await basketState.SaveChangesAsync(); // save to local storage
        basketState.BasketItemCount--;
    }

    private async Task PlaceOrder()
    {
        ShowErrors = false;
        SubmitSpinnerHidden = "";

        var orderAddress = new Address
        {
            FirstName = addressDTO.FirstName,
            LastName = addressDTO.LastName,
            Street = addressDTO.Street,
            SecondLine = addressDTO.SecondLine,
            City = addressDTO.City,
            County = addressDTO.County,
            PostCode = addressDTO.PostCode
        };

        var orderToCreate = new OrderCreateDTO
        {
            Items = basketItems,
            SendToAddress = orderAddress,
            PaymentIntentId = basketState.Basket.PaymentIntentId
        };

        var newOrder = await orderService.CreateOrderAsync(orderToCreate);

        if (newOrder != null)
        {
            SubmitSpinnerHidden = "hidden";
            string paymentResult = "";

            try
            {
                // invoke js method to submit payment
                paymentResult = await JsRuntime.InvokeAsync<string>("payWithCard");
            }
            catch (JSException ex)
            {
                Error.ProcessError(ex, "Pages/Checkout/OrderSummary.PlaceOrder()");
                paymentResult = "There was a problem sending your payment details";
            }

            if (paymentResult != "success")
            {
                ShowErrors = true;
                ErrorMessage = paymentResult;
                toastService.ShowError(paymentResult, "Payment Failed");
            }
            else
            {
                ShowErrors = false;

                // remove the payment intent id and secret from the basket
                basketState.Basket.PaymentIntentId = "";
                basketState.Basket.ClientSecret = "";

                // empty the basket of items
                basketState.Basket.BasketItems.Clear();
                await basketState.SaveChangesAsync();

                // toast message
                toastService.ShowSuccess("Thanks, your order has been placed!", "Order Placed");

                // save the address to the user's account
                await accountService.SaveUserAddressAsync(addressDTO);

                // navigate away to confirmation page
                navigationManager.NavigateTo($"/checkout/success/{newOrder.Id}");
            }
        }
        else
        {
            ShowErrors = true;
            ErrorMessage = "Sorry, there was a problem creating your order";
            SubmitSpinnerHidden = "hidden";
        }
    }

    private void ClearAddress()
    {
        addressDTO = new AddressDTO { };
    }
}
