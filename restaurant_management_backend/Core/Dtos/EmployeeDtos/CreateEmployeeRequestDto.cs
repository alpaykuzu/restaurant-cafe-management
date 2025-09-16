using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.EmployeeDtos
{
    public class CreateEmployeeRequestDto
    {
        public int UserId { get; set; }
        public int RestaurantId { get; set; }
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
    }
}
