namespace SourceDocs.Core.Services
{
    public interface INotificationService
    {
        void Notify(string message, NotificationType type = NotificationType.Dedault);
    }

    public enum NotificationType
    {
        Dedault,
        Info,
        Success,
        Warning,
        Error
    }
}