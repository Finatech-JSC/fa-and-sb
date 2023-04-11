using MicroBase.Entity;
using MicroBase.Entity.Notifications;
using MicroBase.Entity.Repositories;
using MicroBase.Share.DataAccess;

namespace MicroBase.Service.Notifications
{
    public interface INotificationUserService : IGenericService<NotificationUser, Guid>
    {
    }

    public class NotificationUserService : GenericService<NotificationUser, Guid>, INotificationUserService
    {
        public NotificationUserService(IRepository<NotificationUser, Guid> repository) 
            : base(repository)
        {
        }

        protected override void ApplyDefaultSort(FindOptions<NotificationUser> findOptions)
        {
            findOptions.SortDescending(s => s.CreatedDate);
        }
    }
}