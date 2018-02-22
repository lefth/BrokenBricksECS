namespace ECS
{
    public interface IMessageReceiver
    {
        void Notify();
    }



    public interface IMessageReceiver<TMessage>
    {
        void Notify(TMessage message);
    }
}
