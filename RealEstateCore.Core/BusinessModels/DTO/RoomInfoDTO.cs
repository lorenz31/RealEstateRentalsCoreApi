using System.Collections.Generic;

namespace RealEstateCore.Core.BusinessModels.DTO
{
    public class RoomInfoDTO
    {
        public string RoomName { get; set; }
        public string RoomTypeName { get; set; }
        public int TotalBeds { get; set; }
        public decimal Price { get; set; }
        public List<string> Features { get; set; }
        public List<string> FloorPlans { get; set; }
    }
}