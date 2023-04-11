using System.ComponentModel.DataAnnotations;

namespace MicroBase.Entity
{
    public abstract class BaseEntity<TKey> : BaseEntity, IBaseEntity<TKey>
    {
        [Key]
        public virtual TKey Id { get; set; }

        public bool IsDelete { get; set; } = false;

        public Guid? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public Guid? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;

        public override object GetIdValue()
        {
            return Id;
        }
    }

    public abstract class BaseEntity : IBaseEntity
    {
        public abstract object GetIdValue();
    }

    public interface IBaseEntity<TKey> : IBaseEntity
    {
        TKey Id { get; set; }

        bool IsDelete { get; set; }

        Guid? CreatedBy { get; set; }

        DateTime CreatedDate { get; set; }

        Guid? ModifiedBy { get; set; }

        DateTime? ModifiedDate { get; set; }
    }

    public interface IBaseEntity
    {
        object GetIdValue();
    }

    public interface IBaseMenuAction
    {
        string ActionType { get; set; }

        string ActionToScreen { get; set; }

        string ActionToLink { get; set; }
    }
}