using System.ComponentModel.DataAnnotations;

namespace RMall.Models.UserPromotions
{
    public class CreateUserPromotion
    {
        [Required]
        public int promotionId { get; set; }

    }
}
