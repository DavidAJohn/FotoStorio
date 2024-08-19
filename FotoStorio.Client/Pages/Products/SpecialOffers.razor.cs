using FotoStorio.Client.Shared;
using FotoStorio.Shared.DTOs;
using FotoStorio.Shared.Entities;
using Microsoft.AspNetCore.Components;

namespace FotoStorio.Client.Pages.Products;

public partial class SpecialOffers
{
    [CascadingParameter]
    public Error Error { get; set; }

    [CascadingParameter]
    public BasketState basketState { get; set; }

    List<ProductDTO> products;

    List<DropdownItem> dropdownOptions = new();

    private bool collapseDropdown = true;

    private void ToggleDropdown()
    {
        collapseDropdown = !collapseDropdown;
    }

    private string SortBySelected = "Name (A-Z)";

    private PagingMetadata metadata = new();

    private ProductParameters productParams = new();

    private string errorMessage = "";
    private bool ClearSearch = false;

    private int itemQuantity = 1;

    protected override void OnInitialized()
    {
        SetSortByDropdownOptions();
    }

    protected override async Task OnInitializedAsync()
    {
        await SelectedPage();
    }

    private async Task SelectedPage(int page = 1)
    {
        productParams.PageIndex = page;

        await GetProducts();
    }

    private async Task GetProducts()
    {
        try
        {
            var productsWithMetadata = await productService.GetProductsOnSpecialOfferAsync(productParams);
            products = productsWithMetadata.Items;
            metadata = productsWithMetadata.Metadata;
            errorMessage = "";
        }
        catch (Exception ex)
        {
            Error.ProcessError(ex, "Pages/Products/SpecialOffers.GetProducts()");
            errorMessage = "Could not retrieve list of products";
        }
    }

    private async Task SearchChanged(string searchTerm = "")
    {
        productParams.PageIndex = 1;
        productParams.Search = searchTerm;

        await GetProducts();
    }

    private async Task AddSort(string SortBy, string SortByName)
    {
        productParams.SortBy = SortBy;
        SortBySelected = SortByName;
        ToggleDropdown();
        await GetProducts();
    }

    private async Task ResetSearch()
    {
        ClearSearch = true;
        await SearchChanged();
    }

    private void SetSortByDropdownOptions()
    {
        dropdownOptions.Add(new DropdownItem { Id = 1, OptionName = "Name (A-Z)", OptionRef = "nameAsc" });
        dropdownOptions.Add(new DropdownItem { Id = 2, OptionName = "Amount Saved (Highest First)", OptionRef = "savingDesc" });
        dropdownOptions.Add(new DropdownItem { Id = 3, OptionName = "Price (Highest to Lowest)", OptionRef = "priceDesc" });
        dropdownOptions.Add(new DropdownItem { Id = 4, OptionName = "Price (Lowest to Highest)", OptionRef = "priceAsc" });
        dropdownOptions.Add(new DropdownItem { Id = 5, OptionName = "Name (Z-A)", OptionRef = "nameDesc" });
    }

    private void GoToHomePage()
    {
        navigationManager.NavigateTo("/");
    }

    private async Task AddProductToBasket(int productId)
    {
        var product = new ProductDTO();

        try
        {
            product = await productService.GetProductByIdAsync(productId);
            errorMessage = "";
        }
        catch (Exception ex)
        {
            Error.ProcessError(ex, "Pages/Products/SpecialOffers.AddItemToBasket()");
            errorMessage = "Could not add this product to your basket";
        }

        if (product != null)
        {
            var productToAdd = new BasketProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Brand = product.Brand,
                Category = product.Category,
                SalePrice = product.SalePrice
            };

            var item = new BasketItem { Product = productToAdd, Quantity = itemQuantity };

            basketState.Basket.BasketItems.Add(item);
            await basketState.SaveChangesAsync(); // save to local storage
            basketState.BasketItemCount++;
            toastService.ShowSuccess("A new item has been added to your basket", "Basket Updated");
        }
        else
        {
            errorMessage = "Could not add this product to your basket";
        }
    }
}
