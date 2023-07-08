using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.Cart
{
    public class CartUpdateDto
    {
        public Guid CartId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
