using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.App.ViewModels
{
    public partial class NewProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        private readonly ProductViewModel _productViewModel;

        [ObservableProperty] private string name;
        [ObservableProperty] private int stock;
        [ObservableProperty] private decimal price;
        [ObservableProperty] private DateOnly date = DateOnly.FromDateTime(DateTime.Now);
        [ObservableProperty] private string message;

        public NewProductViewModel(IProductService productService, ProductViewModel productViewModel)
        {
            _productService = productService;
            _productViewModel = productViewModel;
        }

        [RelayCommand]
        private void AddProduct()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                Message = "Product name is required.";
                return;
            }

            var product = new Product(0, Name, Stock, Date, Price);
            _productService.Add(product);
            _productViewModel.Products.Add(product);

            Name = string.Empty;
            Stock = 0;
            Price = 0;
            Date = DateOnly.FromDateTime(DateTime.Now);

            _productViewModel.RefreshProducts();
            Shell.Current.GoToAsync("..");
            _productViewModel.RefreshProducts();
        }
    }
}
