using Core.DAOs;

namespace User.Common.DAOs
{
    public class UserEventModel : EventModel
    {
        
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}