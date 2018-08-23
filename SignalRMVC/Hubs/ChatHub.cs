using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using SignalRMVC.Models;

namespace SignalRMVC.Hubs
{
    public class ChatHub : Hub
    {
        static List<User> Users = new List<User>();

        //Send message
        public void Send (string name, string message)
        {
            Clients.All.addMessage(name, message);
        }

        //Connect new user
        public void Connect(string userName)
        {
            var id = Context.ConnectionId;

            if (!Users.Any(x => x.ConnectionId == id))
            {
                Users.Add(new User { ConnectionId = id, Name = userName });

                //Send a message to the current user
                Clients.Caller.onConnected(id, userName, Users);

                //Send a message to all users expext the current one
                Clients.AllExcept(id).onNewUserConnected(id, userName);
            }
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var item = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
                if(item != null)
            {
                Users.Remove(item);
                var id = Context.ConnectionId;
                Clients.All.onUserDisconnected(id, item.Name);
            }
            return base.OnDisconnected(stopCalled);
        }

        public void Hello()
        {
            Clients.All.hello();
        }
    }
}