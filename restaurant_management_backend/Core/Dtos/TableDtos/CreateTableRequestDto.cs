using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.TableDtos
{
    public class CreateTableRequestDto
    {
        public int Number { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; }
    }
}
