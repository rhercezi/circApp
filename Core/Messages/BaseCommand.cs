namespace Core.Messages
{
    public abstract class BaseCommand : BaseMessage
    {
        public Guid Id { get; set; }
    }
}