namespace Client.Events
{
    public class LoadingOpenEvent : PubSubEvent<(bool isOpen, bool isLogin)>
    {
    }
}