using AutoMapper;
using Core.Dtos.InventoryItemDtos;
using Core.Dtos.InventoryTransactionDtos;
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
    public class InventoryTransactionService : IInventoryTransactionService
    {
        private readonly IGenericRepository<InventoryTransaction> _inventoryTransactionRepository;
        private readonly IMapper _mapper;
        public InventoryTransactionService(IGenericRepository<InventoryTransaction> inventoryTransactionRepository, IMapper mapper)
        {
            _inventoryTransactionRepository = inventoryTransactionRepository;
            _mapper = mapper;
        }
        public async Task<ApiResponse<IEnumerable<InventoryItemTransactionResponseDto>>> GetAllInventoryTransactionByRestaurantIdAsync(int restaurantId)
        {
            var inventoryTransactions = await _inventoryTransactionRepository.FindAsync(it => it.InventoryItem.RestaurantId == restaurantId);
            var inventoryItemDtos = _mapper.Map<IEnumerable<InventoryItemTransactionResponseDto>>(inventoryTransactions);
            return ApiResponse<IEnumerable<InventoryItemTransactionResponseDto>>.Ok(inventoryItemDtos, "Başarılı.");
        }
        public async Task<ApiResponse<IEnumerable<InventoryItemTransactionResponseDto>>> GetInventoryTransactionByEmployeeAsync(int employeeId)
        {
            var inventoryTransactions = await _inventoryTransactionRepository.FindAsync(e => e.EmployeeId == employeeId);
            if (inventoryTransactions == null)
            {
                return ApiResponse<IEnumerable<InventoryItemTransactionResponseDto>>.Fail("Envanter işlemi bulunamadı.");
            }
            var inventoryItemDtos = _mapper.Map<IEnumerable<InventoryItemTransactionResponseDto>>(inventoryTransactions);
            return ApiResponse<IEnumerable<InventoryItemTransactionResponseDto>>.Ok(inventoryItemDtos, "Başarılı.");
        }
        public async Task<ApiResponse<NoContent>> CreateInventoryTransactionAsync(CreateInventoryTransactionRequestDto req)
        {
            var inventoryTransaction = _mapper.Map<InventoryTransaction>(req);
            await _inventoryTransactionRepository.AddAsync(inventoryTransaction);
            var inventoryItemDto = _mapper.Map<InventoryItemResponseDto>(inventoryTransaction);
            return ApiResponse<NoContent>.Ok("Envanter işlemi başarıyla eklendi.");
        }
        public async Task<ApiResponse<NoContent>> DeleteInventoryTransactionAsync(int id)
        {
            var inventoryTransaction = await _inventoryTransactionRepository.GetByIdAsync(id);
            if (inventoryTransaction == null)
            {
                return ApiResponse<NoContent>.Fail("Envanter işlemi bulunamadı.");
            }
            await _inventoryTransactionRepository.DeleteAsync(inventoryTransaction);
            return ApiResponse<NoContent>.Ok("Envanter işlemi başarıyla silindi.");
        }
    }
}
