
using System;

public interface IMenuScreenManager
{
    public static event Action<IMenuScreenManager> OnNotificationRecived;
    public bool IsNotification { get; }
    public void NotifyStateChanged()
    {
        OnNotificationRecived?.Invoke(this);
    }
    public void PerformStateUpdate();
}
