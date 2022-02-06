using System;
using System.Collections.Generic;

namespace TCP_Server.Models
{
    public partial class CarTb
    {
        public int Id { get; set; }
        public string Vendor { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int Year { get; set; }
    }
}
