using System;

namespace MicroBase.Share.Models.Notifications
{
    public class AddDataToInboxModel
    {
        public Guid Id { get; set; }

        public string Subcontent { get; set; }
        
        public string Bodydescription { get; set; }
        
        public Guid Identityuserid { get; set; }
        
        public string Image { get; set; }

        public Guid? Notificationsettingid { get; set; }
        
        public string Title { get; set; }

        public string Extraparams { get; set; }

        public string Redirectto { get; set; }

        public string Redirecttype { get; set; }

        public string Link{ get; set; }

        public int Notiinscreen { get; set; } = 0;

        public bool Isread { get; set; } = false;
    }
}
