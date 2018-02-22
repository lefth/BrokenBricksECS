using System.Collections.Generic;



namespace ECS
{
    public class Messenger
    {

        public List<IMessageReceiver> ReceiverList = new List<IMessageReceiver>();

        public void Register(IMessageReceiver receiver)
        {
            ReceiverList.Add(receiver);
        }

        public void Notify()
        {
            foreach (var receiver in ReceiverList) receiver.Notify();
        }
    }



    public class Messenger<TMessage>
    {

        public List<IMessageReceiver<TMessage>> ReceiverList = new List<IMessageReceiver<TMessage>>();

        public void Register(IMessageReceiver<TMessage> receiver)
        {
            ReceiverList.Add(receiver);
        }

        public void Notify(TMessage message)
        {
            foreach (var receiver in ReceiverList) receiver.Notify(message);
        }
    }
}
