namespace SourceDocs.Core.Services
{
    public interface INotificationService
    {
        void Notify(string message, NotificationType type = NotificationType.Info);
    }

    public enum NotificationType
    {
        Info,
        Warning,
        Error
    }
}