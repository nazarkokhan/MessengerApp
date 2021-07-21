using System.ComponentModel.DataAnnotations;

namespace MessengerApp.DAL.Entities.Abstract
{
    public interface IEntity<TKey>
    {
        [Key]
        TKey Id { get; set; }
    }
}