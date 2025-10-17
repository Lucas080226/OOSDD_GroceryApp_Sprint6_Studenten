using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.App.Views;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;

namespace Grocery.App.ViewModels
{
    public partial class ProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;

        public ObservableCollection<Product> Products { get; set; }

        // Declare an instance property of type Client
        [ObservableProperty]
        private Client client;

        public ProductViewModel(IProductService productService, GlobalViewModel global)
        {
            _productService = productService;
            Products = new ObservableCollection<Product>(_productService.GetAll());

            // Assign the actual logged-in client from global viewmodel
            client = global.Client;
        }

        [RelayCommand]
        private async Task AddNewProduct()
        {
            // Use the instance variable 'Client', not the type
            if (client != null && client.Role == Role.Admin)
            {
                await Shell.Current.GoToAsync(nameof(NewProductView));
            }
        }

        public void RefreshProducts()
        {
            Products.Clear();
            foreach (var p in _productService.GetAll())
                Products.Add(p);
        }
    }
}
