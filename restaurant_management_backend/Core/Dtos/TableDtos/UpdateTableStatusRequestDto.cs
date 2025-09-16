using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.TableDtos
{
    public class UpdateTableStatusRequestDto
    {
        public int Id { get; set; }
        public string Status { get; set; }
    }
}
