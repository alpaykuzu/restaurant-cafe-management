using AutoMapper;
using Core.Dtos.ReservationDtos;
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
    public class ReservationService : IReservationService
    {
        private readonly IGenericRepository<Reservation> _reservationRepository;
        private readonly IGenericRepository<Table> _tableRepository;
        private readonly IMapper _mapper;

        public ReservationService(IGenericRepository<Reservation> reservationRepository, IMapper mapper, IGenericRepository<Table> tableRepository)
        {
            _reservationRepository = reservationRepository;
            _mapper = mapper;
            _tableRepository = tableRepository;
        }

        public async Task<ApiResponse<ReservationResponseDto>> CreateReservationAsync(CreateReservationRequestDto req)
        {
            // Aynı masa ve saat için başka rezervasyon var mı?
            var existingReservations = await _reservationRepository.FindAsync(r =>
                r.TableId == req.TableId &&
                r.RestaurantId == req.RestaurantId &&
                r.ReservationTime == req.ReservationTime &&
                r.Status != "Cancelled" 
            );

            if (existingReservations.Any())
            {
                return ApiResponse<ReservationResponseDto>.Fail("Bu saat için masa zaten rezerve edilmiş.");
            }

            var reservation = _mapper.Map<Reservation>(req);
            reservation.CreatedAt = DateTime.UtcNow;
            reservation.Status = "Pending";

            await _reservationRepository.AddAsync(reservation);

            var table = await _tableRepository.GetByIdAsync(req.TableId);
            if (table != null)
            {
                table.Status = "Reserved";
                await _tableRepository.UpdateAsync(table);
            }

            var dto = _mapper.Map<ReservationResponseDto>(reservation);
            return ApiResponse<ReservationResponseDto>.Ok(dto, "Rezervasyon başarıyla oluşturuldu.");
        }

        public async Task<ApiResponse<ReservationResponseDto>> UpdateReservationAsync(UpdateReservationRequestDto req)
        {
            var reservation = await _reservationRepository.GetByIdAsync(req.Id);
            if (reservation == null)
                return ApiResponse<ReservationResponseDto>.Fail("Rezervasyon bulunamadı.");

            // Masa değişirse/rezervasyon zamanı değişirse çakışma kontrolü
            var conflictReservations = await _reservationRepository.FindAsync(r =>
                r.Id != req.Id &&
                r.TableId == reservation.TableId &&
                r.RestaurantId == reservation.RestaurantId &&
                r.ReservationTime == req.ReservationTime &&
                r.Status != "Cancelled"
            );

            if (conflictReservations.Any())
            {
                return ApiResponse<ReservationResponseDto>.Fail("Bu saat için masa zaten rezerve edilmiş.");
            }

            reservation.ReservationTime = req.ReservationTime;
            reservation.NumberOfGuests = req.NumberOfGuests;
            reservation.SpecialRequests = req.SpecialRequests;
            reservation.Status = req.Status;

            await _reservationRepository.UpdateAsync(reservation);

            var dto = _mapper.Map<ReservationResponseDto>(reservation);
            return ApiResponse<ReservationResponseDto>.Ok(dto, "Rezervasyon güncellendi.");
        }

        public async Task<ApiResponse<NoContent>> CancelReservationAsync(int id)
        {
            var reservation = await _reservationRepository.GetByIdAsync(id);
            if (reservation == null)
                return ApiResponse<NoContent>.Fail("Rezervasyon bulunamadı.");

            reservation.Status = "Cancelled";
            await _reservationRepository.UpdateAsync(reservation);

            return ApiResponse<NoContent>.Ok("Rezervasyon iptal edildi.");
        }

        public async Task<ApiResponse<IEnumerable<ReservationResponseDto>>> GetReservationsByRestaurantIdAsync(int restaurantId)
        {
            var reservations = await _reservationRepository.FindAsync(r => r.RestaurantId == restaurantId);
            var dtos = _mapper.Map<IEnumerable<ReservationResponseDto>>(reservations);

            return ApiResponse<IEnumerable<ReservationResponseDto>>.Ok(dtos, "Rezervasyonlar listelendi.");
        }

        public async Task<ApiResponse<IEnumerable<ReservationResponseDto>>> GetReservationsByCustomerAsync(string customerName)
        {
            var reservations = await _reservationRepository.FindAsync(r => r.CustomerName.ToLower().Contains(customerName.ToLower()));
            var dtos = _mapper.Map<IEnumerable<ReservationResponseDto>>(reservations);

            return ApiResponse<IEnumerable<ReservationResponseDto>>.Ok(dtos, "Müşteri rezervasyonları listelendi.");
        }
    }
}
