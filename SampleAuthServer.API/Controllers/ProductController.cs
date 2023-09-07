using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleAuthServer.API.Core.Dtos;
using SampleAuthServer.API.Core.Models;
using SampleAuthServer.API.Core.Services;

namespace SampleAuthServer.API.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class ProductController : CustomBaseController
	{
		private readonly IGenericService<Product, ProductDto> _productService;

		public ProductController(IGenericService<Product, ProductDto> productService)
		{
			_productService = productService;
		}

		[HttpGet]
		public async Task<IActionResult> GetProducts()
		{
			var products = await _productService.GetAllAsync();

			return ActionResultInstance(products);
		}

		[HttpPost]
		public async Task<IActionResult> SaveProduct(ProductDto productDto)
		{
			var response = await _productService.AddAsync(productDto);

			return ActionResultInstance(response);
		}

		[HttpPut]
		public async Task<IActionResult> UpdateProduct(ProductDto productDto)
		{
			var response = await _productService.Update(productDto, productDto.Id);

			return ActionResultInstance(response);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteProduct(int id)
		{
			var response = await _productService.Remove(id);

			return ActionResultInstance(response);
		}
	}
}