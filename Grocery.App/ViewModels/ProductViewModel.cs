using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;
using Grocery.App.Views;

namespace Grocery.App.ViewModels
{
    public partial class ProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        public ObservableCollection<Product> Products { get; set; }

        public ProductViewModel(IProductService productService)
        {
            _productService = productService;
            Products = new ObservableCollection<Product>(_productService.GetAll());
        }

        [RelayCommand]
        private async Task AddNewProduct()
        {
            await Shell.Current.GoToAsync(nameof(NewProductView));
        }

        public void RefreshProducts()
        {
            Products.Clear();
            foreach (var p in _productService.GetAll())
                Products.Add(p);
        }
    }
}
