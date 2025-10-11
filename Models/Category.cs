using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyGamesWeb.Models
{

    [Table("Category")]
    public class Category
    {

        public int Id { get; set; }

     
        [MaxLength(30)]

        public required string CategoryName { get; set; }

        public List<Product> Product { get; set; }



    }

}
