using AutoMapper;
using Core.Dtos.InventoryItemDtos;
using Core.Interfaces;
using Core.Models;
using Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class InventoryItemService : IInventoryItemService
    {
        private readonly IGenericRepository<InventoryItem> _inventoryItemRepository;
        private readonly IMapper _mapper;
        public InventoryItemService(IGenericRepository<InventoryItem> inventoryItemRepository, IMapper mapper)
        {
            _inventoryItemRepository = inventoryItemRepository;
            _mapper = mapper;
        }
        public async Task<ApiResponse<IEnumerable<InventoryItemResponseDto>>> GetAllInventoryByRestaurantIdAsync(int restaurantId)
        {
            var inventoryItems = await _inventoryItemRepository.FindAsync(i => i.RestaurantId == restaurantId);
            var inventoryItemDtos = _mapper.Map<IEnumerable<InventoryItemResponseDto>>(inventoryItems);
            return ApiResponse<IEnumerable<InventoryItemResponseDto>>.Ok(inventoryItemDtos, "Envanter getirildi.");
        }
        public async Task<ApiResponse<InventoryItemResponseDto>> GetInventoryItemByIdAsync(int id)
        {
            var inventoryItem = await _inventoryItemRepository.GetByIdAsync(id);
            if (inventoryItem == null)
            {
                return ApiResponse<InventoryItemResponseDto>.Fail("Envanter öğesi bulunamadı.");
            }
            var inventoryItemDto = _mapper.Map<InventoryItemResponseDto>(inventoryItem);
            return ApiResponse<InventoryItemResponseDto>.Ok(inventoryItemDto, "Envanter öğesi getirildi.");
        }
        public async Task<ApiResponse<InventoryItemResponseDto>> UpdateInventoryItemAsync(UpdateInventoryItemRequestDto req)
        {
            var inventoryItem = await _inventoryItemRepository.GetByIdAsync(req.Id);
            if (inventoryItem == null)
            {
                return ApiResponse<InventoryItemResponseDto>.Fail("Envanter öğesi bulunamadı.");
            }
            _mapper.Map(req, inventoryItem);
            await _inventoryItemRepository.UpdateAsync(inventoryItem);
            var updatedInventoryItemDto = _mapper.Map<InventoryItemResponseDto>(inventoryItem);
            return ApiResponse<InventoryItemResponseDto>.Ok(updatedInventoryItemDto, "Envanter öğesi güncellendi.");
        }
        public async Task<ApiResponse<NoContent>> UpdateStockLevelAsync(int id, int newStockLevel)
        {
            var inventoryItem = await _inventoryItemRepository.GetByIdAsync(id);
            if (inventoryItem == null)
            {
                return ApiResponse<NoContent>.Fail("Envanter öğesi bulunamadı.");
            }
            inventoryItem.StockLevel = newStockLevel;
            await _inventoryItemRepository.UpdateAsync(inventoryItem);
            return ApiResponse<NoContent>.Ok("Stok seviyesi güncellendi.");
        }
        public async Task<ApiResponse<InventoryItemResponseDto>> CreateInventoryItemAsync(CreateInventoryItemRequestDto req)
        {
            var inventoryItem = _mapper.Map<InventoryItem>(req);
            await _inventoryItemRepository.AddAsync(inventoryItem);
            var createdInventoryItemDto = _mapper.Map<InventoryItemResponseDto>(inventoryItem);
            return ApiResponse<InventoryItemResponseDto>.Ok(createdInventoryItemDto, "Envanter öğesi oluşturuldu.");
        }
        public async Task<ApiResponse<NoContent>> DeleteInventoryItemAsync(int id)
        {
            var inventoryItem = await _inventoryItemRepository.GetByIdAsync(id);
            if (inventoryItem == null)
            {
                return ApiResponse<NoContent>.Fail("Envanter öğesi bulunamadı.");
            }
            await _inventoryItemRepository.DeleteAsync(inventoryItem);
            return ApiResponse<NoContent>.Ok("Envanter öğesi silindi.");
        }
    }
}
