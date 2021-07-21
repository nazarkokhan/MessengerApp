using System.ComponentModel.DataAnnotations;

namespace MessengerApp.DAL.Entities.Abstract
{
    public abstract class EntityBase : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
    }
}